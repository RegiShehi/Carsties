using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        return await repo.GetAuctionsAsync(date);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await repo.GetAuctionsByIdAsync(id);

        if (auction is null) return NotFound();

        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(AuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);

        auction.Seller = User.Identity?.Name;

        repo.AddAuction(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await repo.SaveChangesAsync();

        if (!result) return BadRequest("Couldn't save changes");

        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {
        var auction = await repo.GetAuctionEntityByIdAsync(id);

        if (auction is null) return NotFound();

        if (auction.Seller != User.Identity?.Name) return Forbid();

        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Model = auctionDto.Color ?? auction.Item.Model;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await repo.SaveChangesAsync();

        if (!result) return BadRequest("Couldn't save changes");

        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await repo.GetAuctionEntityByIdAsync(id);

        if (auction is null) return NotFound();

        if (auction.Seller != User.Identity?.Name) return Forbid();

        repo.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await repo.SaveChangesAsync();

        if (!result) return BadRequest("Couldn't save changes");

        return Ok();
    }
}