using comissions.app.api.Extensions;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;


    public RequestsController(ApplicationDbContext dbContext, IPaymentService paymentService, IStorageService storageService)
    {
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Customer/Requests")]
    public async Task<IActionResult> GetRequests(string search="",int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var requests = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.Name.Contains(search) || x.Message.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Customer/Requests/{requestId:int}")]
    public async Task<IActionResult> GetRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .Include(x=>x.Artist)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var result = request.ToModel();
        return Ok(result);
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Artist/Requests")]
    public async Task<IActionResult> GetArtistRequests(string search="",int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var requests = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .Where(x=>x.Artist.Name.Contains(search) || x.Message.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Artist/Requests/{requestId:int}")]
    public async Task<IActionResult> GetArtistRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var result = request.ToModel();
        return Ok(result);
    }
    
    [Authorize("write:request")]
    [HttpPut]
    [Route("Artist/Requests/{requestId:int}/Accept")]
    public async Task<IActionResult> AcceptRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        
        request.Accepted = true;
        request.AcceptedDate = DateTime.UtcNow;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        
        var result = request.ToModel();
        return Ok(result);
    }

    
    [Authorize("write:request")]
    [HttpPut]
    [Route("Artist/Requests/{requestId:int}/Deny")]
    public async Task<IActionResult> DenyRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        request.Declined = true;
        request.DeclinedDate = DateTime.UtcNow;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        return Ok(result);
    }

    [Authorize("write:request")]
    [HttpPost]
    [Route("Request")]
    public async Task<IActionResult> CreateRequest([FromBody] RequestCreateModel model)
    {
        var openRequests = await _dbContext.Requests
            .Where(x=>x.UserId==User.GetUserId())
            .CountAsync();
        
        if(openRequests>3)
            return BadRequest("You can only have 3 open requests at a time.");
        
        var request = new Request()
        {
            Amount = model.Amount,
            Message = model.Message,
            RequestDate = DateTime.UtcNow,
            UserId = User.GetUserId(),
            ArtistId = model.ArtistId,
            Accepted = false,
            AcceptedDate = null,
            Declined = false,
            DeclinedDate = null,
            Completed = false,
            CompletedDate = null
        };
        _dbContext.Requests.Add(request);
        await _dbContext.SaveChangesAsync();
        return Ok(request.ToModel());
    }
}