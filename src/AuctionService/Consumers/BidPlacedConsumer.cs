using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext dbContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consuming bid placed");

        var auction = await dbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction is null) return;

        if (auction.CurrentHighBid is null || (context.Message.BidStatus.Contains("Accepted") &&
                                               context.Message.Amount > auction.CurrentHighBid))
        {
            auction.CurrentHighBid = context.Message.Amount;

            await dbContext.SaveChangesAsync();
        }
    }
}