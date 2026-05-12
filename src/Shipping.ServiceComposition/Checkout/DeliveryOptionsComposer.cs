using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.DeliveryOptions;
using Shipping.ServiceComposition.Events;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class DeliveryOptionsComposer(ShippingDbContext dbContext, IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // Get all available delivery options
        var deliveryOptions = await dbContext.DeliveryOptions.ToListAsync(ct);

        // The slice is the user's just-selected option (synchronously
        // written by DeliveryOptionSubmitHandler). Fall back to the DB
        // for orders that have already been submitted previously.
        Guid? selectedDeliveryOption = null;
        var slice = await workflow.Read<DeliveryOptionSlice>(orderId, DeliveryOptionWorkflowSlice.Key, ct);
        if (slice is not null)
        {
            selectedDeliveryOption = slice.DeliveryOptionId;
        }
        else
        {
            var fromDb = await dbContext.Orders
                .Where(s => s.OrderId == orderId)
                .Select(s => s.DeliveryOptionId)
                .FirstOrDefaultAsync(ct);
            if (fromDb is { } id && id != Guid.Empty)
                selectedDeliveryOption = id;
        }

        var optionsModel = Mapper.MapToDictionary(deliveryOptions);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionsLoaded
        {
            DeliveryOptions = optionsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.DeliveryOptions = optionsModel.Select(kvp => kvp.Value);
        vm.SelectedDeliveryOption = selectedDeliveryOption;
    }
}
