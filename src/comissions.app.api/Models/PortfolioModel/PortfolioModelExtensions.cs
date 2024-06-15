using comissions.app.api.Entities;

namespace comissions.app.api.Models.PortfolioModel;

public static class PortfolioModelExtensions
{
    public static PortfolioModel ToModel(this ArtistPortfolioPiece sellerProfileRequest)
    {
        return new PortfolioModel()
        {
            Id = sellerProfileRequest.Id
        };
    }
}