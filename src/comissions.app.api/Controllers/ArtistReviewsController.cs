using comissions.app.api.Extensions;
using comissions.app.api.Models.Artist;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novu;

namespace comissions.app.api.Controllers;

[Route("api/Artist")]
public class ArtistReviewsController: Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;

    public ArtistReviewsController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
    {
        _client = client;
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Reviews")]
    public async Task<IActionResult> GetArtistReviews([FromQuery]int offset = 0, [FromQuery]int limit = 10)
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
        var result = Artist.Requests.Where(x=>x.Reviewed).Skip(offset).Take(limit).Select(x=> new RequestReviewModel()
        {
            RequestId = x.Id,
            Message = x.ReviewMessage,
            Rating = x.Rating.Value,
            ReviewDate = x.ReviewDate
        }).ToList();
        
        return Ok(result);
    }
        
    [HttpGet]
    [Authorize("read:artist")]
    [Route("Reviews/Count")]
    public async Task<IActionResult> ReviewCount()
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
        var result = Artist.Requests.Where(x=>x.Reviewed).Select(x=> new RequestReviewModel()
        {
            RequestId = x.Id,
            Message = x.ReviewMessage,
            Rating = x.Rating.Value,
            ReviewDate = x.ReviewDate
        }).ToList().Count;
        
        return Ok(result);
    }
}


// using comissions.app.api.Services.Payment;
// using comissions.app.api.Services.Storage;
// using comissions.app.database;
// using Novu;
//
// namespace comissions.app.api.Controllers;
//[Route("api/Artist")]
// public class ArtistReviews
// {
//     private readonly ApplicationDbContext _dbContext;
//     private readonly IStorageService _storageService;
//     private readonly IPaymentService _paymentService;
//     private readonly NovuClient _client;
//
//     public ArtistReviews(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService, NovuClient client)
//     {
//         _client = client;
//         _paymentService = paymentService;
//         _storageService = storageService;
//         _dbContext = dbContext;
//     }
// }