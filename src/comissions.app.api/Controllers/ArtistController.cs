using comissions.app.api.Extensions;
using comissions.app.api.Models.PortfolioModel;
using comissions.app.api.Models.Artist;
using comissions.app.api.Models.ArtistRequest;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Models;
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
    [Route("Payout")]
    public async Task<IActionResult> Payout()
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

        var account = _paymentService.GetAccount(Artist.StripeAccountId);
        var balance = _paymentService.GetBalance(Artist.StripeAccountId);
        var result = new PayoutModel()
        {
            Enabled = account.PayoutsEnabled,
            Balance = balance,
            PayoutUrl =  _paymentService.CreateDashboardUrl(Artist.StripeAccountId),
            OnboardUrl = _paymentService.CreateArtistAccountOnboardingUrl(Artist.StripeAccountId)
        };
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
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Page")]
    public async Task<IActionResult> GetArtistPage()
    {
        var userId = User.GetUserId();
        var Artist = await _dbContext.UserArtists.Include(x=>x.ArtistPageSettings).FirstOrDefaultAsync(artist=>artist.UserId==userId);
        if(Artist==null)
            return NotFound();
        var result = Artist.ArtistPageSettings.ToModel();
        return Ok(result);
    }
    
    
    [HttpPut]
    [Authorize("write:artist")]
    [Route("Page")]
    public async Task<IActionResult> UpdateArtistPage([FromBody]ArtistPageSettingsModel model)
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists
            .Include(x=>x.ArtistPageSettings).FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist == null)
        {
            var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(ArtistRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        var updatedArtist = model.ToModel(existingArtist.ArtistPageSettings);
        updatedArtist = _dbContext.ArtistPageSettings.Update(updatedArtist).Entity;
        await _dbContext.SaveChangesAsync();
        var result = updatedArtist.ToModel();
        return Ok(result);
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
        
        var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (ArtistRequest != null)
            return Ok(ArtistRequest.ToModel());
        
        ArtistRequest = new ArtistRequest()
        {
            Accepted = false,
            Message = message,
            RequestDate = DateTime.UtcNow,
            UserId = userId
        };
                
        _dbContext.ArtistRequests.Add(ArtistRequest);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }   
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("{sellerServiceId:int}/Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> GetPortfolio(int sellerServiceId, int portfolioId)
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
        if(existingArtist.Suspended)
            return BadRequest();

        var portfolio = await _dbContext.ArtistPortfolioPieces
            .FirstAsync(x => x.ArtistId == existingArtist.Id && x.Id==portfolioId);
        var content = await _storageService.DownloadImageAsync(portfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }

    [HttpGet]
    [Route("Portfolio")]
    [Authorize("read:artist")]
    public async Task<IActionResult> GetPortfolio()
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
        if(existingArtist.Suspended)
            return BadRequest();
        var portfolio = await _dbContext.ArtistPortfolioPieces.Where(x=>x.ArtistId==existingArtist.Id).ToListAsync();
        var result = portfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Portfolio")]
    [Authorize("write:artist")]
    public async Task<IActionResult> AddPortfolio()
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist == null)
        {
            return BadRequest();
        }

        if(existingArtist.Suspended)
            return BadRequest();
        var url = await _storageService.UploadImageAsync(HttpContext.Request.Body, Guid.NewGuid().ToString());
        var portfolio = new ArtistPortfolioPiece()
        {
            ArtistId = existingArtist.Id,
            FileReference = url
        };
        portfolio.ArtistId = existingArtist.Id;
        _dbContext.ArtistPortfolioPieces.Add(portfolio);
        await _dbContext.SaveChangesAsync();
        var result = portfolio.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:artist")]
    [Route("Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> DeletePortfolio(int portfolioId)
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
        if(existingArtist.Suspended)
            return BadRequest();
        var portfolio = await _dbContext.ArtistPortfolioPieces.FirstOrDefaultAsync(x=>x.Id==portfolioId);
        if(portfolio==null)
            return NotFound();
        if(portfolio.ArtistId!=existingArtist.Id)
            return BadRequest();
        _dbContext.ArtistPortfolioPieces.Remove(portfolio);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet]
    [Authorize("write:artist")]
    [Route("Onboard")]
    public async Task<IActionResult> PaymentAccountStatus()
    {
        var userId = User.GetUserId();
        var existingArtist = await _dbContext.UserArtists.FirstOrDefaultAsync(Artist=>Artist.UserId==userId);
        if (existingArtist == null)
        {
            var ArtistRequest = await _dbContext.ArtistRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(ArtistRequest!=null)
                return BadRequest();
            return BadRequest();
        }
        
        if(existingArtist.Suspended)
            return BadRequest();
        var result = _paymentService.ArtistAccountIsOnboarded(existingArtist.StripeAccountId);
        return Ok(new ArtistOnboardStatusModel(){ Onboarded= result });
    }
    
    [HttpGet]
    [Authorize("write:artist")]
    [Route("Onboard/Url")]
    public async Task<IActionResult> GetPaymentAccount()
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
        if(existingArtist.Suspended)
            return BadRequest();
        if(existingArtist.StripeAccountId==null)
            return BadRequest();

        var result = _paymentService.CreateArtistAccountOnboardingUrl(existingArtist.StripeAccountId);
        return Ok(new ArtistOnboardUrlModel()
        {
            OnboardUrl = result
        });
    }
    
}