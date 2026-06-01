
using Stripe;

namespace comissions.app.api.Services.Payment;

public interface IPaymentService
{
    public string CreateCustomer();
    string CreateArtistAccount();
    string CreateArtistAccountOnboardingUrl(string accountId); 
    bool ArtistAccountIsOnboarded(string accountId);
    string Charge(int orderArtistServiceId, string? sellerStripeAccountId, decimal orderPrice);
    string CreateDashboardUrl(string accountId);
    Account GetAccount(string? artistStripeAccountId);
    decimal GetBalance(string accountId);
    decimal GetPendingBalance(string accountId);
}