namespace comissions.app.database.Models.Request;

public static class RequestModelExtensions
{
    public static RequestImageModel ToModel(this Entities.RequestAsset requestAsset)
    {
        return new RequestImageModel()
        {
            Id = requestAsset.Id,
            FileReference = requestAsset.FileReference
        };
    }
    public static RequestImageModel ToModel(this Entities.RequestReference requestAsset)
    {
        return new RequestImageModel()
        {
            Id = requestAsset.Id,
            FileReference = requestAsset.FileReference
        };
    }
    public static RequestModel ToModel(this Entities.Request sellerProfile)
    {
        return new RequestModel()
        {
            Id = sellerProfile.Id,
            UserId = sellerProfile.UserId,
            RequestDate = sellerProfile.RequestDate,
            AcceptedDate = sellerProfile.AcceptedDate,
            Accepted = sellerProfile.Accepted,
            Amount = sellerProfile.Amount,
            Completed = sellerProfile.Completed,
            CompletedDate = sellerProfile.CompletedDate,
            Declined = sellerProfile.Declined,
            DeclinedDate = sellerProfile.DeclinedDate,
            Paid = sellerProfile.Paid,
            PaidDate = sellerProfile.PaidDate,
            PaymentUrl = sellerProfile.PaymentUrl,
            Message = sellerProfile.Message,
            Reviewed = sellerProfile.Reviewed,
            ReviewDate = sellerProfile.ReviewDate,
            ReviewMessage = sellerProfile.ReviewMessage,
            ReviewRating = sellerProfile.Rating,
        };
    }
}