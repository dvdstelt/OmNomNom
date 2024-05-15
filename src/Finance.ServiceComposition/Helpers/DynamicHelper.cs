namespace Finance.ServiceComposition.Helpers;

public static class DynamicHelper
{
    public static void TrySetTotalPrice(dynamic vm, decimal price)
    {
        // There's no way to detect if a dynamic property has been set, so let's do it like this
        try
        {
            vm.TotalPrice += price;
        }
        catch (Exception e)
        {
            vm.TotalPrice = price;
        }
    }
}