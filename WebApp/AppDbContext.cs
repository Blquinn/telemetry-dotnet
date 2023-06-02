using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using WebApp.Orders.Models;

namespace WebApp;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=test.db");
    }
}
