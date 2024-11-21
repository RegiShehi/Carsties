using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddMassTransit(x =>
{
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

app.Run();