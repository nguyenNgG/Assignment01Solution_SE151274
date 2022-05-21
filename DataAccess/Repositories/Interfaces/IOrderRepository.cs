using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetOrders();
        public Task<List<Order>> GetOrders(int memberId);
        public Task<Order> GetOrder(int id);
        public Task AddOrder(Order order);
        public Task UpdateOrder(Order order);
        public Task DeleteOrder(int id);
    }
}
