namespace comissions.app.api.Models.Discovery;

public class DiscoverySellerModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> SocialMediaLinks { get; set; }
    public string Biography { get; set; }
    public bool PrepaymentRequired { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}