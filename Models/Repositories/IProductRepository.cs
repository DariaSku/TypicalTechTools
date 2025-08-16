namespace TypicalTechTools.Models.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        void CreateProduct(Product product);
        void UpdateProductPrice(int id, double price);
    }
}
