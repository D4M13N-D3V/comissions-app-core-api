using comissions.app.api.Entities;
using comissions.app.api.Extensions;
using comissions.app.api.Models.Request;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Novu;
using Novu.DTO.Events;
using Stripe.Checkout;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/Requests")]
public class CustomerRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly NovuClient _client;
    private readonly string _webHookSecret;

    private static readonly string[] AllowedImageContentTypes =
        { "image/jpeg", "image/png", "image/gif", "image/webp" };

    private static bool IsAllowedImageContentType(string? contentType)
        => contentType != null && AllowedImageContentTypes.Contains(contentType.Split(';')[0].Trim().ToLowerInvariant());

    public CustomerRequestsController(ApplicationDbContext dbContext, NovuClient client, IPaymentService paymentService, IStorageService storageService, IConfiguration configuration)
    {
        _client = client;
        _webHookSecret = configuration.GetValue<string>("Stripe:WebHookSecret");
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
    }
    
    [Route("PaymentWebhook")]
    [HttpPost   ]
    [AllowAnonymous]
    public async Task<IActionResult> ProcessWebhookEvent()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        // If you are testing your webhook locally with the Stripe CLI you
        // can find the endpoint's secret by running `stripe listen`
        // Otherwise, find your endpoint's secret in your webhook settings
        // in the Developer Dashboard
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webHookSecret);
        }
        catch (StripeException)
        {
            // Invalid signature / payload — reject rather than throwing an unhandled 500.
            return BadRequest();
        }

        if (stripeEvent.Type == Events.CheckoutSessionExpired)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            var requestId = session?.LineItems?.FirstOrDefault()?.Price?.Product?.Name;
            if (!int.TryParse(requestId, out var expiredRequestId))
                return Ok();
            var request = await _dbContext.Requests
                .Include(x=>x.Artist)
                .Include(x=>x.User)
                .FirstOrDefaultAsync(x=>x.Id==expiredRequestId);
            if (request != null && request.Accepted && !request.Declined && !request.Completed &&
                request.Artist.StripeAccountId == connectedAccountId)
            {
                var paymentUrl = _paymentService.Charge(request.Id,request.Artist.StripeAccountId,request.Amount);
                request.PaymentUrl = paymentUrl;
                _dbContext.Entry(request).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }
        else if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            if (session?.Metadata == null || !session.Metadata.TryGetValue("orderId", out var orderId)
                || !int.TryParse(orderId, out var completedRequestId))
                return Ok();
            var request = await _dbContext.Requests
                .Include(x=>x.Artist)
                .FirstOrDefaultAsync(x=>x.Id==completedRequestId);

            // Unknown request, or event came from a different connected account — ignore.
            if (request == null || request.Artist.StripeAccountId != connectedAccountId)
                return Ok();

            // Idempotency: if already marked paid, do nothing (Stripe may redeliver events).
            if (request.Paid)
                return Ok();

            // Verify the amount actually paid matches what we expect before marking paid.
            var expectedAmount = (long)Math.Round(request.Amount * 100m);
            if (session.AmountTotal != expectedAmount)
            {
                Console.WriteLine($"Webhook amount mismatch for request {request.Id}: " +
                                  $"expected {expectedAmount}, got {session.AmountTotal}.");
                return Ok();
            }

            request.Paid = true;
            request.PaidDate = DateTime.UtcNow;
            _dbContext.Entry(request).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            // All other Stripe event types are not acted on here.
            Console.WriteLine("Unhandled Stripe event type: {0}", stripeEvent.Type);
        }
        return Ok();
    }
    
    
    #region Customer
    [Authorize("read:request")]
    [HttpGet]
    [Route("Customer")]
    public async Task<IActionResult> GetRequests(string search = "", int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var query = _dbContext.Requests
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Artist.Name.Contains(search) || x.Message.Contains(search));
        }

        var requests = await query
            .OrderByDescending(x => x.Id) // Sort by Id in descending order
            .Include(x => x.Artist)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

        var result = requests.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }

    
    [HttpGet]
    [Route("Customer/Count")]
    public async Task<IActionResult> GetRequestCount(string search="")
    {
        var userId = User.GetUserId();
        var query = _dbContext.Requests
            .Where(x => x.UserId == userId);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Artist.Name.Contains(search) || x.Message.Contains(search));
        }

        var result = query.Count();
        return Ok(result);
    }

    
    [Authorize("read:request")]
    [HttpGet]
    [Route("Customer/{requestId:int}")]
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
    [Route("Customer/{requestId:int}/Payment")]
    public async Task<IActionResult> PaymentUrl(int requestId)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .Include(x=>x.Artist)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        if(request.PaymentUrl==null)
            request.PaymentUrl = _paymentService.Charge(request.Id,request.Artist.StripeAccountId,request.Amount);
        _dbContext.Entry(request).State = EntityState.Modified;
        _dbContext.SaveChanges();
        return Ok(new {paymentUrl = request.PaymentUrl});
    }
    
    [Authorize("write:request")]
    [HttpPut]
    [Route("Customer/{requestId:int}/Review")]
    public async Task<IActionResult> ReviewRequest(int requestId, RequestReviewModel model)
    {
        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        
        if(request.Completed==false || request.Accepted==false || request.Reviewed )
            return BadRequest("Request has not been completed or accepted or has already been reviewed.");
        
        request.Reviewed = true;
        request.ReviewDate = DateTime.UtcNow;
        request.ReviewMessage = model.Message;
        request.Rating = model.Rating;
        _dbContext.Entry(request).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        return Ok(result);
    }
    
    
    [HttpGet]
    [Route("Customer/{requestId:int}/References")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetReferences(int requestId)
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
    [Route("Customer/{requestId:int}/References/Count")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetReferencesCount(int requestId)
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
    [Route("Customer/{requestId:int}/References/{referenceId:int}")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetReferenceImage(int requestId, int referenceId)
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
        return new FileStreamResult(content, "application/octet-stream");
    }
    
    [HttpPost]
    [Route("Customer/{requestId:int}/References")]
    [Authorize("write:request")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> AddReference(int requestId)
    {
        if (!IsAllowedImageContentType(Request.ContentType))
            return BadRequest("Only image uploads (jpeg, png, gif, webp) are allowed.");

        var userId = User.GetUserId();
        var request = await _dbContext.Requests
            .Where(x=>x.UserId==userId)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();

        if (request.Accepted || request.Declined)
            return BadRequest("Request has already been accepted or declined.");
        
        var references = await _dbContext.RequestReferences
            .Where(x=>x.RequestId==requestId)
            .ToListAsync();
        if(references.Count>=10)
            return BadRequest("You can only add 10 references to a request.");
        
        var url = await _storageService.UploadImageAsync(HttpContext.Request.Body, Guid.NewGuid().ToString());
        var requestReference = new RequestReference()
        {
            RequestId = request.Id,
            FileReference = url
        };
        _dbContext.RequestReferences.Add(requestReference);
        await _dbContext.SaveChangesAsync();
        var result = requestReference.ToModel();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Customer/{requestId:int}/Assets")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetAssets(int requestId)
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
    [Route("Customer/{requestId:int}/Assets/Count")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetAssetsCount(int requestId)
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
    [Route("Customer/{requestId:int}/Assets/{referenceId:int}")]
    [Authorize("read:request")]
    public async Task<IActionResult> GetAssetImage(int requestId, int referenceId)
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
        return new FileStreamResult(content, "application/octet-stream");
    }
    #endregion
    
    
    

    [Authorize("write:request")]
    [HttpPost]
    [Route("Request")]
    public async Task<IActionResult> CreateRequest(RequestCreateModel model)
    {
        var openRequests = await _dbContext.Requests
            .Where(x=>x.UserId==User.GetUserId() && x.Declined==false && x.Completed==false)
            .CountAsync();
        
        var artist = await _dbContext.UserArtists.FirstOrDefaultAsync(x=>x.Id==model.ArtistId);
        if(artist==null)
            return NotFound("Artist not found.");
        
        if(openRequests>=3)
            return BadRequest("You can only have 3 open requests at a time.");
        var userId = User.GetUserId();
        var request = new Request()
        {
            Amount = model.Amount,
            Message = model.Message,
            RequestDate = DateTime.UtcNow,
            UserId = userId,
            ArtistId = model.ArtistId,
            Accepted = false,
            AcceptedDate = null,
            Declined = false,
            DeclinedDate = null,
            Completed = false,
            CompletedDate = null
        };
        var dbRequest = _dbContext.Requests.Add(request).Entity;
        await _dbContext.SaveChangesAsync();
        var newArtistTriggerModel = new EventCreateData()
        {
            EventName = "requestcreatedbuyer",
            To =
            {
                SubscriberId = userId,
            },
            Payload = { }
        };
        
        
        await _client.Event.Trigger(newArtistTriggerModel);
        var newTriggerModel = new EventCreateData()
        {
            EventName = "requestcreatedartist",
            To =
            {
                SubscriberId = artist.UserId,
            },
            Payload = { }
        };
        await _client.Event.Trigger(newTriggerModel);
        return Ok(request.ToModel());
    }
}