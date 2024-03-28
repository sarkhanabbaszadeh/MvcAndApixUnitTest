using Microsoft.EntityFrameworkCore;
using MvcAndApixUnitTest.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAndApixUnitTest.Test
{
    public class ProductControllerTest
    {
        protected DbContextOptions<XUnitTestDbContext> _contextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<XUnitTestDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public void Seed()
        {

            using(XUnitTestDbContext context = new XUnitTestDbContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Category.Add(new Category { Name = "Pens" });
                context.Category.Add(new Category { Name = "Books" });

                context.Product.Add(new Product() { CategoryID = 1, Name = "Pen 1", Price = 85, Stock = 99, Color = "White" });
                context.Product.Add(new Product() { CategoryID = 1, Name = "Pen 2", Price = 65, Stock = 79, Color = "Black" });
            }
        }
    }
}
