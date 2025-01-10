using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void UpDate(OrderHeader orderHeader)
        {
          _db.orderHeaders.Update(orderHeader);
        }
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderfromDB = _db.orderHeaders.FirstOrDefault(u => u.Id == id );
            if (orderfromDB != null)
            {
                orderfromDB.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderfromDB.PaymentStatus = paymentStatus;
                }

            }
        }

        public void UpDataStripePayment(int id, string sessionID, string paymentintentId)
        {
            var orderfromDB = _db.orderHeaders.FirstOrDefault(u => u.Id == id );
            if (!string.IsNullOrEmpty(sessionID))
            {
                orderfromDB.SessionId = sessionID;
            }
            if (!string.IsNullOrEmpty(paymentintentId)) { 
            
                orderfromDB.PaymentIntentId = paymentintentId;
                orderfromDB.PaymentDate = DateTime.Now;
            
            
            }
        }


    }
}