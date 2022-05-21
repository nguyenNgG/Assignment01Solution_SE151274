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
    public class OrderRepository : IOrderRepository
    {
        public Task AddOrder(Order order) => OrderDAO.Instance.AddOrder(order);
        public Task DeleteOrder(int id) => OrderDAO.Instance.DeleteOrder(id);
        public Task<Order> GetOrder(int id) => OrderDAO.Instance.GetOrder(id);
        public Task<List<Order>> GetOrders() => OrderDAO.Instance.GetOrders();
        public Task<List<Order>> GetOrders(int memberId) => OrderDAO.Instance.GetOrders(memberId);
        public Task UpdateOrder(Order order) => OrderDAO.Instance.UpdateOrder(order);
    }
}
