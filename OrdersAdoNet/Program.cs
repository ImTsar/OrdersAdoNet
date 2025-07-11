using OrdersAdoNet.DTOs;
using OrdersAdoNet.Services;

var connectionString = "Data Source=DESKTOP-9AG710N\\SQLEXPRESS;Initial Catalog=OrdersDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

var orderService = new OrdersService(connectionString);
var ordersReader = await orderService.GetLastYearWithDataReaderAsync();

Console.WriteLine("Orders from the last year (SqlDataReader):");
if (!ordersReader.Any())
{
    Console.WriteLine("No orders found.");
}
else
{
    foreach (var order in ordersReader)
    {
        Console.WriteLine($"Order ID: {order.Id}, Date: {order.CreationDate:yyyy-MM-dd HH:mm:ss}, Analysis ID: {order.AnalysisId}, Analysis Name: {order.AnalysisName}");
    }
}

var ordersDataSet = await orderService.GetLastYearWithDataSetAsync();

Console.WriteLine("\nOrders from the last year (DataSet):");
if (!ordersDataSet.Any())
{
    Console.WriteLine("No orders found.");
}
else
{
    foreach (var order in ordersDataSet)
    {
        Console.WriteLine($"Order ID: {order.Id}, Date: {order.CreationDate:yyyy-MM-dd HH:mm:ss}, Analysis ID: {order.AnalysisId}, Analysis Name: {order.AnalysisName}");
    }
}

int orderId = 0;
try
{
    var newOrder = new OrderDto
    {
        CreationDate = DateTime.Now,
        AnalysisId = 1 
    };

    orderId = await orderService.CreateOrderAsync(newOrder);
    Console.WriteLine($"\nOrder created successfully with ID: {orderId}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"\nValidation error creating order: {ex.Message}");
}

try
{
    var updatedOrder = new OrderDto
    {
        Id = orderId,
        CreationDate = DateTime.Now.AddHours(-1),
        AnalysisId = 2
    };

    await orderService.UpdateOrderAsync(updatedOrder);
    Console.WriteLine($"Order with ID {orderId} updated successfully.");

}
catch (ArgumentException ex)
{
    Console.WriteLine($"\nValidation error updating order: {ex.Message}");
}

try
{
    await orderService.DeleteOrderAsync(35);
    Console.WriteLine($"Order with ID {orderId} deleted successfully.");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"\nValidation error deleting order: {ex.Message}");
}