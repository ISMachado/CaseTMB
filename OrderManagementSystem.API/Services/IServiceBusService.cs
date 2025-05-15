using System.Threading.Tasks;
using OrderManagementSystem.API.Models;

namespace OrderManagementSystem.API.Services
{
    public interface IServiceBusService
    {
        Task SendMessageAsync(Order order);
    }
} 