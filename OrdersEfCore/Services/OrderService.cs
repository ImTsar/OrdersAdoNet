using OrdersEfCore.Data;
using OrdersEfCore.DTOs;
using OrdersEfCore.Models;
using OrdersEfCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OrdersEfCore.Services
{
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrdersContext _context;

        public OrderService(IOrderRepository orderRepository, OrdersContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetLastYearAsync()
        {
            var allOrders = _orderRepository.GetOrders();
            var lastYear = DateTime.UtcNow.AddYears(-1);

            return await allOrders
                .Where(o => o.CreationDate >= lastYear)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CreationDate = o.CreationDate,
                    AnalysisId = o.AnalysisId,
                    AnalysisName = o.Analysis.Name,
                })
                .ToListAsync();
        }

        public async Task<int> CreateAsync(DateTime creationDate, int analysisId)
        {
            await EnsureAnalysisExistsAsync(analysisId);

            var newOrder = new Order
            {
                CreationDate = creationDate,
                AnalysisId = analysisId
            };

            await _orderRepository.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return newOrder.Id;
        }

        public async Task<bool> AnalysisExistsAsync(int analysisId)
        {
            return await _orderRepository.GetAllAnalysis().AnyAsync(a => a.Id == analysisId);
        }

        public async Task UpdateAsync(int ordId, DateTime creationDate, int analysisId)
        {
            var order = await GetOrder(ordId);
            await EnsureAnalysisExistsAsync(analysisId);

            order.CreationDate = creationDate;
            order.AnalysisId = analysisId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int ordId)
        {
            var order = await GetOrder(ordId);

            _orderRepository.Delete(order);
            await _context.SaveChangesAsync();
        }

        private async Task<Order> GetOrder(int ordId)
        {
            var order = await _orderRepository.GetByIdAsync(ordId);
            if (order == null)
                throw new InvalidOperationException($"Order with ID {ordId} not found");

            return order;
        }

        private async Task EnsureAnalysisExistsAsync(int analysisId)
        {
            var analysisExists = await _orderRepository.GetAllAnalysis().AnyAsync(a => a.Id == analysisId);

            if (!analysisExists)
                throw new InvalidOperationException($"Analysis with ID {analysisId} not found");
        }
    }
}
