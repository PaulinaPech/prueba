using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductoDb>(opt => opt.UseInMemoryDatabase("ProductoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/productos", async (HttpContext context) =>
{
  int pagina = Convert.ToInt32(context.Request.Query["pagina"]);
  int productos = 3; // Cantidad de productos por pagina

  // Obtener lista completa de productos de la memoria
  List<Producto> productos = await db.Productos.ToListAsync();

  // Calcular información de paginación
 int totalProductos = productos.Count;
 int paginasTotales = (int)Math.Ceiling((double)totalProductos / tamanoPagina);
 int indiceInicial = (pagina - 1) * tamanoPagina;
 int indiceFinal = Math.Min(indiceInicial + tamanoPagina, totalProductos);

 // Obtener lista de productos paginada
 List<Producto> productosPaginados = productos.GetRange(indiceInicial, indiceFinal - indiceInicial);

 // Crear instancia de ProductosPaginasResult y devolverla como resultado
 return new ProductosPaginasResult<Producto>(productosPaginados, pagina, paginasTotales);

});

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

app.MapPut("/productos/{id}", async (int id, Producto inputProducto, ProductoDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = inputProducto.Nombre;
    producto.Precio = inputProducto.Precio;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/productos/{id}", async (int id, ProductoDb db) => {
    if (await db.Productos.FindAsync(id) is Producto producto)
    {
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return Results.Ok(producto);
    }
    return Results.NotFound();
});

app.Run();