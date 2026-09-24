using PlayGames.Models;

namespace PlayGames.Data;

public static class GameSeeder
{
    public static void Seed(StoreDbContext db)
    {
        if (db.Games.Any()) return;

        db.Games.AddRange(
            new Game
            {
                Name = "Grand Theft Auto V",
                Description = "Open-world action game.",
                Price = 2999,
                DiscountPrice = 1999,
                Genre = "Action",
                ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg"
            },
            new Game
            {
                Name = "Forza Horizon 5",
                Description = "Open-world racing game.",
                Price = 3999,
                DiscountPrice = 2999,
                Genre = "Racing",
                ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/header.jpg"
            }
        );

        db.SaveChanges();
    }
}
