using comissions.app.api.Models.Artist;
using comissions.app.api.Models.PortfolioModel;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Models.Request;
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
    [Route("Artists")]
    public async Task<IActionResult> GetArtists(string search="", [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var sellers = await _dbContext.UserArtists
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellers.Select(x=>x.ToDiscoveryModelWithoutReviews()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}")]
    public async Task<IActionResult> GetArtist(int sellerId)
    {
        var seller = await _dbContext.UserArtists
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var result = seller.ToDiscoveryModel();
        return Ok(result);
    }
    
    
    [HttpGet]
    [Route("Artists/{sellerName}")]
    public async Task<IActionResult> GetArtistByName(string sellerName)
    {
        
        var seller = await _dbContext.UserArtists.Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.User.DisplayName==sellerName);
        if(seller==null)
            return NotFound();
        var result = seller.ToModel();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}/Reviews")]
    public async Task<IActionResult> GetArtistReviews(int sellerId,  [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerReviews = await _dbContext.Requests
            .Where(x=>x.ArtistId==sellerId && x.Reviewed)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellerReviews.Select(x=>new RequestReviewModel()
        {
            RequestId = x.Id,
            Message = x.ReviewMessage,
            Rating = x.Rating.Value,
            ReviewDate = x.ReviewDate
        }).ToList();
        return Ok(result);
    }
    
    public async Task<IActionResult> GetArtistReviewsCount(int sellerId)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerReviews = await _dbContext.Requests
            .Where(x=>x.ArtistId==sellerId && x.Reviewed)
            .CountAsync();
        return Ok(sellerReviews);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}/Portfolio")]
    public async Task<IActionResult> GetArtistPortfolio(int sellerId,  [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.ArtistPortfolioPieces
            .Where(x=>x.ArtistId==sellerId)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellerPortfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}/Portfolio/Count")]
    public async Task<IActionResult> GetArtistPortfolioCount(int sellerId)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.ArtistPortfolioPieces
            .Where(x=>x.ArtistId==sellerId)
            .CountAsync();
        return Ok(sellerPortfolio);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}/Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> GetArtistPortfolioPiece(int sellerId, int portfolioId)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound();
        var sellerPortfolio = await _dbContext.ArtistPortfolioPieces
            .FirstOrDefaultAsync(x=>x.Id==portfolioId);
        if(sellerPortfolio==null)
            return NotFound("Portfolio piece not found.");
        
        var content = await _storageService.DownloadImageAsync(sellerPortfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
    
    [HttpGet]
    [Route("Artists/Count")]
    public async Task<IActionResult> GetArtistsCount(string search="")
    {
        var result = await _dbContext.UserArtists
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .CountAsync();
        return Ok(result);
    }
}