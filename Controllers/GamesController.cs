using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayGames.Data;
using PlayGames.Models;

namespace PlayGames.Controllers;

public class GamesController : Controller
{
    private readonly StoreDbContext _db;

    public GamesController(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int? install = null)
    {
        var model = new GameStoreViewModel
        {
            Games = await _db.Games
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(),

            AutoInstallId = install
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

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int gameId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage(
                "/Account/Login",
                new
                {
                    area = "Identity",
                    returnUrl = "/Games"
                });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var game = await _db.Games
            .FirstOrDefaultAsync(x => x.Id == gameId && x.IsActive);

        if (game == null)
            return NotFound();

        var exists = await _db.LibraryItems
            .AnyAsync(x => x.UserId == userId && x.GameId == gameId);

        if (!exists)
        {
            _db.LibraryItems.Add(new LibraryItem
            {
                UserId = userId,
                GameId = gameId,
                AddedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
