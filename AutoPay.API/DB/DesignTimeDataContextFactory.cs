using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoPay.API.DB;

public class DesignTimeDataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<DataContext> builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseSqlite("DataSource=data.db");
        return new DataContext(builder.Options);
    }
}