using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TypicalTechTools.Models.Repositories;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    /// <summary>
    /// Handles product operations such as listing, creation, editing, and viewing product details.
    /// </summary>
    /// <remarks>
    /// ADMIN users can create and edit product details.
    /// All users can view product list and product details.
    /// </remarks>
    public class ProductController : Controller
    {
        //  private readonly CsvParser _csvParser;
        private readonly IProductRepository _productRepository;
        private readonly ICommentRepository _commentRepository;

        public ProductController(IProductRepository productRepository, ICommentRepository commentRepository)
        {
            _productRepository = productRepository;
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// Displays a list of all products.
        /// </summary>
        /// <returns>Product list view.</returns>
        // GET: ProductController
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        /// <summary>
        /// Shows the product creation form.
        /// </summary>
        /// <returns>View for creating a new product.</returns>
        // POST: ProductController/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of a new product.
        /// Only accessible by ADMIN users.
        /// </summary>
        /// <param name="product">Product data from the form.</param>
        /// <returns>Redirects to product list or returns form with validation errors.</returns>
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
              return View(product);
            }

            product.UpdatedDate = DateTime.Now;
            _productRepository.CreateProduct(product);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays details for a single product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Details view for the selected product.</returns>
        // GET: ProductController/Details/5
        public IActionResult Details(int id)
        {
            var result = _productRepository.GetProductById(id);
            return View(result);
        }

        /// <summary>
        /// Shows the product edit form.
        /// </summary>
        /// <param name="id">Product ID to edit.</param>
        /// <returns>View with product details to edit.</returns>
        // GET: ProductController/Edit/5
        public IActionResult Edit(int id)
        {
            var result = _productRepository.GetProductById(id);
            return View(result);
        }

        /// <summary>
        /// Handles updates to the product's price and update date.
        /// Only accessible by ADMIN users.
        /// </summary>
        /// <param name="product">Product object with updated values.</param>
        /// <returns>Redirects to product list or returns form with errors.</returns>
        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product); // Вернёт форму с ошибками
            }

            // Получаем существующий продукт из базы
            var existingProduct = _productRepository.GetProductById(product.ProductId);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Обновляем только цену и дату
            existingProduct.ProductPrice = product.ProductPrice;
            existingProduct.UpdatedDate = DateTime.Now;

            _productRepository.UpdateProductPrice(existingProduct.ProductId, existingProduct.ProductPrice);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
