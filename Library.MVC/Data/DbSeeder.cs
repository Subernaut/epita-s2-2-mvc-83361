using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Library.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Domain;

namespace Library.MVC.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // --- Identity roles + Admin ---
            string[] roles = { "Admin", "Inspector", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            const string adminEmail = "admin@inspections.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // --- Skip if already seeded ---
            if (await context.Premises.AnyAsync()) return;

            var rnd = new Random();

            // --- Premises: 12 across 3 towns ---
            var towns = new[] { "Springfield", "Rivertown", "Lakeside" };
            var premisesFaker = new Faker<Premise>()
                .RuleFor(p => p.Name, f => f.Company.CompanyName())
                .RuleFor(p => p.Address, f => f.Address.StreetAddress())
                .RuleFor(p => p.Town, f => f.PickRandom(towns))
                .RuleFor(p => p.RiskRating, f => f.PickRandom<RiskRating>());

            var premisesList = premisesFaker.Generate(12);
            await context.Premises.AddRangeAsync(premisesList);
            await context.SaveChangesAsync();

            // --- Inspections: 25 across premises and dates ---
            var inspections = new List<Inspection>();
            for (int i = 0; i < 25; i++)
            {
                var premises = premisesList[rnd.Next(premisesList.Count)];
                var inspectionDate = DateTime.UtcNow.AddDays(-rnd.Next(0, 60));
                inspections.Add(new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = inspectionDate,
                    Score = rnd.Next(50, 101),
                    Outcome = rnd.NextDouble() > 0.2 ? Outcome.Pass : Outcome.Fail, // ~20% fail
                    Notes = new Faker().Lorem.Sentence(5, 10)
                });
            }
            await context.Inspections.AddRangeAsync(inspections);
            await context.SaveChangesAsync();

            // --- FollowUps: 10, some overdue, some closed ---
            var followUps = new List<FollowUp>();
            for (int i = 0; i < 10; i++)
            {
                var inspection = inspections[rnd.Next(inspections.Count)];
                var dueDate = DateTime.UtcNow.AddDays(rnd.Next(-15, 15)); // some past, some future
                bool isClosed = rnd.NextDouble() > 0.5;
                followUps.Add(new FollowUp
                {
                    InspectionId = inspection.Id,
                    DueDate = dueDate,
                    Status = isClosed ? FollowUpStatus.Closed : FollowUpStatus.Open,
                    ClosedDate = isClosed ? dueDate.AddDays(rnd.Next(0, 5)) : null
                });
            }

            await context.FollowUps.AddRangeAsync(followUps);
            await context.SaveChangesAsync();
        }
    }
}