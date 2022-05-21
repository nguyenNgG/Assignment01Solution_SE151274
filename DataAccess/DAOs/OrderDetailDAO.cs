using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;
        private static readonly object instanceLock = new object();
        private OrderDetailDAO() { }

        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<OrderDetail> GetOrderDetail(int orderId, int productId)
        {
            FStoreDBContext db = new FStoreDBContext();
            OrderDetail orderDetail = null;
            orderDetail = await db.OrderDetails.FirstOrDefaultAsync(m => m.OrderId == orderId && m.ProductId == productId);
            return orderDetail;
        }

        public async Task<List<OrderDetail>> GetOrderDetails(int orderId)
        {
            FStoreDBContext db = new FStoreDBContext();
            List<OrderDetail> orderDetails = await db.OrderDetails.Where(m => m.OrderId == orderId).ToListAsync();
            return orderDetails;
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.OrderDetails.Add(orderDetail);
            await db.SaveChangesAsync();
        }

        public async Task UpdateOrderDetail(OrderDetail orderDetail)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.OrderDetails.Update(orderDetail);
            await db.SaveChangesAsync();
        }

        public async Task DeleteOrderDetail(int orderId, int productId)
        {
            FStoreDBContext db = new FStoreDBContext();
            OrderDetail orderDetail = new OrderDetail { ProductId = productId, OrderId = orderId };
            db.OrderDetails.Attach(orderDetail);
            db.OrderDetails.Remove(orderDetail);
            await db.SaveChangesAsync();
        }
    }
}
