using comissions.app.api.Models.SellerProfileRequest;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.api.Services.Payment;
using comissions.app.database;
using comissions.app.database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminSellerRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;

    public AdminSellerRequestsController(ApplicationDbContext dbContext, IPaymentService paymentService)
    {
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
    [Authorize("read:seller-profile-request")]
    public async Task<IActionResult> GetSellerRequests(int offset = 0, int pageSize = 10)
    {
        var requests = _dbContext.SellerProfileRequests.Skip(offset).Take(pageSize).ToList();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    /// <summary>
    /// Gets the amount of requests there are from users to become a seller.
    /// </summary>
    /// <returns>The number of requests.</returns>
    [HttpGet]
    [Authorize("read:seller-profile-request")]
    [Route("Count")]
    public async Task<IActionResult> GetSellerRequestsCount()
    {
        var result = _dbContext.SellerProfileRequests.Count();
        return Ok(result);
    }
    
    /// <summary>
    /// Accepts a request to become a seller from a user.
    /// </summary>
    /// <param name="userId">The ID of the user to accept the request for.</param>
    /// <returns>The new seller profile.</returns>
    [HttpPut]
    [Authorize("write:seller-profile-request")]
    [Route("{userId}")]
    public async Task<IActionResult> AcceptSellerRequest(string userId)
    {
        var request = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        
        if(request==null)
            return NotFound("No request for that user exists.");
        
        if (request.Accepted == true)
            return BadRequest("User is already a seller.");

        request.Accepted = true;
        request.AcceptedDate = DateTime.UtcNow;
        var accountId = _paymentService.CreateSellerAccount();
        var newSellerProfile = new UserSellerProfile()
        {
            UserId = userId,
            AgeRestricted = false,
            Description = string.Empty,
            StripeAccountId = accountId,
            SocialMediaLink1 = "",
            SocialMediaLink2 = "",
            SocialMediaLink3 = "",
            SocialMediaLink4 = "",
            Name = "Default Shop",
            SellerProfilePageSettings = new SellerProfilePageSettings(){
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
            }
        };
        _dbContext.UserSellerProfiles.Add(newSellerProfile);
        request = _dbContext.SellerProfileRequests.Update(request).Entity;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        return Ok(result);
    }
}