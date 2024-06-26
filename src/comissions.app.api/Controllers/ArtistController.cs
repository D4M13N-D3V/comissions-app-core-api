using comissions.app.api.Extensions;
using comissions.app.api.Models.PortfolioModel;
using comissions.app.api.Models.SellerProfile;
using comissions.app.api.Models.SellerProfileRequest;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Novu;
using Novu.DTO.Events;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;

    public ArtistController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
    {
        _client = client;
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    
    [HttpGet]
    [Authorize("read:artist")]
    public async Task<IActionResult> GetArtist()
    {
        var userId = User.GetUserId();
        var Artist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if(Artist==null)
        {
            var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(ArtistRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        var result = Artist.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:artist")]
    public async Task<IActionResult> UpdateArtist(ArtistModel model)
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist == null)
        {
            var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(ArtistRequest!=null)
                return BadRequest();
            return Unauthorized();
        }

        if (_dbContext.UserArtists.Any(x => x.Name.ToLower() == model.Name.ToLower()))
            return BadRequest();
        
        var updatedArtist = model.ToModel(existingArtist);
        updatedArtist = _dbContext.UserArtists.Update(updatedArtist).Entity;
        await _dbContext.SaveChangesAsync();
        var result = updatedArtist.ToModel();

        var newTriggerModel = new EventCreateData()
        {
            EventName = "artistupdated",
            To =
            {
                SubscriberId = userId,
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Stats")]
    public async Task<IActionResult> GetArtistStats()
    {
        var userId = User.GetUserId();
        var Artist = await _dbContext.UserArtists.Include(x=>x.Requests).FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if(Artist==null)
        {
            var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(ArtistRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        var result = Artist.ToStatsModel();
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Request")]
    public async Task<IActionResult> GetArtistRequest()
    {
        var userId = User.GetUserId();
        var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if(ArtistRequest==null)
            return NotFound();
        var result = ArtistRequest.ToModel();
        return Ok(result);
    }
}