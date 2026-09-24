using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayGames.Data;

namespace PlayGames.Controllers;

public class GamesController : Controller
{
    private readonly StoreDbContext _db;

    public GamesController(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var games = await _db.Games
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(games);
    }
}