namespace comissions.app.api.Models.Discovery;

public class DiscoveryArtistModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SocialMeidaLink1 { get; set; }
    public string SocialMeidaLink2 { get; set; }
    public string SocialMeidaLink3 { get; set; }
    public string SocialMeidaLink4 { get; set; }
    public string Description { get; set; }
    public bool PrepaymentRequired { get; set; }
    public double? AverageRating { get; set; }
    public int? ReviewCount { get; set; }
    public string RequestGuidelines { get; set; }
}