using comissions.app.api.Models.Artist;
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
    [Route("Artists")]
    public async Task<IActionResult> GetArtists(string search="",int offset = 0, int pageSize = 10)
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
    [Route("Artists/{sellerName}/Page")]
    public async Task<IActionResult> GetArtistPage(string sellerName)
    {
        var seller = await _dbContext.UserArtists
            .Include(x=>x.ArtistPageSettings)
            .FirstOrDefaultAsync(x=>x.Name==sellerName.Replace('-', ' '));
        if(seller==null)
            return NotFound();
        var result = seller.ArtistPageSettings.ToModel();
        result.Artist = seller.ToModel();   
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artists/{sellerId:int}/Portfolio")]
    public async Task<IActionResult> GetArtistPortfolio(int sellerId, int offset = 0, int pageSize = 10)
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