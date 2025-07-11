using Microsoft.Data.SqlClient;
using OrdersAdoNet.DTOs;
using System.Data;

namespace OrdersAdoNet.Services
{
    public class OrdersService
    {
        public readonly string _connectionString;

        public OrdersService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<OrderDto>> GetLastYearWithDataReaderAsync()
        {
            var orders = new List<OrderDto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    @"SELECT o.ord_id, o.ord_datetime, o.ord_an, a.an_name 
                    FROM Orders o
                    INNER JOIN Analysis a ON o.ord_an = a.an_id
                    WHERE o.ord_datetime >= @OneYearAgo",
                    connection))
                {
                    command.Parameters.AddWithValue("@OneYearAgo", DateTime.Now.AddYears(-1));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            orders.Add(new OrderDto
                            {
                                Id = reader.GetInt32(0),
                                CreationDate = reader.GetDateTime(1),
                                AnalysisId = reader.GetInt32(2),
                                AnalysisName = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return orders;
        }

        public async Task<IEnumerable<OrderDto>> GetLastYearWithDataSetAsync()
        {
            var orders = new List<OrderDto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var adapter = new SqlDataAdapter(
                    @"SELECT o.ord_id, o.ord_datetime, o.ord_an, a.an_name 
                    FROM Orders o
                    INNER JOIN Analysis a ON o.ord_an = a.an_id
                    WHERE o.ord_datetime >= @OneYearAgo",
                    connection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@OneYearAgo", DateTime.Now.AddYears(-1));
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        orders.Add(new OrderDto
                        {
                            Id = (int)row["ord_id"],
                            CreationDate = (DateTime)row["ord_datetime"],
                            AnalysisId = (int)row["ord_an"],
                            AnalysisName = (string)row["an_name"]
                        });
                    }
                }
            }

            return orders;
        }

        public async Task<int> CreateOrderAsync(OrderDto orderDto)
        {
            if (orderDto.CreationDate > DateTime.Now)
                throw new ArgumentException("Order date cannot be in the future");

            if (!await AnalysisExistsAsync(orderDto.AnalysisId))
                throw new ArgumentException("Analysis ID does not exist");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    @"INSERT INTO Orders (ord_datetime, ord_an) 
                    VALUES (@OrderDateTime, @AnalysisId);
                    SELECT SCOPE_IDENTITY();",
                    connection))
                {
                    command.Parameters.AddWithValue("@OrderDateTime", orderDto.CreationDate);
                    command.Parameters.AddWithValue("@AnalysisId", orderDto.AnalysisId);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            if (!await OrderExistsAsync(orderDto.Id))
                throw new ArgumentException("Order ID does not exist");

            if (orderDto.CreationDate > DateTime.Now)
                throw new ArgumentException("Order date cannot be in the future");

            if (!await AnalysisExistsAsync(orderDto.AnalysisId))
                throw new ArgumentException("Analysis ID does not exist");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    @"UPDATE Orders 
                  SET ord_datetime = @OrderDateTime, ord_an = @AnalysisId 
                  WHERE ord_id = @OrderId",
                    connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderDto.Id);
                    command.Parameters.AddWithValue("@OrderDateTime", orderDto.CreationDate);
                    command.Parameters.AddWithValue("@AnalysisId", orderDto.AnalysisId);                  
                }
            }
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            if (!await OrderExistsAsync(orderId))
                throw new ArgumentException("Order ID does not exist");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    @"DELETE FROM Orders WHERE ord_id = @OrderId",
                    connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                }
            }
        }

        private async Task<bool> OrderExistsAsync(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "SELECT COUNT(*) FROM Orders WHERE ord_id = @OrderId",
                    connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    return (int)await command.ExecuteScalarAsync() > 0;
                }
            }
        }

        private async Task<bool> AnalysisExistsAsync(int analysisId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    "SELECT COUNT(*) FROM Analysis WHERE an_id = @AnalysisId",
                    connection))
                {
                    command.Parameters.AddWithValue("@AnalysisId", analysisId);

                    return (int)await command.ExecuteScalarAsync() > 0;
                }
            }
        }
    }
}
