using OrdersEfCore.Data;
using OrdersEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace OrdersEfCore.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _context;

        public OrderRepository(OrdersContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetOrders(bool asNoTracking = true)
        {
            var query = _context.Orders.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public IQueryable<Analysis> GetAllAnalysis()
        {
            return _context.Analyses.AsQueryable();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }
    }
}
