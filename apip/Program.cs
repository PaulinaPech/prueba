using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductoDb>(opt => opt.UseInMemoryDatabase("ProductoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddScoped<ProductosController>();

builder.Services.AddCors(options => {
    //* Se configura una politica
    options.AddPolicy("Nueva Politica", app => {
        //* Permite agregar todo los origenes, cabezera y methodos(GET, POST, PUT Y DELETE)
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
//Con la siguiente url pueden acceder a la pagina, por pagina se obtiene 5 productos http://localhost:DEPENDIENDO SU PUERTO/productos?page=2&pageSize=5 By Stephanie uwu


//* Permite la habilitacion de Cors
app.UseCors("Nueva Politica");

app.Run();