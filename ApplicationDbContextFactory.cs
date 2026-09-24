using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using PlayGames.Data;

namespace PlayGames
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var password = Environment.GetEnvironmentVariable("PLAYGAMES_DB_PASSWORD");

            // EF migrations do not need to connect to PostgreSQL.
            // A placeholder is used only when generating the model/migration.
            if (string.IsNullOrWhiteSpace(password))
            {
                password = "design-time-only";
            }

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = "127.0.0.1",
                Port = 5432,
                Database = "playgames",
                Username = "postgres",
                Password = password
            }.ConnectionString;

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
