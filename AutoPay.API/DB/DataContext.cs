using AutoPay.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPay.API.DB;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<AccessRight> AccessRights { get; set; }
    public DbSet<SettlementCheck> SettlementChecks { get; set; }
    public DbSet<Image> Images { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
       
    }
}