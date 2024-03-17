using comissions.app.database.Entities;

namespace comissions.app.database.Models.Admin;

public class AdminArtistModel
{
    public int Id { get; set; }
    public bool PrepaymentRequired { get; set; }
    public string SocialMediaLink3 { get; set; }
    public string SocialMediaLink2 { get; set; }
    public string SocialMediaLink1 { get; set; }
    public string Description { get; set; }
    public string SocialMediaLink4 { get; set; }
    public string RequestGuidelines { get; set; }
    public string Name { get; set; }
    public int NumberOfRequests { get; set; }
    public int NumberOfReviews { get; set; }
    public int NumberOfPaid { get; set; }
    public decimal AmountMade { get; set; }
    public decimal FeesCollected { get; set; }
    public int NumberOfAssets { get; set; }
    public int NumberOfPortfolio { get; set; }
    public double? AverageRating  { get; set; }
    public User User { get; set; }
}

public static class AdminArtistModelExtensions
{
    public static AdminArtistModel ToAdminArtistModel(this UserArtist artist)
    {
        return new AdminArtistModel
        {
            Id = artist.Id,
            PrepaymentRequired = artist.PrepaymentRequired,
            SocialMediaLink3 = artist.SocialMediaLink3,
            SocialMediaLink2 = artist.SocialMediaLink2,
            SocialMediaLink1 = artist.SocialMediaLink1,
            Description = artist.Description,
            SocialMediaLink4 = artist.SocialMediaLink4,
            RequestGuidelines = artist.RequestGuidelines,
            Name = artist.Name,
            User = artist.User,
            NumberOfRequests = artist.Requests.Count,
            NumberOfReviews = artist.Requests.Count(x => x.Reviewed),
            NumberOfPaid = artist.Requests.Count(x => x.Paid),
            AmountMade = artist.Requests.Sum(r => r.Amount),
            FeesCollected = artist.Requests.Sum(r => r.Amount)*(decimal)0.15,
            NumberOfAssets = artist.Requests.SelectMany(x=>x.RequestAssets).Count(),
            NumberOfPortfolio = artist.PortfolioPieces.Count,
            AverageRating = artist.Requests.Count(x=>x.Reviewed) == 0 ? 0 : artist.Requests.Where(x=>x.Reviewed).Average(x=>x.Rating)
        };
    }
}