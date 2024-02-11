using ArtPlatform.Database.Entities;
using comissions.app.api.Models.Discovery;
using comissions.app.database.Entities;

namespace comissions.app.api.Models.SellerProfile;

public static class SellerProfileModelExtensions
{
    public static SellerProfileModel ToModel(this UserSellerProfile sellerProfile)
    {
        return new SellerProfileModel()
        {
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    public static DiscoverySellerModel ToDiscoveryModel(this UserSellerProfile sellerProfile)
    {
        
        return new DiscoverySellerModel()
        {
            Name = sellerProfile.User.DisplayName,
            Id = sellerProfile.Id,
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired,
            AverageRating = sellerProfile.SellerServices?.Average(x=>x.Reviews.Average(y=>y.Rating)),
            ReviewCount = sellerProfile.SellerServices?.Sum(x=>x.Reviews.Count)
        };
    }
    public static UserSellerProfile ToModel(this SellerProfileModel sellerProfile, UserSellerProfile existingSellerProfile)
    {
        existingSellerProfile.SocialMediaLinks = sellerProfile.SocialMediaLinks;
        existingSellerProfile.Biography = sellerProfile.Biography;
        existingSellerProfile.PrepaymentRequired = sellerProfile.PrepaymentRequired;
        return existingSellerProfile;
    }
}