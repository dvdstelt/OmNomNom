namespace Finance.ServiceComposition.Helpers;

public static class DynamicHelper
{
    public static void TrySetTotalPrice(dynamic vm, decimal price)
    {
        // Check if TotalPrice property exists using reflection or a dictionary-based approach
        if (vm is IDictionary<string, object> dict)
        {
            if (dict.ContainsKey("TotalPrice"))
            {
                vm.TotalPrice += price;
            }
            else
            {
                vm.TotalPrice = price;
            }
        }
        else
        {
            // Fallback for non-dictionary dynamic objects
            try
            {
                var existingValue = vm.TotalPrice;
                vm.TotalPrice = existingValue + price;
            }
            catch
            {
                vm.TotalPrice = price;
            }
        }
    }
}
