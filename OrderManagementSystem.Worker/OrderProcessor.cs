using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.API.Data;
using OrderManagementSystem.API.Models;

namespace OrderManagementSystem.Worker
{
    public class OrderProcessor : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly ILogger<OrderProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderProcessor(
            IConfiguration configuration,
            ILogger<OrderProcessor> logger,
            IServiceProvider serviceProvider)
        {
            var connectionString = configuration.GetConnectionString("ServiceBus");
            var queueName = configuration["ServiceBus:QueueName"];
            
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(args.Message.Body);
                var order = JsonSerializer.Deserialize<Order>(messageBody);

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var orderToUpdate = await dbContext.Orders.FindAsync(order.Id);
                if (orderToUpdate != null)
                {
                    orderToUpdate.Status = "Processando";
                    await dbContext.SaveChangesAsync();

                    // Simulate processing time
                    await Task.Delay(5000);

                    orderToUpdate.Status = "Finalizado";
                    await dbContext.SaveChangesAsync();
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                throw;
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error processing message");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
} 