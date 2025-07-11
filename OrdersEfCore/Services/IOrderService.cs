using OrdersEfCore.DTOs;

namespace OrdersEfCore.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetLastYearAsync();
        Task<int> CreateAsync(DateTime creationDate, int analysisId);
        Task<bool> AnalysisExistsAsync(int analysisId);
        Task UpdateAsync(int ordId, DateTime creationDate, int analysisId);
        Task DeleteAsync(int ordId);
    }
}
