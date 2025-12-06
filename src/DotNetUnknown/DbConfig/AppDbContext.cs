using DotNetUnknown.Transaction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.DbConfig;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<MasterAccount> MasterAccount { get; set; } = null!;
    public DbSet<SubAccount> SubAccount { get; set; } = null!;
    public DbSet<AuditTrail> AuditTrail { get; set; } = null!;
}