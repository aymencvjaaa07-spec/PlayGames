using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayGames.Data;
using PlayGames.Models;

namespace PlayGames.Controllers;

public class HomeController : Controller
{
    private readonly StoreDbContext _db;

    public HomeController(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = new GameStoreViewModel
        {
            Games = await _db.Games
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync()
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                model.OwnedGameIds = (
                    await _db.LibraryItems
                        .Where(x => x.UserId == userId)
                        .Select(x => x.GameId)
                        .ToListAsync()
                ).ToHashSet();
            }
        }

        return View("~/Views/Games/Index.cshtml", model);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
