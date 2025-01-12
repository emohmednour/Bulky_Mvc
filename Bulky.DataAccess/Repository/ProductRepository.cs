using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Product obj)
        {
             var objFromDB= db.products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.Description = obj.Description;
                objFromDB.Author = obj.Author;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price = obj.Price;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.ProductImages = obj.ProductImages;
                //if (obj.ImageURl != null)
                //{
                //    objFromDB.ImageURl=obj.ImageURl;
                //}

            
            
            
            
            }

        }
    }
}
