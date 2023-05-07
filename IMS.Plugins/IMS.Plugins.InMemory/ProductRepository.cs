using IMS.CoreBusiness;
using IMS.UseCases.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;
        public ProductRepository()
        {
            _products = new List<Product>()
            {
                new Product(){ ProductID = 1,ProductName = "Bike", Quantity = 10, Price = 150 },
                new Product(){ ProductID = 1,ProductName = "Car", Quantity = 5, Price = 25000 }
            };
        }
        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) return await Task.FromResult(_products);

            return _products.Where(i => i.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
        public Task AddProductAsync(Product product)
        {
            if (_products.Any(x => x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            var maxId = _products.Max(x => x.ProductID);
            product.ProductID = maxId +1;

            _products.Add(product);

            return Task.CompletedTask;
        }
    }
}
