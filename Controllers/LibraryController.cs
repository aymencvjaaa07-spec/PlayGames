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

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Download(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var owned = await _db.LibraryItems
            .AnyAsync(x => x.UserId == userId && x.GameId == gameId);

        if (!owned)
            return Forbid();

        var game = await _db.Games
            .FirstOrDefaultAsync(x => x.Id == gameId && x.IsActive);

        if (game == null || string.IsNullOrWhiteSpace(game.DownloadUrl))
            return NotFound();

        using var client = new HttpClient();

        var response = await client.GetAsync(
            game.DownloadUrl,
            HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync();

        var contentType =
            response.Content.Headers.ContentType?.ToString()
            ?? "application/octet-stream";

        var fileName =
            Path.GetFileName(new Uri(game.DownloadUrl).AbsolutePath);

        if (string.IsNullOrWhiteSpace(fileName))
            fileName = $"{game.Name}.zip";

        return File(stream, contentType, fileName);
    }
}
