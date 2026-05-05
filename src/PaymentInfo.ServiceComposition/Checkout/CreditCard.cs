using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class CreditCard(PaymentInfoDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/creditcard/{customerId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var customerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

        var creditCards = await dbContext.CreditCards
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.LastUsed)
            .ToListAsync(request.HttpContext.RequestAborted);

        var creditCardsViewModel = new List<dynamic>();
        foreach (var creditCard in creditCards)
        {
            dynamic vmItem = new ExpandoObject();
            vmItem.CardId = creditCard.CreditCardId;
            vmItem.CardHolder = creditCard.CardHolder;
            vmItem.CardType = creditCard.CardType;
            vmItem.LastDigits = creditCard.LastDigits;
            vmItem.ExpiryDate = creditCard.ExpiryDate;
            vmItem.Currency = creditCard.Currency;
            vmItem.LastUsed = creditCard.LastUsed;
            creditCardsViewModel.Add(vmItem);
        }

        vm.CreditCards = creditCardsViewModel;
    }
}
