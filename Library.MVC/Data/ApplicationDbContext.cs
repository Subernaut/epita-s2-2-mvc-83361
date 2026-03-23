using Library.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Library.MVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Premise> Premises => Set<Premise>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Inspection>()
            .HasOne(i => i.Premises)
            .WithMany(p => p.Inspections)
            .HasForeignKey(i => i.PremisesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FollowUp>()
            .HasOne(f => f.Inspection)
            .WithMany(i => i.FollowUps)
            .HasForeignKey(f => f.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Premise>()
            .Property(p => p.RiskRating)
            .HasConversion<string>();

        modelBuilder.Entity<Inspection>()
            .Property(i => i.Outcome)
            .HasConversion<string>();

        modelBuilder.Entity<FollowUp>()
            .Property(f => f.Status)
            .HasConversion<string>();
    }
}

