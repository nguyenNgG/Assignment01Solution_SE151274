using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        public Task<List<OrderDetail>> GetOrderDetails(int orderId);
        public Task<OrderDetail> GetOrderDetail(int orderId, int productId);
        public Task AddOrderDetail(OrderDetail orderDetail);
        public Task UpdateOrderDetail(OrderDetail orderDetail);
        public Task DeleteOrderDetail(int orderId, int productId);
    }
}
