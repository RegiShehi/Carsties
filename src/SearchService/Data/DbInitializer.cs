using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb",
            MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text).CreateAsync();

        // Call Auction Service through HTTP Client to fetch data and populate search service
        using var scope = app.Services.CreateScope();

        // var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = new List<Item>(); // await httpClient.GetItemsForSearchDb();

        Console.WriteLine(items.Count + " returned from Auction Service");

        if (items.Count > 0) await DB.SaveAsync(items);
    }
}