using TypicalTechTools.Models;
using TypicalTechTools.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace TypicalTechTools.Models.Repositories
{
    /// <summary>
    /// Provides methods for interacting with product data in the database.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly TypicalTechToolsDBContext _context;

        public ProductRepository(TypicalTechToolsDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all products from the database.
        /// </summary>
        /// <returns>List of products</returns>
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        /// <summary>
        /// Gets a single product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product object or null</returns>
        public Product GetProductById(int id)
        {
            var product = _context.Products.Where(a => a.ProductId == id).FirstOrDefault();
            return product;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">Product to add</param>
        public void CreateProduct(Product product)
        {
            product.UpdatedDate = DateTime.Now;
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the price of an existing product by ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="price">New price</param>
        public void UpdateProductPrice(int id, double price)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                product.ProductPrice = price;
                product.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}