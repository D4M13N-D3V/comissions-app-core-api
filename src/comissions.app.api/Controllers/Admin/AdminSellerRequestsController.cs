using comissions.app.api.Entities;
using comissions.app.api.Extensions;
using comissions.app.api.Models;
using comissions.app.api.Models.SellerProfileRequest;
using comissions.app.api.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novu;
using Novu.DTO.Events;

namespace comissions.app.api.Controllers.Admin;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminArtistRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;
    public AdminArtistRequestsController(NovuClient client, ApplicationDbContext dbContext, IPaymentService paymentService)
    {
        _client = client;
        _paymentService = paymentService;
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Gets a list of all of the requests from users to become a seller.
    /// </summary>
    /// <param name="offset"> The offset to start at.</param>
    /// <param name="pageSize"> The amount of records to return.</param>
    /// <returns>A list of seller profile requests</returns>
    [HttpGet]
    public async Task<IActionResult> GetArtistRequests(int offset = 0, int pageSize = 10)
    {
        var requests = _dbContext.ArtistRequests.Include(x=>x.User).OrderByDescending(x=>x.Id).Skip(offset).Take(pageSize).ToList();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("{requestId:int}")]
    public async Task<IActionResult> GetArtistRequest(int requestId)
    {
        var request = await _dbContext.ArtistRequests.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var result = request.ToModel();
        return Ok(result);
    }
    
    /// <summary>
    /// Gets the amount of requests there are from users to become a seller.
    /// </summary>
    /// <returns>The number of requests.</returns>
    [HttpGet]
    [Route("Count")]
    public async Task<IActionResult> GetArtistRequestsCount()
    {
        var result = _dbContext.ArtistRequests.Count();
        return Ok(result);
    }
    
    /// <summary>
    /// Accepts a request to become a seller from a user.
    /// </summary>
    /// <param name="userId">The ID of the user to accept the request for.</param>
    /// <returns>The new seller profile.</returns>
    [HttpPut]
    [Route("{requestId:int}")]
    public async Task<IActionResult> AcceptArtistRequest(int requestId)
    {
        var request = await _dbContext.ArtistRequests.Include(x=>x.User).FirstOrDefaultAsync(request=>request.Id==requestId);
        
        if(request==null)
            return NotFound("No request for that user exists.");
        
        if (request.Accepted == true)
            return BadRequest("User is already a seller.");

        request.Accepted = true;
        request.AcceptedDate = DateTime.UtcNow;
        var accountId = _paymentService.CreateArtistAccount();
        var newArtist = new UserArtist()
        {
            UserId = request.UserId,
            AgeRestricted = false,
            Description = string.Empty,
            StripeAccountId = accountId,
            SocialMediaLink1 = "",
            SocialMediaLink2 = "",
            SocialMediaLink3 = "",
            SocialMediaLink4 = "",
            RequestGuidelines = "",
            Name = "Default Shop",
        };
        var dbProfile = _dbContext.UserArtists.Add(newArtist).Entity;
        await _dbContext.SaveChangesAsync();
        var newSettings = new ArtistPageSettings()
        {
            ArtistId = dbProfile.Id,
            BackgroundColor = "rgb(126, 115, 115)",
            HeaderColor = "rgb(194, 187, 187)",
            HeaderTextSize = 5,
            HeaderUseImage = false,
            HeaderImageUrl = "",
            DescriptionHeaderText = "",
            DescriptionHeaderColor = "rgb(194, 187, 187)",
            DescriptionHeaderSize = 3,
            DescriptionHeaderUseImage = false,
            DescriptionHeaderImageUrl = "",
            DescriptionBackgroundColor = "rgb(103, 97, 97)",
            DescriptionTextColor = "rgb(186, 186, 186)",
            DescriptionTextSize = 1,
            PortfolionHeaderText = "",
            PortfolionHeaderColor = "rgb(194, 187, 187)",
            PortfolionHeaderSize = 3,
            PortfolionHeaderUseImage = false,
            PortfolionHeaderImageUrl = "",
            PortfolioBackgroundColor = "rgb(78, 73, 73)",
            PortfolioMasonry = true,
            PortfolioColumns = 3,
            PortfolioEnabledScrolling = true,
            PortfolioMaximumSize = 50,
            RequestHeaderText = "",
            RequestHeaderColor = "rgb(194, 187, 187)",
            RequestHeaderSize = 3,
            RequestHeaderUseImage = false,
            RequestHeaderImageUrl = "",
            RequestBackgroundColor = "rgb(103, 97, 97)",
            RequestTermsColor = "rgb(194, 187, 187)",
            RequestButtonBGColor = "rgb(101, 97, 97)",
            RequestButtonTextColor = "rgb(194, 187, 187)",
            RequestButtonHoverBGColor = "rgb(98, 98, 98)"
        };
        var dbSettings = _dbContext.ArtistPageSettings.Add(newSettings).Entity;
        await _dbContext.SaveChangesAsync();
        
        request = _dbContext.ArtistRequests.Update(request).Entity;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        var newTriggerModel = new EventCreateData()
        {
            EventName = "artistrequestaccepted",
            To =
            {
                SubscriberId = request.UserId,
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        return Ok(result);
    }
    
    [HttpDelete]
    [Route("{requestId:int}")]
    public async Task<IActionResult> DenyArtistRequest(int requestId)
    {
        var request = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.Id==requestId);
        
        if(request==null)
            return NotFound("No request for that user exists.");
        
        if (request.Accepted == true)
            return BadRequest("User is already a seller.");
        
        _dbContext.ArtistRequests.Remove(request);
        await _dbContext.SaveChangesAsync();
        var newTriggerModel = new EventCreateData()
        {
            EventName = "artistrequestdenied",
            To =
            {
                SubscriberId = request.UserId,
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        return Ok();
    }
    
    [HttpGet]
    [Route("Messages")]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _dbContext.ArtistRequestMessages.ToListAsync();
        var result = messages.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Messages")]
    public async Task<IActionResult> AddMessage([FromBody] string message)
    {
        var newMessage = new ArtistRequestMessage()
        {
            UserId = User.GetUserId(),
            Message = message,
            SentDate = DateTime.UtcNow
        };
        _dbContext.ArtistRequestMessages.Add(newMessage);
        await _dbContext.SaveChangesAsync();
        return Ok(newMessage.ToModel());
    }
}