using System.Dynamic;
using Microsoft.Extensions.Options;
using Shipping.Data.Models;

namespace Shipping.ServiceComposition.DeliveryOptions;

public class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(IEnumerable<DeliveryOption> deliveryOptions)
    {
        var deliveryOptionsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var option in deliveryOptions)
        {
            deliveryOptionsViewModel[option.DeliveryOptionId] = MapToViewModel(option);
        }
        
        return deliveryOptionsViewModel;
    }

    public static dynamic MapToViewModel(DeliveryOption option)
    {
        dynamic vm = new ExpandoObject();
        vm.DeliveryOptionId = option.DeliveryOptionId;
        vm.Name = option.Name;
        vm.Description = option.Description;

        return vm;
    }
}