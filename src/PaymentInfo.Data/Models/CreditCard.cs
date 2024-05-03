namespace PaymentInfo.Data.Models;

public class CreditCard
{

    public Guid CreditCardId { get; set; }
    public Guid CustomerId { get; set; }
    public string CardHolder { get; set; }
    public string CardType { get; set; }
    public string LastDigits { get; set; }
    public string ExpiryDate { get; set; }
    public string Currency { get; set; }
    public DateTime LastUsed { get; set; }

    /// <summary>
    /// Token from payment provider
    /// </summary>
    public string Token { get; set; }
}