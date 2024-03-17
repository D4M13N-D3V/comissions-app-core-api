using comissions.app.api.Models.User;

namespace comissions.app.api.Models.ArtistRequest;

public static class ArtistRequestModelExtensions
{
    public static ArtistRequestModel ToModel(this database.Entities.ArtistRequest sellerProfileRequest)
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