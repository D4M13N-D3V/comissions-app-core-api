using comissions.app.api.Extensions;
using comissions.app.api.Models.Artist;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novu;

namespace comissions.app.api.Controllers;


[Route("api/Artist")]
public class ArtistPageController: Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;

    public ArtistPageController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
    {
        _client = client;
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
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
}