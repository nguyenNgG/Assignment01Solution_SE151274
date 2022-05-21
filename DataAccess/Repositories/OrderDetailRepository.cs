using BusinessObjects;
using DataAccess.DAOs;
using DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public Task AddOrderDetail(OrderDetail orderDetail) => OrderDetailDAO.Instance.AddOrderDetail(orderDetail);
        public Task DeleteOrderDetail(int orderId, int productId) => OrderDetailDAO.Instance.DeleteOrderDetail(orderId, productId);
        public Task<OrderDetail> GetOrderDetail(int orderId, int productId) => OrderDetailDAO.Instance.GetOrderDetail(orderId, productId);
        public Task<List<OrderDetail>> GetOrderDetails(int orderId) => OrderDetailDAO.Instance.GetOrderDetails(orderId);
        public Task UpdateOrderDetail(OrderDetail orderDetail) => OrderDetailDAO.Instance.UpdateOrderDetail(orderDetail);
    }
}
