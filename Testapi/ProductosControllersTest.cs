using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Testapi{

public class ProductosControllersTeest
{
        private readonly ProductoDb _db;
        private readonly ProductosController _controller;

    public ProductosControllersTeest()
        {
            var options = new DbContextOptionsBuilder<ProductoDb>()
                .UseInMemoryDatabase("ProductoList")
                .Options;
            _db = new ProductoDb(options);
            _controller = new ProductosController(_db);
        }
        
        [Fact]
        public async Task GetProductos_ok()
        {
            var result = await _controller.GetProductos();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
       public async Task GetProductos_ReturnsOkResult()
       {
          var options = new DbContextOptionsBuilder<ProductoDb>()
          .UseInMemoryDatabase("ProductoList")
          .Options;

            using (var context = new ProductoDb(options))
            {
            //Esto sirve para limpiar la base de datos y no generar un error con Id duplicado
              await context.Database.EnsureDeletedAsync();
              await context.Database.EnsureCreatedAsync();
              
              context.Productos.AddRange(
               new Producto { Id = 1, Nombre = "Chips Moradas", Precio = 17.0 },
               new Producto { Id = 2, Nombre = "Chips Verdes", Precio = 17.0 },
               new Producto { Id = 3, Nombre = "Kinder Chocolate", Precio = 8 }
               );
                await context.SaveChangesAsync();
            }

            using (var context = new ProductoDb(options))
            {
               var controller = new ProductosController(context);

               var result = await controller.GetProductos();

               var okResult = Assert.IsType<OkObjectResult>(result);
               var items = Assert.IsAssignableFrom<IEnumerable<Producto>>(okResult.Value);

               Assert.Equal(3, items.Count());
            }
        }

         [Fact]
        public async Task GetProducto_Ok()
        {
           var options = new DbContextOptionsBuilder<ProductoDb>()
           .UseInMemoryDatabase("ProductoList")
           .Options;

           using (var context = new ProductoDb(options))
           {
            var producto = new Producto { Id = 4, Nombre = "Carlos V", Precio = 10.0 };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var controller = new ProductosController(context);

            var result = await controller.GetProducto(4);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultProducto = Assert.IsType<Producto>(okResult.Value);

             Assert.Equal(producto.Id, resultProducto.Id);
             Assert.Equal(producto.Nombre, resultProducto.Nombre);
             Assert.Equal(producto.Precio, resultProducto.Precio);
            }
        }

         [Fact]
         public async Task GetProducto_ReturnsNotFound()
         {

         var result = await _controller.GetProducto(100); // Id inexistente

         Assert.IsType<NotFoundResult>(result);
        }

       [Fact]
        public async Task CreateProducto_ReturnsCreatedResult()
        {
          var options = new DbContextOptionsBuilder<ProductoDb>()
          .UseInMemoryDatabase("ProductoList")
          .Options;
           using (var context = new ProductoDb(options))
           {
             var producto = new Producto { Nombre = "Ruffles Verdes", Precio = 17.0 };
             var result = await _controller.CreateProducto(producto);

             var createdResult = Assert.IsType<CreatedAtActionResult>(result);
             Assert.IsType<Producto>(createdResult.Value);
           }
        }

        [Fact]
        public async Task UpdateProducto_ReturnsNoContent()
        {
         var options = new DbContextOptionsBuilder<ProductoDb>()
         .UseInMemoryDatabase("ProductoList")
         .Options;

          using (var context = new ProductoDb(options))
          {
           var producto = new Producto { Id = 4, Nombre = "Carlos V", Precio = 10.0 };
           context.Productos.Add(producto);
           await context.SaveChangesAsync();
          }

         using (var context = new ProductoDb(options))
           {
           var controller = new ProductosController(context);
           var updatedProducto = new Producto { Id = 4, Nombre = "Chips Verdes", Precio = 20.0 };
           var result = await controller.UpdateProducto(4, updatedProducto);
        
           Assert.IsType<NoContentResult>(result);

           //En esta parte verifica que el producto se haya actualizado
           var producto = await context.Productos.FindAsync(4);
           Assert.Equal(updatedProducto.Nombre, producto.Nombre);
           Assert.Equal(updatedProducto.Precio, producto.Precio);
          }
        }
        
        [Fact]
        public async Task DeleteProducto_ReturnsOkResult()
        {
            var options = new DbContextOptionsBuilder<ProductoDb>()
                .UseInMemoryDatabase("ProductoList")
                .Options;

            using (var context = new ProductoDb(options))
            {
                var producto = new Producto { Id = 5, Nombre = "Producto de prueba", Precio = 10.0 };
                context.Productos.Add(producto);
                await context.SaveChangesAsync();

                var controller = new ProductosController(context);

                var result = await controller.DeleteProducto(5);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var resultProducto = Assert.IsType<Producto>(okResult.Value);

                Assert.Equal(producto.Id, resultProducto.Id);
                Assert.Equal(producto.Nombre, resultProducto.Nombre);
                Assert.Equal(producto.Precio, resultProducto.Precio);

                //En esta parte Comprueba que el producto fue eliminado de forma exitosa 
                var deletedProducto = await context.Productos.FindAsync(5);
                Assert.Null(deletedProducto);
            }
        }

    }
}