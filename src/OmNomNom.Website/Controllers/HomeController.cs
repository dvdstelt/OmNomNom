using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}