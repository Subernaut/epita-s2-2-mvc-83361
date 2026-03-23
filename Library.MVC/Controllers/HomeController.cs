using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.MVC.Models;
using Serilog;

namespace Library.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Visited Home/Index page.");
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Visited Home/Privacy page.");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        // Log the error context if available
        if (HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>() is { } exceptionFeature)
        {
            var ex = exceptionFeature.Error;
            Log.Error(ex, "Unhandled exception occurred. RequestId: {RequestId}", requestId);
        }

        var model = new ErrorViewModel
        {
            RequestId = requestId
        };

        return View(model);
    }
}