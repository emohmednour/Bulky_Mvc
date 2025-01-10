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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Company company)
        {
            var result = db.companies.FirstOrDefault(u => u.id == company.id);
            if (result != null)
            {
                result.Name = company.Name;
                result.PhoneNumber = company.PhoneNumber;
                result.City = company.City;
                result.StreetAddress = company.StreetAddress;
                result.State = company.State;
                
            }
        }
    }
}
