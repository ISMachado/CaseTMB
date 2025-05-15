using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.API.Data;
using OrderManagementSystem.API.Models;
using OrderManagementSystem.API.Services;
using System.Linq;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceBusService _serviceBusService;

        public OrdersController(ApplicationDbContext context, IServiceBusService serviceBusService)
        {
            _context = context;
            _serviceBusService = serviceBusService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .OrderBy(o => o.DataCriacao)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            order.Id = Guid.NewGuid();
            order.DataCriacao = DateTime.UtcNow;
            // order.Status = "Pendente";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _serviceBusService.SendMessageAsync(order);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, Order updatedOrder)
        {
            if (id != updatedOrder.Id)
                return BadRequest();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Cliente = updatedOrder.Cliente;
            order.Produto = updatedOrder.Produto;
            order.Valor = updatedOrder.Valor;
            order.Status = updatedOrder.Status;
            // Não atualiza DataCriacao

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 