using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ProductDAO

    {
        public ProductDAO() { }
        public static List<Product> GetProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new EStoreContext())
                {
                    listProducts = context.Products.ToList();
                    listProducts.ForEach(f =>
                    {
                        f.Category = context.Categories.Find(f.CategoryId);
                    });
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listProducts;
        }

        public static List<Product> Search(string keyword)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new EStoreContext())
                {
                    bool hasPrice = decimal.TryParse(keyword, out decimal price);

                    listProducts = context.Products
                        .Where(f => f.ProductName.Contains(keyword) || (hasPrice && f.UnitPrice == price))
                        .ToList();

                    listProducts.ForEach(f =>
                    {
                        f.Category = context.Categories.Find(f.CategoryId);
                    });
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listProducts;
        }


        public static List<Product> FindAllProductsByCategoryId(int categoryId)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new EStoreContext())
                {
                    listProducts = context.Products.Where(f => f.CategoryId == categoryId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listProducts;
        }



        public static Product FindProductById(int ProductId)
        {
            var Product = new Product();
            try
            {
                using (var context = new EStoreContext())
                {
                    Product = context.Products.SingleOrDefault(f => f.ProductId == ProductId);
                    Product.Category = context.Categories.Find(Product.CategoryId);
                    Product.OrderDetails = context.OrderDetails.Where(o => o.ProductId == ProductId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Product;
        }

        public static void SaveProduct(Product Product)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    context.Products.Add(Product);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateProduct(Product Product)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    context.Entry(Product).State =
                        Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteProduct(Product Product)
        {
            try
            {
                using (var context = new EStoreContext())
                {
                    var ProductToDelete = context
                        .Products
                        .SingleOrDefault(f => f.ProductId == Product.ProductId);
                    context.Products.Remove(ProductToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
