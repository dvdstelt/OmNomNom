using PaymentInfo.Data.Models;

namespace PaymentInfo.Data.Migrations;

public class SeedData
{
    static readonly Guid CustomerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

    static readonly Guid DennisMasterCardId = Guid.Parse("be09bff7-ab1c-499d-9e40-a282de05cd88");
    static readonly Guid DennisAmexCardId = Guid.Parse("20fa31b6-2a5c-4f85-b689-38afa2fb6768");

    public static IEnumerable<CreditCard> Products()
    {
        var now = DateTime.Now;
        var expiryDateMasterCard = now.AddMonths(42);
        var expiryDateAmex = now.AddMonths(1);

        return new List<CreditCard>
        {
            new()
            {
                CustomerId = CustomerId, CreditCardId = DennisMasterCardId, CardHolder = "Dennis van der Stelt",
                CardType = "MasterCard", LastDigits = "1337", Currency = "EUR",
                ExpiryDate = $"{expiryDateMasterCard.Month}/{expiryDateMasterCard.Year}", Token = "86aa155d663e85d0",
                LastUsed = DateTime.Today.AddMinutes(1000)
            },
            new()
            {
                CustomerId = CustomerId, CreditCardId = DennisAmexCardId, CardHolder = "D.M. van der Stelt",
                CardType = "Amex", LastDigits = "4532", Currency = "USD",
                ExpiryDate = $"{expiryDateAmex.Month}/{expiryDateAmex.Year}", Token = "90d445b6e3274ebc",
                LastUsed = new DateTime(2024, 01, 17)
            },
        };
    }

    public static Order Orders()
    {
        return new Order()
        {
            OrderId = Guid.Parse("08bebbee-0e7e-4368-afab-74f4720f5f4e"),
            CreditCardId = DennisMasterCardId
        };
    }
}