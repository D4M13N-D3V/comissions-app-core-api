using comissions.app.api.Entities;
using comissions.app.api.Extensions;
using comissions.app.api.Models;
using comissions.app.api.Models.PortfolioModel;
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
public class ArtistAccessRequestController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;

    public ArtistAccessRequestController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
    {
        _client = client;
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    [HttpPost]
    [Authorize("write:artist")]
    public async Task<IActionResult> RequestArtist([FromBody] string message)
    {
        var userId = User.GetUserId();
        
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist != null)
        {
            return Unauthorized();
        }
        
        var artistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (artistRequest != null)
            return Ok(artistRequest.ToModel());
        
        artistRequest = new ArtistRequest()
        {
            Accepted = false,
            Message = message,
            RequestDate = DateTime.UtcNow,
            UserId = userId
        };
                
        _dbContext.ArtistRequests.Add(artistRequest);
        await _dbContext.SaveChangesAsync();
        return Ok(artistRequest.ToModel());
    }   
    
    [HttpGet]
    [Authorize("read:artist")]
    public async Task<IActionResult> GetRequest()
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist != null)
        {
            return Unauthorized();
        }
        
        var artistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (artistRequest == null)
            return NotFound();
        
        return Ok(artistRequest.ToModel());
    }
    
    [HttpPost]
    [Authorize("write:artist")]
    [Route("Messages")]
    public async Task<IActionResult> AddMessage([FromBody] string message)
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist != null)
        {
            return Unauthorized();
        }
        
        var artistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (artistRequest == null)
            return NotFound();

        var newMessage = new ArtistRequestMessage()
        {
            UserId = userId,
            Message = message,
            SentDate = DateTime.UtcNow
        };
        artistRequest.ArtistRequestMessages.Add(newMessage);
        await _dbContext.SaveChangesAsync();
        return Ok(artistRequest.ToModel());
    }
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Messages")]
    public async Task<IActionResult> GetMessages()
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist != null)
        {
            return Unauthorized();
        }
        var artistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (artistRequest == null)
            return NotFound();
        var result = artistRequest.ArtistRequestMessages.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }
}