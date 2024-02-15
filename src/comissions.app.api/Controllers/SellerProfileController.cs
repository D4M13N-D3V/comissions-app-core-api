using comissions.app.api.Extensions;
using comissions.app.api.Models.PortfolioModel;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.api.Models.SellerProfile;
using comissions.app.api.Models.SellerProfileRequest;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerProfileController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;


    public SellerProfileController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService)
    {
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("read:seller-profile")]
    public async Task<IActionResult> GetSellerProfile()
    {
        var userId = User.GetUserId();
        var sellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if(sellerProfile==null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        var result = sellerProfile.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> UpdateSellerProfile(SellerProfileModel model)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        var updatedSellerProfile = model.ToModel(existingSellerProfile);
        updatedSellerProfile = _dbContext.UserSellerProfiles.Update(updatedSellerProfile).Entity;
        await _dbContext.SaveChangesAsync();
        var result = updatedSellerProfile.ToModel();
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize("read:seller-profile")]
    [Route("Request")]
    public async Task<IActionResult> GetSellerProfileRequest()
    {
        var userId = User.GetUserId();
        var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if(sellerProfileRequest==null)
            return NotFound();
        var result = sellerProfileRequest.ToModel();
        return Ok(result);
    }   
    
    [HttpPost]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> RequestSellerProfile()
    {
        var userId = User.GetUserId();
        
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile != null)
        {
            return Unauthorized();
        }
        
        var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if (sellerProfileRequest != null)
            return Ok(sellerProfileRequest.ToModel());
        
        sellerProfileRequest = new SellerProfileRequest()
        {
            Accepted = false,
            RequestDate = DateTime.UtcNow,
            UserId = userId
        };
        _dbContext.SellerProfileRequests.Add(sellerProfileRequest);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }   
    [HttpGet]
    [Authorize("read:seller-profile")]
    [Route("{sellerServiceId:int}/Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> GetPortfolio(int sellerServiceId, int portfolioId)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        if(existingSellerProfile.Suspended)
            return BadRequest();

        var portfolio = await _dbContext.SellerProfilePortfolioPieces
            .FirstAsync(x => x.SellerProfileId == existingSellerProfile.Id && x.Id==portfolioId);
        var content = await _storageService.DownloadImageAsync(portfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }

    [HttpGet]
    [Route("Portfolio")]
    [Authorize("read:seller-profile")]
    public async Task<IActionResult> GetPortfolio()
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        if(existingSellerProfile.Suspended)
            return BadRequest();
        var portfolio = await _dbContext.SellerProfilePortfolioPieces.Where(x=>x.SellerProfileId==existingSellerProfile.Id).ToListAsync();
        var result = portfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Portfolio")]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> AddPortfolio(IFormFile file)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }

        if(existingSellerProfile.Suspended)
            return BadRequest();
        var url = await _storageService.UploadImageAsync(file, Guid.NewGuid().ToString());
        var portfolio = new SellerProfilePortfolioPiece()
        {
            SellerProfileId = existingSellerProfile.Id,
            FileReference = url
        };
        portfolio.SellerProfileId = existingSellerProfile.Id;
        _dbContext.SellerProfilePortfolioPieces.Add(portfolio);
        await _dbContext.SaveChangesAsync();
        var result = portfolio.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-profile")]
    [Route("Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> DeletePortfolio(int portfolioId)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        if(existingSellerProfile.Suspended)
            return BadRequest();
        var portfolio = await _dbContext.SellerProfilePortfolioPieces.FirstOrDefaultAsync(x=>x.Id==portfolioId);
        if(portfolio==null)
            return NotFound();
        if(portfolio.SellerProfileId!=existingSellerProfile.Id)
            return BadRequest();
        _dbContext.SellerProfilePortfolioPieces.Remove(portfolio);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet]
    [Authorize("write:seller-profile")]
    [Route("Onboard")]
    public async Task<IActionResult> PaymentAccountStatus()
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        
        if(existingSellerProfile.Suspended)
            return BadRequest();
        if(existingSellerProfile.StripeAccountId!=null)
            return BadRequest();

        var accountId = _paymentService.CreateSellerAccount();
        existingSellerProfile.StripeAccountId = accountId;
        existingSellerProfile = _dbContext.UserSellerProfiles.Update(existingSellerProfile).Entity;
        await _dbContext.SaveChangesAsync();
        var result = _paymentService.SellerAccountIsOnboarded(accountId);
        return Ok(new SellerOnboardStatusModel(){ Onboarded= result });
    }
    
    [HttpGet]
    [Authorize("write:seller-profile")]
    [Route("Onboard/Url")]
    public async Task<IActionResult> GetPaymentAccount()
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest();
            return Unauthorized();
        }
        if(existingSellerProfile.Suspended)
            return BadRequest();
        if(existingSellerProfile.StripeAccountId==null)
            return BadRequest();

        var result = _paymentService.CreateSellerAccountOnboardingUrl(existingSellerProfile.StripeAccountId);
        return Ok(new SellerOnboardUrlModel()
        {
            OnboardUrl = result
        });
    }
    
}