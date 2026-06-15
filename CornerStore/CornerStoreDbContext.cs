using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }



    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {
    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cashier>().HasData(new Cashier[]
        {
            new Cashier { Id = 1, FirstName = "Ken", LastName = "Jones" },
            new Cashier { Id = 2, FirstName = "Barb", LastName = "Lovell"}
        });

        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category { Id = 1, CategoryName = "Chips"},
            new Category { Id = 2, CategoryName = "Medicine"},
            new Category { Id = 3, CategoryName = "Drinks"},
            new Category { Id = 4, CategoryName = "Specialty"}
        });

        modelBuilder.Entity<Order>().HasData(new Order[]
        {
            new Order {Id = 1, CashierId = 1 },
            new Order {Id = 2, CashierId = 2 },
            new Order {Id = 3, CashierId = 1 },
            new Order {Id = 4, CashierId = 2 }
        });

        modelBuilder.Entity<Product>().HasData(new Product[]
        {
            new Product {Id = 1, ProductName = "Ragin' Cajun Potato Skins", Brand= "Lays", Price = 6.99M, CategoryId = 1},
            new Product {Id = 2, ProductName = "Tylenol Headache pills", Brand = "Tylenol",Price = 16.99M, CategoryId = 2},
            new Product {Id = 3, ProductName = "Dr. Bepis", Brand = "Pepis", Price = 1.99M, CategoryId = 3},
            new Product {Id = 4, ProductName = "Vegan Hot Pocket", Brand = "Amy's", Price = 26.99M, CategoryId = 4},
        });

    
    modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

    modelBuilder.Entity<OrderProduct>().HasData(new OrderProduct[]
{
    new OrderProduct {ProductId = 1, OrderId = 1, Quantity = 10 },
    new OrderProduct {ProductId = 3, OrderId = 2, Quantity = 5 }, 
});

    }
}