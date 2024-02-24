using comissions.app.api.Models.Artist;
using comissions.app.api.Models.PortfolioModel;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
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

        if (seller.ArtistPageSettings == null)
        {
            var newSettings = new ArtistPageSettings()
            {
                ArtistId = seller.Id,
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
            var result = dbSettings.ToModel();
            result.Artist = seller.ToModel();   
            return Ok(result);
        }
        else
        {
            var result = seller.ArtistPageSettings.ToModel();
            result.Artist = seller.ToModel();   
            return Ok(result);
        }
        
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