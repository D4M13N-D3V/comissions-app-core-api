using comissions.app.api.Extensions;
using comissions.app.database;
using comissions.app.database.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminArtistsController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminArtistsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetArtists([FromQuery]string search="", [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var sellers = await _dbContext.UserArtists.Include(x=>x.User)
            .Include(x=>x.Requests)
            .Include(x=>x.Requests).ThenInclude(x=>x.RequestAssets)
            .Include(x=>x.PortfolioPieces)
            .Where(x=>x.User.DisplayName.Contains(search) || x.User.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellers.Select(x => x.ToAdminArtistModel());
        return Ok(result);
    }
    
    [HttpGet("Count")]
    public async Task<IActionResult> GetArtistsCount([FromQuery]string search="")
    {
        var result = await _dbContext.UserArtists.Include(x=>x.User)
            .Where(x=>x.User.DisplayName.Contains(search) || x.User.Email.Contains(search))
            .CountAsync();
        return Ok(result);
    }
    
    [HttpGet("{sellerId:int}")]
    public async Task<IActionResult> GetArtist(int sellerId)
    {
        var seller = await _dbContext.UserArtists.Include(x=>x.User)
            .Include(x=>x.Requests)
            .Include(x=>x.Requests).ThenInclude(x=>x.RequestAssets)
            .Include(x=>x.PortfolioPieces)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);

        if (seller == null)
            return NotFound();
        var result = seller.ToAdminArtistModel();
        return Ok(result);
    }
    
    
    [HttpPut("{sellerId:int}/Terminate")]
    public async Task<IActionResult> TerminateArtist(int sellerId)
    {
        var seller = _dbContext.UserArtists.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound();

        _dbContext.UserArtists.Remove(seller);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{sellerId:int}/SetBiography")]
    public async Task<IActionResult> SetBiography(int sellerId, [FromBody]string biography)
    {
        var seller = _dbContext.UserArtists.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound();

        seller.Description = biography;
        _dbContext.UserArtists.Update(seller);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}