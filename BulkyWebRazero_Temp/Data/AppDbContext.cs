using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazero_Temp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> db ):base(db)
        {
        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category {Id =10 , Name="Derama" , DisplayOrder = 10  },
                new Category {Id =20 , Name="Romatic" , DisplayOrder = 20  },
                new Category {Id =30 , Name="Action" , DisplayOrder = 30  }
                );
        }
    }
}
