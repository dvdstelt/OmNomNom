using System.Net.Mail;
using ITOps.Shared.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;
using Shipping.Endpoint.Messages.Events;

namespace OmNomNom.Website.Handlers;

public class OrderShippedHandler(
    IServiceProvider serviceProvider,
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider)
    : IHandleMessages<OrderShipped>
{
    public async Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        var requestUri = $"https://localhost:7126/email/summary/{message.OrderId}";

        var httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(requestUri, context.CancellationToken);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync(context.CancellationToken);
        var content = JObject.Parse(body);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var view = FindView(actionContext, "/Views/Emails/EmailContent.cshtml");

        await using var output = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<JObject>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = content // new CustomerModel() { Fullname = "Dennis van der Stelt"}
            },
            new TempDataDictionary(
                actionContext.HttpContext,
                tempDataProvider),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext);

        var htmlBody = output.ToString();

        var mailMessage = new MailMessage();
        mailMessage.To.Add("dennis@bloggingabout.net");
        mailMessage.From = new MailAddress("noreply@omnomnom.dev");
        mailMessage.Body = htmlBody;
        mailMessage.IsBodyHtml = true;
        mailMessage.Subject = "Thanks for ordering with OmNomNom";

        var client = new SmtpClient("localhost", 25);
        await client.SendMailAsync(mailMessage, context.CancellationToken);
    }

    private IView FindView(ActionContext actionContext, string viewName)
    {
        var getViewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success)
        {
            return getViewResult.View;
        }

        var findViewResult = viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success)
        {
            return findViewResult.View;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

        throw new InvalidOperationException(errorMessage);
    }
}

public class CustomerModel
{
    public string Fullname { get; set; }
}