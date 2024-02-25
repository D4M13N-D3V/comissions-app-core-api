using Stripe;

namespace comissions.app.api.Services.Payment;

public class StripePaymentServiceProvider:IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _baseUiUrl;
    
    
    public StripePaymentServiceProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = _configuration.GetValue<string>("Stripe:ApiKey");
        StripeConfiguration.ApiKey = _apiKey;
        _baseUiUrl = _configuration.GetValue<string>("UI:BaseUrl");
    }

    public string CreateCustomer()
    {
        var options = new CustomerCreateOptions
        {
            Name = "Jenny Rosen",
            Email = "jennyrosen@example.com",
        };
        var service = new CustomerService();
        var customer = service.Create(options);
        return customer.Id;
    }
    
    // public string ChargeCustomer(string customerId, int amount)
    // {
    //     var options = new PaymentIntentCreateOptions
    //     {
    //         Amount = amount,
    //         Currency = "usd",
    //         AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
    //         {
    //             Enabled = true,
    //         },
    //     };
    //     var requestOptions = new RequestOptions
    //     {
    //         StripeAccount = "{{CONNECTED_ACCOUNT_ID}}",
    //     };
    //     var service = new PaymentIntentService();
    //     var intent = service.Create(options, requestOptions);
    //     throw new NotImplementedException();
    // }
    public string CreateArtistAccount()
    {
        var accountCreateOptions = new AccountCreateOptions { Type = "express",
            Capabilities
                = new AccountCapabilitiesOptions
                {
                    CardPayments
                        = new AccountCapabilitiesCardPaymentsOptions { Requested
                            = true },
                    Transfers
                        = new AccountCapabilitiesTransfersOptions { Requested
                            = true },
                } };
        var accountService = new AccountService();
        var account = accountService.Create(accountCreateOptions);
        return account.Id;
    }
    
    public string CreateArtistAccountOnboardingUrl(string accountId)
    {
        var options = new AccountLinkCreateOptions
        {
            Account = accountId,
            RefreshUrl = $"{_baseUiUrl}/dashboard",
            ReturnUrl = $"{_baseUiUrl}/dashboard",
            Type = "account_onboarding",
        };
        var service = new AccountLinkService();
        var url = service.Create(options);
        return url.Url;
    }
    
    public bool ArtistAccountIsOnboarded(string accountId)
    {
        var service = new AccountService();
        var account = service.Get(accountId);
        return account.Requirements.CurrentlyDue.Count == 0 && account.ChargesEnabled==true && account.DetailsSubmitted==true;
    }

    public string Charge(int requestId, string? sellerStripeAccountId,
        double requestAmount)
    {
        var feeAmount = (long)Math.Round((requestAmount*0.05) * 100);
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions> {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round(requestAmount * 100),
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Skeb-Like Service",
                            },
                        },
                        Quantity = 1,
                    },
                },
            PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    ApplicationFeeAmount = feeAmount,
                },
            Mode = "payment",
            SuccessUrl = $"{_baseUiUrl}/dashboard/requests",
            CancelUrl = $"{_baseUiUrl}/dashboard/requests",
            Metadata = new Dictionary<string, string>()
            {
                ["orderId"] = requestId.ToString()
            }
        };
        var requestOptions = new RequestOptions
        {
            StripeAccount = sellerStripeAccountId
        };
        var service = new Stripe.Checkout.SessionService();
        var session = service.Create(options, requestOptions);
        return session.Url;
    }

    public string CreateDashboardUrl(string accountId)
    {
        var service = new LoginLinkService();
        var url = service.Create(accountId);
        return url.Url;
    }

    public Account GetAccount(string? artistStripeAccountId)
    {
        var AccountService = new AccountService();
        var account = AccountService.Get(artistStripeAccountId);
        return account;
    }
    
    public double GetBalance(string accountId)
    {
        var balanceService = new BalanceService();
        var balance = balanceService.Get(new RequestOptions()
        {
            StripeAccount = accountId
        });
        return balance.Available[0].Amount/100;
    }
}