using comissions.app.api.Entities;
using comissions.app.api.Models.User;

namespace comissions.app.api.Models.SellerProfileRequest;

public static class ArtistRequestModelExtensions
{
    public static ArtistRequestModel ToModel(this ArtistRequest sellerProfileRequest)
    {
        return new ArtistRequestModel()
        {
            Id = sellerProfileRequest.Id,
            UserId = sellerProfileRequest.UserId,
            User = sellerProfileRequest.User.ToModel(),
            RequestDate = sellerProfileRequest.RequestDate,
            Accepted = sellerProfileRequest.Accepted,
            Message = sellerProfileRequest.Message
        };
    }
}