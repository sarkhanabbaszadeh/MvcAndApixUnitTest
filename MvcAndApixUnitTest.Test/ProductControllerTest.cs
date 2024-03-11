using Microsoft.AspNetCore.Mvc;
using Moq;
using MvcAndApixUnitTest.Web.Controllers;
using MvcAndApixUnitTest.Web.Models;
using MvcAndApixUnitTest.Web.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAndApixUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _repositoryMock;
        private readonly ProductsController _productsController;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _repositoryMock = new Mock<IRepository<Product>>();
            _productsController = new ProductsController(_repositoryMock.Object);

            _products = new List<Product>() { 
                new Product { Id = 1, Name = "Book", Price = 12, Stock = 48, Color = "Red" }, 
                new Product { Id = 2, Name = "Pen", Price = 3, Stock = 72, Color = "White" } };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productsController.Index();

            Assert.IsType<ViewResult>(result);
        } 

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _productsController.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Details(null);

            var redirect= Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _repositoryMock.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _productsController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = _products.First(x=>x.Id == productId);

            _repositoryMock.Setup(repo=>repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);

        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productsController.Create();

            Assert.IsType<ViewResult>(result);
        }
    }
}
