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

        var gameIds = await _db.LibraryItems
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .Select(x => x.GameId)
            .ToListAsync();

        var games = await _db.Games
            .Where(x => gameIds.Contains(x.Id) && x.IsActive)
            .ToListAsync();

        games = gameIds
            .Join(
                games,
                id => id,
                game => game.Id,
                (_, game) => game)
            .ToList();

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

        var exists = await _db.LibraryItems
            .AnyAsync(x => x.UserId == userId && x.GameId == gameId);

        if (!exists)
        {
            _db.LibraryItems.Add(new PlayGames.Models.LibraryItem
            {
                UserId = userId,
                GameId = gameId,
                AddedAt = DateTime.UtcNow
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
