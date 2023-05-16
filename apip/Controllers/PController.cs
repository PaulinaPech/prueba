using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductosController : ControllerBase
{
    private readonly ProductoDb _db;

    public ProductosController(ProductoDb db)
    {
        _db = db;
    }

    [HttpGet("/productos")]
    public async Task<IActionResult> GetProductos(int page = 1, int pageSize = 5)
    {
    var skip = (page - 1) * pageSize;
    var take = pageSize;

    var items = await _db.Productos.Skip(skip).Take(take).ToListAsync();

    return Ok(items);
    }


    [HttpGet("/productos/{id}")]
    public async Task<IActionResult> GetProducto(int id)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto is null)
            return NotFound();

        return Ok(producto);
    }

    [HttpPost("/productos")]
    public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
    {
    _db.Productos.Add(producto);
    await _db.SaveChangesAsync();

    return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
    }
    /*
    Correción con base a la prueba unitaria
    [HttpPost("/productos")]
    public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
    {
        _db.Productos.Add(producto);
        await _db.SaveChangesAsync();

        return Created($"/productos/{producto.Id}", producto);
    }
    */

    [HttpPut("/productos/{id}")]
    public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto inputProducto)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto is null)
            return NotFound();

        producto.Nombre = inputProducto.Nombre;
        producto.Precio = inputProducto.Precio;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("/productos/{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto is null)
            return NotFound();

        _db.Productos.Remove(producto);
        await _db.SaveChangesAsync();

        return Ok(producto);
    }
}