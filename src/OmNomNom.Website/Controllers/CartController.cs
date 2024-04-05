using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.Controllers;

public class CartController : Controller
{
    [HttpGet("/cart/{orderId}")]
    public IActionResult Index(string orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }
}