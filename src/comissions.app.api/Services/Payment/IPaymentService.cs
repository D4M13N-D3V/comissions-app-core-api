
namespace comissions.app.api.Services.Payment;

public interface IPaymentService
{
    public string CreateCustomer();
    string CreateArtistAccount();
    string CreateArtistAccountOnboardingUrl(string accountId); 
    bool ArtistAccountIsOnboarded(string accountId);
    string Charge(int orderArtistServiceId, string? sellerStripeAccountId, double orderPrice);
    string CreateDashboardUrl(string accountId);
}