using System.ComponentModel.DataAnnotations.Schema;

namespace comissions.app.database.Entities;

public record UserArtist
{
    public int Id { get; set; }
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string RequestGuidelines { get; set; }
    public string SocialMediaLink1 { get; set; }
    public string SocialMediaLink2 { get; set; }
    public string SocialMediaLink3 { get; set; }
    public string SocialMediaLink4 { get; set; }
    public bool AgeRestricted { get; set; }
    public string? StripeAccountId { get; set; }
    public bool PrepaymentRequired { get; set; } = false;
    public virtual User User { get; set; } = null!;
    
    public int ArtistPageSettingsId { get; set; }
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ArtistPageSettings ArtistPageSettings { get; set; } = null!;
    public virtual ICollection<ArtistPortfolioPiece> PortfolioPieces { get; set; } = new List<ArtistPortfolioPiece>();
}