using IMS.CoreBusiness;

namespace IMS.UseCases.Products
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
    }
}