using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.DbConfig;

public class MyDbContext(DbContextOptions options) : DbContext(options)
{
}