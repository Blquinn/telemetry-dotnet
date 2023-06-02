using Microsoft.EntityFrameworkCore;
using Worker.Orders.Models;

namespace WebApp;

public class WorkerDbContext : DbContext
{
    public DbSet<OrderState> OrderStates { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=worker.db");
    }
}
