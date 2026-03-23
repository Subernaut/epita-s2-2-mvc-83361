using Library.Domain.Enrichers;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// --- Build WebApplication builder ---
var builder = WebApplication.CreateBuilder(args);

// --- Add HttpContextAccessor for UserName enrichment ---
builder.Services.AddHttpContextAccessor();

// --- Configure Serilog ---
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FoodInspectionsApp")
    .Enrich.WithEnvironmentName() // no parameter needed
    .Enrich.With(new UserNameEnricher(builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()))
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(); // replace default logger

// --- Add services ---
builder.Services.AddControllersWithViews();

// Add EF DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// --- Global exception handling + friendly error page ---
if (!app.Environment.IsDevelopment())
{
    // In production, redirect to Error page
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // In development, show detailed exception page
    app.UseDeveloperExceptionPage();
}

// --- Apply migrations and seed data ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbSeeder.SeedAsync(context, userManager, roleManager);

        Log.Information("Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

// --- Configure HTTP request pipeline ---
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor pages for Identity
app.MapRazorPages();

// --- Log startup ---
Log.Information("Application starting up.");

// --- Run app ---
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}