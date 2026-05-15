namespace PaymentInfo.Data.Models;

public class CreditCard
{

    public Guid CreditCardId { get; set; }
    public Guid CustomerId { get; set; }
    public string CardHolder { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string LastDigits { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public DateTime LastUsed { get; set; }

    /// <summary>
    /// Token from payment provider
    /// </summary>
    public string Token { get; set; } = null!;
}