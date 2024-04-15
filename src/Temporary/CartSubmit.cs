using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class CartSubmit : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var routeData = request.HttpContext.GetRouteData();
        var orderId = Guid.Parse(routeData.Values["orderId"] as string ?? throw new InvalidOperationException("OrderId can't be empty"));

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true );
        var body = await reader.ReadToEndAsync();
        var content = JObject.Parse(body);

        //use the content object instance as needed
    }
}