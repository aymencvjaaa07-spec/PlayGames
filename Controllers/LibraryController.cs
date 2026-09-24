using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayGames.Data;

namespace PlayGames.Controllers;

[Authorize]
public class LibraryController : Controller
{
    private readonly StoreDbContext _db;

    public LibraryController(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var games = await (
            from item in _db.LibraryItems
            join game in _db.Games on item.GameId equals game.Id
            where item.UserId == userId && game.IsActive
            orderby item.AddedAt descending
            select game
        ).ToListAsync();

        return View(games);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var game = await _db.Games
            .FirstOrDefaultAsync(x => x.Id == gameId && x.IsActive);

        if (game == null)
            return NotFound();

        var exists = await _db.LibraryItems.AnyAsync(
            x => x.UserId == userId && x.GameId == gameId);

        if (!exists)
        {
            _db.LibraryItems.Add(new PlayGames.Models.LibraryItem
            {
                UserId = userId,
                GameId = gameId
            });

            await _db.SaveChangesAsync();
        }

        if (game.Price <= 0 &&
            !string.IsNullOrWhiteSpace(game.DownloadUrl))
        {
            return RedirectToAction(
                "Index",
                "Games",
                new { install = game.Id });
        }

        return RedirectToAction(nameof(Index));
    }
}
