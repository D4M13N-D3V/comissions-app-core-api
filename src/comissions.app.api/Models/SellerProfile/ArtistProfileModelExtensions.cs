using comissions.app.api.Models.Discovery;
using comissions.app.database.Entities;

namespace comissions.app.api.Models.Artist;

public static class ArtistModelExtensions
{
    public static ArtistModel ToModel(this UserArtist sellerProfile)
    {
        return new ArtistModel()
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
    
    public static DiscoveryArtistModel ToDiscoveryModelWithoutReviews(this UserArtist sellerProfile)
    {
        return new DiscoveryArtistModel()
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
    public static DiscoveryArtistModel ToDiscoveryModel(this UserArtist sellerProfile)
    {
        
        return new DiscoveryArtistModel()
        {
            Name = sellerProfile.User.DisplayName,
            Id = sellerProfile.Id,
            SocialMeidaLink1 = sellerProfile.SocialMediaLink1,
            SocialMeidaLink2 = sellerProfile.SocialMediaLink2,
            SocialMeidaLink3 = sellerProfile.SocialMediaLink3,
            SocialMeidaLink4 = sellerProfile.SocialMediaLink4,
            Description = sellerProfile.Description,
            RequestGuidelines = sellerProfile.RequestGuidelines,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    public static UserArtist ToModel(this ArtistModel sellerProfile, UserArtist existingArtist)
    {
        existingArtist.Name = sellerProfile.Name;
        existingArtist.SocialMediaLink1 = sellerProfile.SocialMeidaLink1;
        existingArtist.SocialMediaLink2 = sellerProfile.SocialMeidaLink2;
        existingArtist.SocialMediaLink3 = sellerProfile.SocialMeidaLink3;
        existingArtist.SocialMediaLink4 = sellerProfile.SocialMeidaLink4;
        existingArtist.Description = sellerProfile.Description;
        existingArtist.RequestGuidelines = sellerProfile.RequestGuidelines;
        existingArtist.PrepaymentRequired = sellerProfile.PrepaymentRequired;
        return existingArtist;
    }
}