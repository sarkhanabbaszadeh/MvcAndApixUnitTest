using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcAndApixUnitTest.Web.Controllers;
using MvcAndApixUnitTest.Web.Models;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MvcAndApixUnitTest.Test
{
    public class ProductControllerTestWithSqlServerLocalDB : ProductControllerTest
    {
        public ProductControllerTestWithSqlServerLocalDB()
        {
            var sqlConnect = @"Server=(localdb)\MSSQLLocalDB;Database=TestDB,Trusted_Connection=true,MultipleActiveResultSets=true";

           SetContextOptions(new DbContextOptionsBuilder<XUnitTestDbContext>().UseSqlServer(sqlConnect).Options);
        }

        [Fact]
        public async Task Create_ModelValidProduct_RetrunRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product { Name = "Pen 3", Price = 67, Stock = 102, Color = "Yellow" };

            using (var context = new XUnitTestDbContext(_contextOptions))
            {
                var category = context.Category.First();
                newProduct.CategoryID = category.ID;

                //var repository = new Repository<Product>(context);
                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);
            }

            using(var context = new XUnitTestDbContext(_contextOptions))
            {
                var product = context.Product.FirstOrDefault(x => x.Name == newProduct.Name);

                Assert.Equal(newProduct.Name, product.Name);
            }
        }

        [Theory,InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeletedAllProducts(int categoryID)
        {
            using(var context = new XUnitTestDbContext(_contextOptions))
            {
                var category = await context.Category.FindAsync(categoryID);

                context.Category.Remove(category);

                context.SaveChanges();
            }

            using(var context = new XUnitTestDbContext(_contextOptions))
            {
                var products = await context.Product.Where(x=>x.CategoryID == categoryID).ToListAsync();

                Assert.Empty(products);
            }
        }
    }
}
