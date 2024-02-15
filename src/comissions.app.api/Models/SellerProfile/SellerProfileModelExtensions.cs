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
            Id = sellerProfile.Id,
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    
    public static DiscoverySellerModel ToDiscoveryModelWithoutReviews(this UserSellerProfile sellerProfile)
    {
        return new DiscoverySellerModel()
        {
            Name = sellerProfile.User.DisplayName,
            Id = sellerProfile.Id,
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired,
        };
    }
    public static DiscoverySellerModel ToDiscoveryModel(this UserSellerProfile sellerProfile)
    {
        var reviews = sellerProfile.SellerServices.SelectMany(x => x.Reviews);
        double reviewAverage = 0;
        if(reviews.Count()>0) reviewAverage = reviews.Average(x=>x.Rating);
        
        return new DiscoverySellerModel()
        {
            Name = sellerProfile.User.DisplayName,
            Id = sellerProfile.Id,
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired,
            AverageRating =reviewAverage,
            ReviewCount = reviews.Count()
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