using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Address : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        vm.Whatever = "Dude";

        return Task.CompletedTask;
    }
}