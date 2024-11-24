using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.Frameworks;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepo;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly Fixture _fixture;
    private readonly AuctionsController _controller;

    public AuctionControllerTests()
    {
        _auctionRepo = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        var mockMapper = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(MappingProfiles).Assembly); })
            .CreateMapper().ConfigurationProvider;

        _mapper = new Mapper(mockMapper);

        _fixture = new Fixture();
        _controller = new AuctionsController(_auctionRepo.Object, _mapper, _publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = Helpers.GetClaimsPrincipal()
                }
            }
        };
    }

    [Fact]
    public async Task GetAuctions_WithNoParams_ReturnsAllAuctions()
    {
        //arrange 
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepo.Setup(r => r.GetAuctionsAsync(string.Empty)).ReturnsAsync(auctions);

        //act
        var result = await _controller.GetAllAuctions(string.Empty);

        //assert
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        //arrange 
        var auction = _fixture.Create<AuctionDto>();
        _auctionRepo.Setup(r => r.GetAuctionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        //act
        var result = await _controller.GetAuctionById(auction.Id);

        //assert
        Assert.NotNull(result.Value);
        Assert.Equal(auction, result.Value);
        Assert.IsType<ActionResult<AuctionDto>>(result);
        Assert.IsType<AuctionDto>(result.Value);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
    {
        //arrange 
        // _auctionRepo.Setup(r => r.GetAuctionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AuctionDto?)null);
        _auctionRepo.Setup(r => r.GetAuctionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

        //act
        var result = await _controller.GetAuctionById(Guid.NewGuid());

        //assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAuction()
    {
        //arrange 
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepo.Setup(r => r.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        //act
        var result = await _controller.CreateAuction(auction);
        var createdResult = result.Result as CreatedAtActionResult;

        //assert
        Assert.NotNull(createdResult);
        Assert.Equal("GetAuctionById", createdResult.ActionName);
        Assert.IsType<ActionResult<AuctionDto>>(result);
        Assert.IsType<AuctionDto>(createdResult.Value);
    }
}