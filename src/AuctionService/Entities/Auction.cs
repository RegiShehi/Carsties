namespace AuctionService.Entities;

public record Auction
{
    public Guid Id { get; init; }
    public int ReservePrice { get; init; }
    public string? Seller { get; init; }
    public string? Winner { get; init; }
    public int? CurrentHighBid { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public DateTime AuctionEnd { get; init; }
    public Status Status { get; init; }
    public Item? Item { get; init; }
}