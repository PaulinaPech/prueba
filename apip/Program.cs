using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductoDb>(opt => opt.UseInMemoryDatabase("ProductoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/productos", async (ProductoDb db) =>
    await db.Productos.ToListAsync());

app.MapGet("/productos/{id}", async (int id, ProductoDb db) =>
    await db.Productos.FindAsync(id)
        is Producto producto
            ? Results.Ok(producto)
            : Results.NotFound());

app.MapPost("/productos", async (Producto producto, ProductoDb db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();

    return Results.Created($"/productos/{producto.Id}", producto);
});


app.Run();
