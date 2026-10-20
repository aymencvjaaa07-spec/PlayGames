using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlayGames.Data;
using PlayGames.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseConfigured = !string.IsNullOrWhiteSpace(connectionString);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

if (databaseConfigured)
{
    builder.Services.AddDbContext<StoreDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<ApplicationDbContext>().AddRoles<IdentityRole>();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/Login";
    });
}

var app = builder.Build();
if (databaseConfigured)
{
    using var scope = app.Services.CreateScope();
    var storeDb = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    storeDb.Database.Migrate();
    identityDb.Database.Migrate();
    GameSeeder.Seed(storeDb);
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
if (databaseConfigured)
{
    app.UseAuthentication();
    app.UseAuthorization();
}
app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
if (databaseConfigured) app.MapRazorPages();
app.Run();
