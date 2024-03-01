using comissions.app.api.Extensions;
using comissions.app.api.Models.Artist;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novu;

namespace comissions.app.api.Controllers;

public class ArtistPaymentController:Controller
{
    
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;

    public ArtistPaymentController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
    {
        _client = client;
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
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
        var pendingBalance = _paymentService.GetPendingBalance(Artist.StripeAccountId);
        var result = new PayoutModel()
        {
            Enabled = account.PayoutsEnabled,
            Balance = balance,
            PendingBalance = pendingBalance,
            PayoutUrl =  _paymentService.CreateDashboardUrl(Artist.StripeAccountId)
        };
        return Ok(result);
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