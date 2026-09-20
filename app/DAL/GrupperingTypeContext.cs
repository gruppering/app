using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.DAL;

public class GrupperingTypeContext(DbContextOptions<GrupperingTypeContext> contextOptions): DbContext(contextOptions)
{ 
    public DbSet<GrupperingUser> Users { get; set; } = null!;
}