using IMS.CoreBusiness;
using IMS.Plugins.EFCoreSqlServer;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;

        public ProductEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using var db = this.contextFactory.CreateDbContext();

            return await db.Products.Where(x=>x.ProductName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();

            /*if (string.IsNullOrEmpty(name)) return await Task.FromResult(_products);

            return _products.Where(i => i.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));*/
        }
        public async Task AddProductAsync(Product product)
        {
            using var db = this.contextFactory.CreateDbContext();

            db.Products.Add(product);
            FlagInventoryUnchanged(product, db);

            await db.SaveChangesAsync();
            /*if (_products.Any(x => x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            var maxId = _products.Max(x => x.ProductID);
            product.ProductID = maxId +1;

            _products.Add(product);

            return Task.CompletedTask;*/
        }

        private void FlagInventoryUnchanged(Product product, IMSContext db)
        {
            if (product?.ProductInventories != null && product.ProductInventories.Count > 0)
            {
                foreach (var prodInv in product.ProductInventories)
                {
                    if (prodInv.Inventory != null)
                    {
                        db.Entry(prodInv.Inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            using var db = this.contextFactory.CreateDbContext();

            return await db.Products.Include(x=>x.ProductInventories).ThenInclude(x=>x.Inventory).FirstOrDefaultAsync(i=>i.ProductID==productId);

            /*var prod = _products.FirstOrDefault(i => i.ProductID == productId);
            var newProd = new Product();
            if(prod != null)
            {
                newProd.ProductID = prod.ProductID;
                newProd.ProductName = prod.ProductName;
                newProd.Price = prod.Price;
                newProd.Quantity = prod.Quantity;
                newProd.ProductInventories = new List<ProductInventory>();
                if(prod.ProductInventories != null && prod.ProductInventories.Count > 0)
                {
                    foreach(var prodInv in prod.ProductInventories)
                    {
                        var newProdInv = new ProductInventory
                        {
                            inventoryId = prodInv.inventoryId,
                            ProductId = prodInv.ProductId,
                            Product = prod,
                            Inventory = new Inventory(),
                            InventoryQuantity = prodInv.InventoryQuantity
                        };
                        if (prodInv.Inventory != null)
                        {
                            newProdInv.Inventory.InventoryID = prodInv.Inventory.InventoryID;
                            newProdInv.Inventory.InventoryName = prodInv.Inventory.InventoryName;
                            newProdInv.Inventory.Price = prodInv.Inventory.Price;
                            newProdInv.Inventory.Quantity = prodInv.Inventory.Quantity;
                        }

                        newProd.ProductInventories.Add(newProdInv);
                    }
                }
            }

            return await Task.FromResult(newProd);*/
        }

        public async Task UpdateProductAsync(Product product)
        {
            using var db = this.contextFactory.CreateDbContext();

            var prod = await db.Products.Include(i => i.ProductInventories)
                .FirstOrDefaultAsync(i => i.ProductID == product.ProductID);

            if(prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;

                FlagInventoryUnchanged(prod, db);

                await db.SaveChangesAsync();
            }

            /*if (_products.Any(i => i.ProductID != product.ProductID && i.ProductName.ToLower() == product.ProductName.ToLower())) return Task.CompletedTask;

            var prod = _products.FirstOrDefault(i => i.ProductID == product.ProductID);
            if (prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;
            }

            return Task.CompletedTask;*/
        }
    }
}
