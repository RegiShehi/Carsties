using AuctionService.Data;
using AuctionService.Entities;
using AutoFixture;

namespace AuctionService.IntegrationTests.Util;

public static class DbHelper
{
    private static readonly Fixture Fixture = new();

    public static void InitDbForTests(AuctionDbContext db)
    {
        db.Auctions.AddRange(GetAuctionsForTest());
        db.SaveChanges();
    }

    public static void ReInitDbForTests(AuctionDbContext db)
    {
        db.Auctions.RemoveRange(db.Auctions);
        db.SaveChanges();

        InitDbForTests(db);
    }

    private static List<Auction> GetAuctionsForTest()
    {
        // var fixture = new Fixture();

        var auctions = new List<Auction>();

        for (var i = 0; i < 3; i++) // Create 3 random auctions
        {
            // Build auction without the Item property
            var auction = Fixture.Build<Auction>()
                .With(a => a.CreatedAt, () => DateTime.UtcNow)
                .With(a => a.UpdatedAt, () => DateTime.UtcNow)
                .With(a => a.AuctionEnd, () => DateTime.UtcNow.AddDays(Fixture.Create<int>() % 60 + 1))
                .Without(a => a.Item)
                .Create();

            // Build Item separately and assign to Auction
            var item = Fixture.Build<Item>()
                .Without(b => b.Auction)
                .Create();

            auction.Item = item;
            auctions.Add(auction);
        }

        return auctions;
    }
}