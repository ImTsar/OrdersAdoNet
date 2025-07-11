using OrdersEfCore.DTOs;
using OrdersEfCore.Models;

namespace OrdersEfCore.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrders(bool asNoTracking = true);
        Task<Order> GetByIdAsync(int id);
        IQueryable<Analysis> GetAllAnalysis();
        Task AddAsync(Order order);
        void Delete(Order order);
    }
}
