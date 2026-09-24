using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlayGames.Controllers;

[Authorize]
public class SettingsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
