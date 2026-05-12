using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

[CompositionHandler]
public class CreditCardComposer(PaymentInfoDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/buy/creditcard/{customerId}")]
    public async Task Handle(Guid customerId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        // TODO: route binds the customerId, but the real lookup is still
        // hardcoded until authentication is wired in.
        customerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

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
