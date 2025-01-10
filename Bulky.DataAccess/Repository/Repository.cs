using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext db;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext db)
        {
            this.db = db;
            this.DbSet = db.Set<T>();
            //db.categories == dbset;
            db.products.Include(u => u.Category).Include(u => u.CategoryId);

        }
        public void Add(T entity)
        {
           DbSet.Add(entity);

        }
        public T Get(Expression<Func<T, bool>> Filter,string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> quary ;
            if (tracked)
            {
                quary = DbSet;
            }
            else
            {
                quary = DbSet.AsNoTracking();
            }

            quary = quary.Where(Filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var inclideProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    quary = quary.Include(inclideProp);
                }

            }
            return quary.FirstOrDefault();
            
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> Filter, string? includeProperties = null)
        {
            IQueryable<T> quary = DbSet;
            if (Filter != null)
            {
            quary = quary.Where(Filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var inclideProp  in includeProperties.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries))
                {
                    quary = quary.Include(inclideProp);
                }

            }
            return quary.ToList();
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);

        }

       
    }
}
