﻿using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader> 
    {
        void UpDate(OrderHeader orderHeader);
         void UpdateStatus(int id , string orderStatus , string? paymentStatus = null);
        void UpDataStripePayment(int id , string sessionID , string paymentintentId);
    }
}