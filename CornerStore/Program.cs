using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endpoints go here

app.MapPost("/api/cashiers",(CornerStoreDbContext db, Cashier cashier) =>
{
     db.Cashiers.Add(cashier);
     db.SaveChanges();
     return Results.Created($"/api/cashiers/{cashier.Id}", cashier);
});

app.MapGet("/api/cashiers/{id}", (CornerStoreDbContext db, int id) =>
{
    return db.Cashiers
        .Include(c => c.Orders)
        .ThenInclude(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .SingleOrDefault(c => c.Id == id);
});

app.MapGet("/api/products", (CornerStoreDbContext db, string? search) =>
{
    return db.Products
        .Where(p => search == null || 
            p.ProductName.ToLower().Contains(search.ToLower()) || 
            p.Category.CategoryName.ToLower().Contains(search.ToLower()))
        .Include(p => p.Category)
        .ToList();
});

app.MapPost("/api/products",(CornerStoreDbContext db, Product product) =>
{
     db.Products.Add(product);
     db.SaveChanges();
     return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPut("/api/products/{id}", (CornerStoreDbContext db, int id, Product product) =>
{
    Product productToUpdate = db.Products.SingleOrDefault(p => p.Id == id);
    if (productToUpdate == null)
    {
        return Results.NotFound();
    }
    productToUpdate.ProductName = product.ProductName;
    productToUpdate.Brand = product.Brand;
    productToUpdate.Price = product.Price;
    productToUpdate.CategoryId = product.CategoryId;
    db.SaveChanges();
    return Results.NoContent();

});

app.MapGet("/api/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    return db.Orders
        .Include(o => o.Cashier)
        .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .ThenInclude(p => p.Category)
        .SingleOrDefault(o => o.Id == id);
});


app.MapGet("/api/orders", (CornerStoreDbContext db, string? orderDate) =>
{
    return db.Orders
    .Where(o => orderDate == null || o.PaidOnDate == DateTime.Parse(orderDate).Date)
    .ToList();
});


app.MapDelete("/api/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    Order order = db.Orders.SingleOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound();
    }
    db.Remove(order);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/orders/", (CornerStoreDbContext db, Order newOrder) =>
{
    try
    {
        db.Orders.Add(newOrder);
        db.SaveChanges();
        return Results.Created($"/api/orders/{newOrder.Id}", newOrder);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
});



app.Run();

//don't move or change this!
public partial class Program { }