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
            Name = sellerProfile.Name,
            SocialMeidaLink1 = sellerProfile.SocialMediaLink1,
            SocialMeidaLink2 = sellerProfile.SocialMediaLink2,
            SocialMeidaLink3 = sellerProfile.SocialMediaLink3,
            SocialMeidaLink4 = sellerProfile.SocialMediaLink4,
            Description = sellerProfile.Description,
            RequestGuidelines = sellerProfile.RequestGuidelines,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    
    public static DiscoverySellerModel ToDiscoveryModelWithoutReviews(this UserSellerProfile sellerProfile)
    {
        return new DiscoverySellerModel()
        {
            Name = sellerProfile.User.DisplayName,
            Id = sellerProfile.Id,
            SocialMeidaLink1 = sellerProfile.SocialMediaLink1,
            SocialMeidaLink2 = sellerProfile.SocialMediaLink2,
            SocialMeidaLink3 = sellerProfile.SocialMediaLink3,
            SocialMeidaLink4 = sellerProfile.SocialMediaLink4,
            Description = sellerProfile.Description,
            RequestGuidelines = sellerProfile.RequestGuidelines,
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
            SocialMeidaLink1 = sellerProfile.SocialMediaLink1,
            SocialMeidaLink2 = sellerProfile.SocialMediaLink2,
            SocialMeidaLink3 = sellerProfile.SocialMediaLink3,
            SocialMeidaLink4 = sellerProfile.SocialMediaLink4,
            Description = sellerProfile.Description,
            RequestGuidelines = sellerProfile.RequestGuidelines,
            PrepaymentRequired = sellerProfile.PrepaymentRequired,
            AverageRating =reviewAverage,
            ReviewCount = reviews.Count()
        };
    }
    public static UserSellerProfile ToModel(this SellerProfileModel sellerProfile, UserSellerProfile existingSellerProfile)
    {
        existingSellerProfile.Name = sellerProfile.Name;
        existingSellerProfile.SocialMediaLink1 = sellerProfile.SocialMeidaLink1;
        existingSellerProfile.SocialMediaLink2 = sellerProfile.SocialMeidaLink2;
        existingSellerProfile.SocialMediaLink3 = sellerProfile.SocialMeidaLink3;
        existingSellerProfile.SocialMediaLink4 = sellerProfile.SocialMeidaLink4;
        existingSellerProfile.Description = sellerProfile.Description;
        existingSellerProfile.RequestGuidelines = sellerProfile.RequestGuidelines;
        existingSellerProfile.PrepaymentRequired = sellerProfile.PrepaymentRequired;
        return existingSellerProfile;
    }
}