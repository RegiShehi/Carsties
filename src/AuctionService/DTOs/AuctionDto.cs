namespace AuctionService.DTOs;

public class AuctionDto
{
    public Guid Id { get; init; }
    public int ReservePrice { get; init; }
    public string? Seller { get; init; }
    public string? Winner { get; init; }
    public int CurrentHighBid { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime AuctionEnd { get; init; }
    public string? Status { get; init; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string? ImageUrl { get; set; }
}