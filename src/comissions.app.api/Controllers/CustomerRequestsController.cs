using comissions.app.api.Extensions;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Enums;
using comissions.app.database.Models.Request;
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
        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webHookSecret);

        if (stripeEvent.Type == Events.CheckoutSessionExpired)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            var requestId = session.LineItems.First().Price.Product.Name;
            var request = await _dbContext.Requests
                .Include(x=>x.Artist)
                .Include(x=>x.User)
                .FirstOrDefaultAsync(x=>x.Id==int.Parse(requestId));
            if (request != null && request.Accepted && !request.Declined && !request.Completed &&
                request.Artist.StripeAccountId == connectedAccountId)
            {
                var paymentUrl = _paymentService.Charge(request.Id,request.Artist.StripeAccountId,Convert.ToDouble(request.Amount));
                request.PaymentUrl = paymentUrl;
            }
        }
        else if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            var requestId = session.Metadata["orderId"];
            var request = await _dbContext.Requests
                .Include(x=>x.Artist)
                .FirstOrDefaultAsync(x=>x.Id==int.Parse(requestId));
                    if (request.Artist.StripeAccountId == connectedAccountId)
                    {
                        request.Paid = true;
                        request.PaidDate = DateTime.UtcNow;
                    }
                    _dbContext.Entry(request).State = EntityState.Modified;
                    _dbContext.SaveChanges();
        }
                else if (stripeEvent.Type == Events.AccountUpdated)
                {
                }
                else if (stripeEvent.Type == Events.AccountApplicationAuthorized)
                {
                }
                else if (stripeEvent.Type == Events.AccountApplicationDeauthorized)
                {
                }
                else if (stripeEvent.Type == Events.AccountExternalAccountCreated)
                {
                }
                else if (stripeEvent.Type == Events.AccountExternalAccountDeleted)
                {
                }
                else if (stripeEvent.Type == Events.AccountExternalAccountUpdated)
                {
                }
                else if (stripeEvent.Type == Events.ApplicationFeeCreated)
                {
                }
                else if (stripeEvent.Type == Events.ApplicationFeeRefunded)
                {
                }
                else if (stripeEvent.Type == Events.ApplicationFeeRefundUpdated)
                {
                }
                else if (stripeEvent.Type == Events.BalanceAvailable)
                {
                }
                else if (stripeEvent.Type == Events.BillingPortalConfigurationCreated)
                {
                }
                else if (stripeEvent.Type == Events.BillingPortalConfigurationUpdated)
                {
                }
                else if (stripeEvent.Type == Events.BillingPortalSessionCreated)
                {
                }
                else if (stripeEvent.Type == Events.CapabilityUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CashBalanceFundsAvailable)
                {
                }
                else if (stripeEvent.Type == Events.ChargeCaptured)
                {
                }
                else if (stripeEvent.Type == Events.ChargeExpired)
                {
                }
                else if (stripeEvent.Type == Events.ChargeFailed)
                {
                }
                else if (stripeEvent.Type == Events.ChargePending)
                {
                }
                else if (stripeEvent.Type == Events.ChargeRefunded)
                {
                }
                else if (stripeEvent.Type == Events.ChargeSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.ChargeUpdated)
                {
                }
                else if (stripeEvent.Type == Events.ChargeDisputeClosed)
                {
                }
                else if (stripeEvent.Type == Events.ChargeDisputeCreated)
                {
                }
                else if (stripeEvent.Type == Events.ChargeDisputeFundsReinstated)
                {
                }
                else if (stripeEvent.Type == Events.ChargeDisputeFundsWithdrawn)
                {
                }
                else if (stripeEvent.Type == Events.ChargeDisputeUpdated)
                {
                }
                else if (stripeEvent.Type == Events.ChargeRefundUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CheckoutSessionAsyncPaymentFailed)
                {
                }
                else if (stripeEvent.Type == Events.CheckoutSessionAsyncPaymentSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                }
                else if (stripeEvent.Type == Events.CheckoutSessionExpired)
                {
                }
                else if (stripeEvent.Type == Events.ClimateOrderCanceled)
                {
                }
                else if (stripeEvent.Type == Events.ClimateOrderCreated)
                {
                }
                else if (stripeEvent.Type == Events.ClimateOrderDelayed)
                {
                }
                else if (stripeEvent.Type == Events.ClimateOrderDelivered)
                {
                }
                else if (stripeEvent.Type == Events.ClimateOrderProductSubstituted)
                {
                }
                else if (stripeEvent.Type == Events.ClimateProductCreated)
                {
                }
                else if (stripeEvent.Type == Events.ClimateProductPricingUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CouponCreated)
                {
                }
                else if (stripeEvent.Type == Events.CouponDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CouponUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CreditNoteCreated)
                {
                }
                else if (stripeEvent.Type == Events.CreditNoteUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CreditNoteVoided)
                {
                }
                else if (stripeEvent.Type == Events.CustomerCreated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CustomerUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerDiscountCreated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerDiscountDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CustomerDiscountUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSourceCreated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSourceDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSourceExpiring)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSourceUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionPaused)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionPendingUpdateApplied)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionPendingUpdateExpired)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionResumed)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionTrialWillEnd)
                {
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerTaxIdCreated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerTaxIdDeleted)
                {
                }
                else if (stripeEvent.Type == Events.CustomerTaxIdUpdated)
                {
                }
                else if (stripeEvent.Type == Events.CustomerCashBalanceTransactionCreated)
                {
                }
                else if (stripeEvent.Type == Events.FileCreated)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountCreated)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountDeactivated)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountDisconnected)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountReactivated)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountRefreshedBalance)
                {
                }
                else if (stripeEvent.Type == Events.FinancialConnectionsAccountRefreshedTransactions)
                {
                }
                else if (stripeEvent.Type == Events.IdentityVerificationSessionCanceled)
                {
                }
                else if (stripeEvent.Type == Events.IdentityVerificationSessionCreated)
                {
                }
                else if (stripeEvent.Type == Events.IdentityVerificationSessionProcessing)
                {
                }
                else if (stripeEvent.Type == Events.IdentityVerificationSessionRequiresInput)
                {
                }
                else if (stripeEvent.Type == Events.IdentityVerificationSessionVerified)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceCreated)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceDeleted)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceFinalizationFailed)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceFinalized)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceMarkedUncollectible)
                {
                }
                else if (stripeEvent.Type == Events.InvoicePaid)
                {
                }
                else if (stripeEvent.Type == Events.InvoicePaymentActionRequired)
                {
                }
                else if (stripeEvent.Type == Events.InvoicePaymentFailed)
                {
                }
                else if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceSent)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceUpcoming)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceUpdated)
                {
                }
                else if (stripeEvent.Type == Events.InvoiceVoided)
                {
                }
                else if (stripeEvent.Type == Events.IssuingAuthorizationCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingAuthorizationUpdated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingCardCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingCardUpdated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingCardholderCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingCardholderUpdated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingDisputeClosed)
                {
                }
                else if (stripeEvent.Type == Events.IssuingDisputeCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingDisputeFundsReinstated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingDisputeSubmitted)
                {
                }
                else if (stripeEvent.Type == Events.IssuingDisputeUpdated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingTokenCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingTokenUpdated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingTransactionCreated)
                {
                }
                else if (stripeEvent.Type == Events.IssuingTransactionUpdated)
                {
                }
                else if (stripeEvent.Type == Events.MandateUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentAmountCapturableUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentCanceled)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentCreated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentPartiallyFunded)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentProcessing)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentRequiresAction)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.PaymentLinkCreated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentLinkUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                }
                else if (stripeEvent.Type == Events.PaymentMethodAutomaticallyUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PaymentMethodDetached)
                {
                }
                else if (stripeEvent.Type == Events.PaymentMethodUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PayoutCanceled)
                {
                }
                else if (stripeEvent.Type == Events.PayoutCreated)
                {
                }
                else if (stripeEvent.Type == Events.PayoutFailed)
                {
                }
                else if (stripeEvent.Type == Events.PayoutPaid)
                {
                }
                else if (stripeEvent.Type == Events.PayoutReconciliationCompleted)
                {
                }
                else if (stripeEvent.Type == Events.PayoutUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PersonCreated)
                {
                }
                else if (stripeEvent.Type == Events.PersonDeleted)
                {
                }
                else if (stripeEvent.Type == Events.PersonUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PlanCreated)
                {
                }
                else if (stripeEvent.Type == Events.PlanDeleted)
                {
                }
                else if (stripeEvent.Type == Events.PlanUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PriceCreated)
                {
                }
                else if (stripeEvent.Type == Events.PriceDeleted)
                {
                }
                else if (stripeEvent.Type == Events.PriceUpdated)
                {
                }
                else if (stripeEvent.Type == Events.ProductCreated)
                {
                }
                else if (stripeEvent.Type == Events.ProductDeleted)
                {
                }
                else if (stripeEvent.Type == Events.ProductUpdated)
                {
                }
                else if (stripeEvent.Type == Events.PromotionCodeCreated)
                {
                }
                else if (stripeEvent.Type == Events.PromotionCodeUpdated)
                {
                }
                else if (stripeEvent.Type == Events.QuoteAccepted)
                {
                }
                else if (stripeEvent.Type == Events.QuoteCanceled)
                {
                }
                else if (stripeEvent.Type == Events.QuoteCreated)
                {
                }
                else if (stripeEvent.Type == Events.QuoteFinalized)
                {
                }
                else if (stripeEvent.Type == Events.RadarEarlyFraudWarningCreated)
                {
                }
                else if (stripeEvent.Type == Events.RadarEarlyFraudWarningUpdated)
                {
                }
                else if (stripeEvent.Type == Events.RefundCreated)
                {
                }
                else if (stripeEvent.Type == Events.RefundUpdated)
                {
                }
                else if (stripeEvent.Type == Events.ReportingReportRunFailed)
                {
                }
                else if (stripeEvent.Type == Events.ReportingReportRunSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.ReviewClosed)
                {
                }
                else if (stripeEvent.Type == Events.ReviewOpened)
                {
                }
                else if (stripeEvent.Type == Events.SetupIntentCanceled)
                {
                }
                else if (stripeEvent.Type == Events.SetupIntentCreated)
                {
                }
                else if (stripeEvent.Type == Events.SetupIntentRequiresAction)
                {
                }
                else if (stripeEvent.Type == Events.SetupIntentSetupFailed)
                {
                }
                else if (stripeEvent.Type == Events.SetupIntentSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.SigmaScheduledQueryRunCreated)
                {
                }
                else if (stripeEvent.Type == Events.SourceCanceled)
                {
                }
                else if (stripeEvent.Type == Events.SourceChargeable)
                {
                }
                else if (stripeEvent.Type == Events.SourceFailed)
                {
                }
                else if (stripeEvent.Type == Events.SourceMandateNotification)
                {
                }
                else if (stripeEvent.Type == Events.SourceRefundAttributesRequired)
                {
                }
                else if (stripeEvent.Type == Events.SourceTransactionCreated)
                {
                }
                else if (stripeEvent.Type == Events.SourceTransactionUpdated)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleAborted)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleCanceled)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleCompleted)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleCreated)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleExpiring)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleReleased)
                {
                }
                else if (stripeEvent.Type == Events.SubscriptionScheduleUpdated)
                {
                }
                else if (stripeEvent.Type == Events.TaxSettingsUpdated)
                {
                }
                else if (stripeEvent.Type == Events.TaxRateCreated)
                {
                }
                else if (stripeEvent.Type == Events.TaxRateUpdated)
                {
                }
                else if (stripeEvent.Type == Events.TerminalReaderActionFailed)
                {
                }
                else if (stripeEvent.Type == Events.TerminalReaderActionSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.TestHelpersTestClockAdvancing)
                {
                }
                else if (stripeEvent.Type == Events.TestHelpersTestClockCreated)
                {
                }
                else if (stripeEvent.Type == Events.TestHelpersTestClockDeleted)
                {
                }
                else if (stripeEvent.Type == Events.TestHelpersTestClockInternalFailure)
                {
                }
                else if (stripeEvent.Type == Events.TestHelpersTestClockReady)
                {
                }
                else if (stripeEvent.Type == Events.TopupCanceled)
                {
                }
                else if (stripeEvent.Type == Events.TopupCreated)
                {
                }
                else if (stripeEvent.Type == Events.TopupFailed)
                {
                }
                else if (stripeEvent.Type == Events.TopupReversed)
                {
                }
                else if (stripeEvent.Type == Events.TopupSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.TransferCreated)
                {
                }
                else if (stripeEvent.Type == Events.TransferReversed)
                {
                }
                else if (stripeEvent.Type == Events.TransferUpdated)
                {
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
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
            request.PaymentUrl = _paymentService.Charge(request.Id,request.Artist.StripeAccountId,Convert.ToDouble(request.Amount));
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
    public async Task<IActionResult> AddReference(int requestId)
    {
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