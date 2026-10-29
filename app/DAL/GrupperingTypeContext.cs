using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.DAL;

public class GrupperingTypeContext: DbContext
{
    public GrupperingTypeContext(DbContextOptions<GrupperingTypeContext> contextOptions) : base(contextOptions)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public DbSet<GrupperingUser> Users { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // We have to manually specify the many-to-many relationship so we don't have to add unnecessary info to the user entity.
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Members)
            .WithMany();
        modelBuilder.Entity<Course>().HasIndex(c => c.JoinCode).IsUnique();
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Members)
            .WithMany();
        modelBuilder.Entity<GrupperingUser>().HasOne(gu => gu.ExternalAuth).WithOne()
            .HasForeignKey<ExternalAuth>(ea => ea.UserId);
        modelBuilder.Entity<ChatMessage>().HasIndex(cm => cm.Channel);
    }
}