using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;

namespace OmNomNom.Website.Helpers;

// Shared plumbing for the order-related emails: pull the order summary
// from the gateway, render a Razor view with it, and send via SMTP.
// Recipient address is hardcoded for now (no customer email is stored
// anywhere in the system - see OrderShippedHandler history for the
// wider gap).
public class OrderEmailSender(
    IHttpClientFactory httpClientFactory,
    IServiceProvider serviceProvider,
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider)
{
    public async Task<JObject> GetOrderSummaryAsync(Guid orderId, CancellationToken ct)
    {
        var httpClient = httpClientFactory.CreateClient("composition-gateway");
        var response = await httpClient.GetAsync($"/email/summary/{orderId}", ct);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(ct);
        return JObject.Parse(body);
    }

    public async Task SendAsync(string viewPath, string subject, JObject content, CancellationToken ct)
    {
        var htmlBody = await RenderRazorViewAsync(viewPath, content);

        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@omnomnom.dev"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mailMessage.To.Add("dennis@bloggingabout.net");

        using var client = new SmtpClient("localhost", 1025);
        await client.SendMailAsync(mailMessage, ct);
    }

    async Task<string> RenderRazorViewAsync(string viewPath, JObject content)
    {
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var view = FindView(actionContext, viewPath);

        await using var output = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<JObject>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = content
            },
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext);
        return output.ToString();
    }

    IView FindView(ActionContext actionContext, string viewName)
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
