namespace comissions.app.api.Models.SellerService;

public static class SellerServiceModelExtensions
{
    
    public static SellerServiceModel ToModel(this database.Entities.SellerService sellerProfileRequest)
    {
        double avgRating = 0;
        int reviewCount = 0;
        return new SellerServiceModel()
        {
            Id = sellerProfileRequest.Id,
            Name = sellerProfileRequest.Name,
            Description = sellerProfileRequest.Description,
            Price = sellerProfileRequest.Price,
            AverageRating = avgRating,
            NumberOfRatings = reviewCount
        };
    }
}