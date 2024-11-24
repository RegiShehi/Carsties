using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
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
        _controller = new AuctionsController(_auctionRepo.Object, _mapper, _publishEndpoint.Object);
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
}