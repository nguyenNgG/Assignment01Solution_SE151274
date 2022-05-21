using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class OrderDAO
    {
        private static OrderDAO instance = null;
        private static readonly object instanceLock = new object();
        private OrderDAO() { }

        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<Order> GetOrder(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Order order = null;
            order = await db.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            return order;
        }

        public async Task<List<Order>> GetOrders(int memberId)
        {
            FStoreDBContext db = new FStoreDBContext();
            return await db.Orders.Where(m => m.MemberId == memberId).ToListAsync();
        }

        public async Task<List<Order>> GetOrders()
        {
            FStoreDBContext db = new FStoreDBContext();
            return await db.Orders.ToListAsync();
        }

        public async Task AddOrder(Order order)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Orders.Add(order);
            await db.SaveChangesAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Orders.Update(order);
            await db.SaveChangesAsync();
        }

        public async Task DeleteOrder(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Order order = new Order { OrderId = id };
            db.Orders.Attach(order);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
        }
    }
}
