using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.API.Data;
using OrderManagementSystem.Worker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

    services.AddHostedService<OrderProcessor>();
});

var host = builder.Build();
host.Run();
