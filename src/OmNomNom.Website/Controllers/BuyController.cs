using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.Controllers;

public class BuyController : Controller
{
    [HttpGet("/buy/address/{orderId}")]
    public IActionResult Address(string orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }

    [HttpGet("/buy/shipping/{orderId}")]
    public IActionResult Shipping(string orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }

    [HttpGet("/buy/payment/{orderId}")]
    public IActionResult Payment(string orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }

    [HttpGet("/buy/summary/{orderId}")]
    public IActionResult Summary(string orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }
}