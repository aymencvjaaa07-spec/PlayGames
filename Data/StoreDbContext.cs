using Microsoft.EntityFrameworkCore;
using PlayGames.Models;

namespace PlayGames.Data;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();
    public DbSet<GameKey> GameKeys => Set<GameKey>();
    public DbSet<SavedGame> SavedGames => Set<SavedGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SavedGame>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();
    }
}
