using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdersEfCore.Data;
using OrdersEfCore.Repositories;
using OrdersEfCore.Services;

var services = new ServiceCollection();

services.AddDbContext<OrdersContext>(options =>
    options.UseSqlServer("Data Source=DESKTOP-9AG710N\\SQLEXPRESS;Initial Catalog=OrdersDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IOrderService, OrderService>();

var serviceProvider = services.BuildServiceProvider();

var orderService = serviceProvider.GetRequiredService<IOrderService>();

var analysisId = 1;
var orderId = 0;
var startDate = DateTime.UtcNow;

Console.WriteLine("Creating 5 orders with 4 months interval backwards:");

for (int i = 0; i < 5; i++)
{
    var date = startDate.AddMonths(-6 * i);

    try
    {
        orderId = await orderService.CreateAsync(date, analysisId);
        Console.WriteLine($"Order with ID {orderId} created on {date:yyyy-MM-dd}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error creating order #{i + 1}: {ex.Message}");
    }
}

Console.WriteLine("\nOrders last year:");
var lastYearOrders = await orderService.GetLastYearAsync();

if (!lastYearOrders.Any())
{
    Console.WriteLine("No orders found within the last year.");
}
else
{
    foreach (var order in lastYearOrders)
    {
        Console.WriteLine($"Order ID: {order.Id}, Date: {order.CreationDate}, Analysis ID: {order.AnalysisId}, Analysis Name: {order.AnalysisName}");
    }
}

try
{
    var dateTime = DateTime.UtcNow;

    orderId = await orderService.CreateAsync(dateTime, analysisId);
    Console.WriteLine($"\nOrder with Id {orderId} was created.");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"\nValidation error: {ex.Message}");
}

try
{
    var dateTime = DateTime.UtcNow.AddDays(-1);
    analysisId = 2;

    await orderService.UpdateAsync(orderId, dateTime, analysisId);
    Console.WriteLine($"\nOrder with Id {orderId} was updated.");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"\nValidation error: {ex.Message}");
}

try
{
    await orderService.DeleteAsync(orderId);
    Console.WriteLine($"\nOrder with Id {orderId} was deleted.");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"\nValidation error: {ex.Message}");
}
