using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentInfo.Data;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class CreditCard(PaymentInfoDbContext dbContext) : ICompositionRequestsHandler
{
    readonly PaymentInfoDbContext dbContext = dbContext;

    [HttpGet("/buy/creditcard/{customerId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var customerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

        var creditCardCollection = dbContext.Database.GetCollection<Data.Models.CreditCard>();
        var creditCards = creditCardCollection.Query().Where(s => s.CustomerId == customerId).ToList();

        var creditCardsViewModel = new List<dynamic>();

        foreach (var creditCard in creditCards.OrderByDescending(s => s.LastUsed))
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

        return Task.CompletedTask;
    }
}