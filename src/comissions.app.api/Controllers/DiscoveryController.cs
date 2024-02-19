using comissions.app.api.Models.SellerProfile;
using comissions.app.api.Models.SellerService;
using comissions.app.api.Models.Discovery;
using comissions.app.api.Models.PortfolioModel;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscoveryController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public DiscoveryController(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }
    
    
    [HttpGet]
    [Route("Sellers")]
    public async Task<IActionResult> GetSellers(string search="",int offset = 0, int pageSize = 10)
    {
        var sellers = await _dbContext.UserSellerProfiles
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellers.Select(x=>x.ToDiscoveryModelWithoutReviews()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}")]
    public async Task<IActionResult> GetSeller(int sellerId)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.SellerServices)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var result = seller.ToDiscoveryModel();
        return Ok(result);
    }
    
    
    [HttpGet]
    [Route("Sellers/{sellerName}/Page")]
    public async Task<IActionResult> GetSellerPage(string sellerName)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.SellerProfilePageSettings)
            .FirstOrDefaultAsync(x=>x.Name==sellerName.Replace('-', ' '));
        if(seller==null)
            return NotFound();
        var result = seller.SellerProfilePageSettings;
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Portfolio")]
    public async Task<IActionResult> GetSellerPortfolio(int sellerId, int offset = 0, int pageSize = 10)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.SellerProfilePortfolioPieces
            .Where(x=>x.SellerProfileId==sellerId)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellerPortfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Portfolio/Count")]
    public async Task<IActionResult> GetSellerPortfolioCount(int sellerId)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.SellerProfilePortfolioPieces
            .Where(x=>x.SellerProfileId==sellerId)
            .CountAsync();
        return Ok(sellerPortfolio);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> GetSellerPortfolioPiece(int sellerId, int portfolioId)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.SellerProfilePortfolioPieces
            .FirstOrDefaultAsync(x=>x.Id==portfolioId);
        if(sellerPortfolio==null)
            return NotFound("Portfolio piece not found.");
        
        var content = await _storageService.DownloadImageAsync(sellerPortfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
    
    [HttpGet]
    [Route("Sellers/Count")]
    public async Task<IActionResult> GetSellersCount(string search="")
    {
        var result = await _dbContext.UserSellerProfiles
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .CountAsync();
        return Ok(result);
    }
}