using Contracts;
using MassTransit;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreated>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue("RabbitMq:Host", "localhost"), "/",
            host =>
            {
                host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest") ?? string.Empty);
                host.Username(builder.Configuration.GetValue("RabbitMq:Password", "guest") ?? string.Empty);
            });

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();