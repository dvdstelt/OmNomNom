using System.Dynamic;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Email;

public class OrderSummary(FinanceDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderData = await request.Bind<OrderData>();
        
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var deliveryOptionCollection = dbContext.Database.GetCollection<DeliveryOption>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderData.OrderId).Single();
        var deliveryOption = deliveryOptionCollection.Query().Where(s => s.DeliveryOptionId == order.DeliveryOptionId)
            .Single();
        dynamic deliveryOptionModel = new ExpandoObject();
        deliveryOptionModel.Price = deliveryOption.Price;
        
        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionLoaded()
        {
            DeliveryOptionId = order.DeliveryOptionId,
            DeliveryOption = deliveryOptionModel
        });
        
        var vm = request.GetComposedResponseModel();
        vm.BillingAddress = order.BillingAddress;
        vm.DeliveryOption = deliveryOptionModel;
        vm.TotalPrice = order.Items.Select(s => s.Price * s.Quantity).Sum() + deliveryOption.Price;
    }
    
    class OrderData
    {
        [FromRoute] public Guid OrderId { get; set; }
    }
}