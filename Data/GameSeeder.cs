using PlayGames.Models;

namespace PlayGames.Data;

public static class GameSeeder
{
    public static void Seed(StoreDbContext db)
    {
        EnsureGame(db, new Game
        {
            Name = "Grand Theft Auto V",
            Description = "Open-world action game.",
            Price = 2999,
            DiscountPrice = 1999,
            Genre = "Action",
            ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg"
        });

        EnsureGame(db, new Game
        {
            Name = "Forza Horizon 5",
            Description = "Open-world racing game.",
            Price = 3999,
            DiscountPrice = 2999,
            Genre = "Racing",
            ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/header.jpg"
        });

        EnsureGame(db, new Game
        {
            Name = "Xonotic",
            Description = "Free online arena first-person shooter.",
            Price = 0,
            DiscountPrice = 0,
            Genre = "FPS",
            ImageUrl = "https://xonotic.org/static/img/xonotic_logo_web.svg",
            DownloadUrl = "https://dl.xonotic.org/xonotic-0.8.6.zip",
            DownloadSha512 = "cb39879e96f19abb2877588c2d50c5d3e64dd68153bec3dd1bebedf4d765e506afa419c28381d7005aed664cb1a042571c132b5b319e4308cab67745d996c2a6",
            InstallFolder = "Xonotic",
            ExecutablePath = "xonotic.exe"
        });

        db.SaveChanges();
    }

    private static void EnsureGame(StoreDbContext db, Game wanted)
    {
        var existing = db.Games.FirstOrDefault(x => x.Name == wanted.Name);

        if (existing == null)
        {
            db.Games.Add(wanted);
            return;
        }

        existing.Description = wanted.Description;
        existing.Price = wanted.Price;
        existing.DiscountPrice = wanted.DiscountPrice;
        existing.Genre = wanted.Genre;
        existing.ImageUrl = wanted.ImageUrl;
        existing.DownloadUrl = wanted.DownloadUrl;
        existing.DownloadSha512 = wanted.DownloadSha512;
        existing.InstallFolder = wanted.InstallFolder;
        existing.ExecutablePath = wanted.ExecutablePath;
        existing.IsActive = wanted.IsActive;
    }
}
