using comissions.app.api.Extensions;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novu;
using Novu.DTO.Events;

namespace comissions.app.api.Controllers;

[Route("api/Requests")]
public class ArtistRequestsController: Controller
{
    
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;
    private readonly string _webHookSecret;

    public ArtistRequestsController(ApplicationDbContext dbContext, NovuClient client, IPaymentService paymentService, IStorageService storageService, IConfiguration configuration)
    {
        _client = client;
        _webHookSecret = configuration.GetValue<string>("Stripe:WebHookSecret");
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    
    
    [HttpGet]
    [Route("Artist/{requestId:int}/References")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistReferences(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var references = await _dbContext.RequestReferences
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        var result = references.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artist/{requestId:int}/References/Count")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistReferencesCount(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var references = await _dbContext.RequestReferences
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        var result = references.Select(x=>x.ToModel()).Count();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artist/{requestId:int}/References/{referenceId:int}")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistReferenceImage(int requestId, int referenceId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var reference = await _dbContext.RequestReferences
            .Where(x=>x.RequestId==requestId)
            .FirstOrDefaultAsync(x=>x.Id==referenceId);
        if(reference==null)
            return NotFound();
        var content = await _storageService.DownloadImageAsync(reference.FileReference);
        var mimeType = _storageService.GetMimeType(reference.FileReference);
        return new FileStreamResult(content, mimeType);
    }
    
    [HttpGet]
    [Route("Artist/{requestId:int}/Assets")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistAssets(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var references = await _dbContext.RequestAssets
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        var result = references.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artist/{requestId:int}/Assets/Count")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistAssetsCount(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var references = await _dbContext.RequestAssets
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        var result = references.Select(x=>x.ToModel()).Count();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Artist/{requestId:int}/Assets/{referenceId:int}")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetArtistAssetImage(int requestId, int referenceId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var reference = await _dbContext.RequestAssets
            .Where(x=>x.RequestId==requestId)
            .FirstOrDefaultAsync(x=>x.Id==referenceId);
        if(reference==null)
            return NotFound();
        var content = await _storageService.DownloadImageAsync(reference.FileReference);
        var mimeType = _storageService.GetMimeType(reference.FileReference);
        return new FileStreamResult(content, mimeType);
    }
    
    
    [HttpPost]
    [Route("Artist/{requestId:int}/References")]
    [Authorize("write:request")]
    public async Task<IActionResult> AddArtistAsset(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();

        if(request.Accepted==false)
            return BadRequest("Request has not been accepted.");
        
        if(request.Paid==false)
            return BadRequest("Request has not been paid.");
        
        if(request.Completed)
            return BadRequest("Request has already been completed.");
        
        var references = await _dbContext.RequestAssets
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        
        if(references.Count>=10)
            return BadRequest("You can only add 10 assets to a request.");
        
        
        
        var url = await _storageService.UploadImageAsync(HttpContext.Request.Body, Guid.NewGuid().ToString());
        var requestReference = new RequestAsset()
        {
            RequestId = request.Id,
            FileReference = url
        };
        _dbContext.RequestAssets.Add(requestReference);
        await _dbContext.SaveChangesAsync();
        var result = requestReference.ToModel();
        return Ok(result);
    }
    
    
    
    
    
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Artist")]
    public async Task<IActionResult> GetArtistRequests(string search="",int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var query = _dbContext.Requests.Include(x=>x.Artist)
            .Where(x => x.Artist.UserId == userId);


        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Artist.Name.Contains(search) || x.Message.Contains(search));
        }

        var requests = await query
            .Include(x => x.Artist)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

        var result = requests.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Artist/Count")]
    public async Task<IActionResult> GetArtistRequestCount(string search="")
    {
        var userId = User.GetUserId();
        var query = _dbContext.Requests.Include(x=>x.Artist)
            .Where(x => x.Artist.UserId == userId);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Artist.Name.Contains(search) || x.Message.Contains(search));
        }

        var result = query.Count();
        return Ok(result);
    }
    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Artist/{requestId:int}")]
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
    [Route("Artist/{requestId:int}/Complete")]
    public async Task<IActionResult> CompleteRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.RequestAssets)
            .Include(x=>x.RequestReferences)
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        
        if(request.RequestAssets.Count()==0)
            return BadRequest("You must add at least one asset to complete the request.");
        
        if(request.Accepted==false)
            return BadRequest("Request has not been accepted.");

        if (request.Declined)
            return BadRequest("Request has already been declined.");
        
        if(request==null)
            return NotFound();
        
        request.Completed = true;
        request.CompletedDate = DateTime.UtcNow;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        
        var result = request.ToModel();
        var newTriggerModel = new EventCreateData()
        {
            EventName = "requestcompleted",
            To =
            {
                SubscriberId = request.UserId
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        return Ok(result);
    }

    
    [Authorize("write:request")]
    [HttpPut]
    [Route("Artist/{requestId:int}/Accept")]
    public async Task<IActionResult> AcceptRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        
        if(request.Completed)
            return BadRequest("Request has already been completed.");
        
        if(request.Accepted)
            return BadRequest("Request has already been accepted.");

        if (request.Declined)
            return BadRequest("Request has already been declined.");
        
        if(request==null)
            return NotFound();
        var paymentUrl = _paymentService.Charge(request.Id,request.Artist.StripeAccountId,Convert.ToDouble(request.Amount));
        request.Accepted = true;
        request.AcceptedDate = DateTime.UtcNow;
        request.Paid = false;
        request.PaymentUrl = paymentUrl;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        var newTriggerModel = new EventCreateData()
        {
            EventName = "requestacceptedbuyer",
            To =
            {
                SubscriberId = request.UserId
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        var newTriggerArtistModel = new EventCreateData()
        {
            EventName = "requestacceptedartist",
            To =
            {
                SubscriberId = request.Artist.UserId
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        
        var result = request.ToModel();
        return Ok(result);
    }

    
    [Authorize("write:request")]
    [HttpPut]
    [Route("Artist/{requestId:int}/Deny")]
    public async Task<IActionResult> DenyRequest(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.Artist.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        
        if(request.Completed)
            return BadRequest("Request has already been completed.");
        
        if(request.Accepted)
            return BadRequest("Request has already been accepted.");

        if (request.Declined)
            return BadRequest("Request has already been declined.");
        request.Declined = true;
        request.DeclinedDate = DateTime.UtcNow;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        var newTriggerModel = new EventCreateData()
        {
            EventName = "requestdenied",
            To =
            {
                SubscriberId = request.UserId
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        return Ok(result);
    }
}