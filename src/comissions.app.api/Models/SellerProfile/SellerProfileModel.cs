namespace comissions.app.api.Models.SellerProfile;

public class SellerProfileModel
{
    public int Id { get; set; }
    public List<string> SocialMediaLinks { get; set; }
    public string Biography { get; set; }
    public bool PrepaymentRequired { get; set; }
}