using Microsoft.EntityFrameworkCore;
using PlayGames.Models;

namespace PlayGames.Data;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameKey> GameKeys => Set<GameKey>();
}