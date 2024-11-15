using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x => { x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); }); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseAuthorization();

app.MapControllers();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        // .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound);
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}