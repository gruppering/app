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
}