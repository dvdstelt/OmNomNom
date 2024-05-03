namespace PaymentInfo.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid CreditCardId { get; set; }
}