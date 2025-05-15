using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using OrderManagementSystem.API.Models;

namespace OrderManagementSystem.API.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly ServiceBusClient _client;
        private readonly string _queueName;

        public ServiceBusService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ServiceBus");
            _queueName = configuration["ServiceBus:QueueName"];
            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendMessageAsync(Order order)
        {
            var sender = _client.CreateSender(_queueName);
            var messageBody = JsonSerializer.Serialize(order);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = order.Id.ToString(),
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message);
        }
    }
} 