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
    [Route("Requests")]
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
    [Route("Requests/{requestId:int}")]
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
    
    [Authorize("write:request")]
    [HttpPost]
    [Route("Requests")]
    public async Task<IActionResult> CreateRequest([FromBody] RequestModel model)
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
            RequestDate = DateTime.Now,
            Accepted = false,
            AcceptedDate = null,
            UserId = User.GetUserId(),
            ArtistId = model.ArtistId
        };
        _dbContext.Requests.Add(request);
        await _dbContext.SaveChangesAsync();
        return Ok(request.ToModel());
    }
}