using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using MvcAndApixUnitTest.Web.Controllers;
using MvcAndApixUnitTest.Web.Helpers;
using MvcAndApixUnitTest.Web.Models;
using MvcAndApixUnitTest.Web.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MvcAndApixUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly Helper _helper;

        private List<Product> _products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _helper = new Helper();
            _products = new List<Product>() { new Product { Id = 1, Name = "Kitab", Price = 25, Stock = 72, Color = "Black" },
            new Product { Id = 2, Name = "Pen", Price = 5, Stock = 120, Color = "red" }};

        }

        [Theory,InlineData(4,5,9)]
        public void Add_SimpleValues_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.add(a, b);

            Assert.Equal(total, result);
        }

        [Fact]
        public async void GetProducts_ActionExecutes_RetrunOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);

            var result = await _controller.GetProducts();

            var okResult= Assert.IsType<OkObjectResult>(result);

            var returnProducts=Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2,returnProducts.ToList().Count);

        }

        [Theory,InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory,InlineData(1),InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = _products.First(x=>x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);

        }

        [Theory,InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            var result = _controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory,InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNotContent(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(productId, product);

            _mockRepo.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAtAction()
        {
            var product = _products.First();

            _mockRepo.Setup(x=>x.Create(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x => x.Create(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        [Theory,InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);

            var resultNotFound = await _controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory,InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNotContent(int productId)
        {
            var product = _products.First(x=>x.Id == productId);

            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.Delete(product));

            var noContentResult = await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x=>x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
    }
}
