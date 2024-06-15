using System.ComponentModel.DataAnnotations.Schema;

namespace comissions.app.api.Entities;

public class ArtistPageSettings
{
    public int Id { get; set; }
    
    [ForeignKey("Artist")]
    public int ArtistId { get; set; }
    public virtual UserArtist Artist { get; set; } = null!;
    
    public string RequestButtonHoverBGColor { get; set; }
    public string RequestButtonHoverTextColor { get; set; }
    public string RequestButtonTextColor { get; set; }
    public string RequestButtonBGColor { get; set; }
    public string RequestTermsColor { get; set; }
    public string RequestBackgroundColor { get; set; }
    public string RequestHeaderImageUrl { get; set; }
    public bool RequestHeaderUseImage { get; set; }
    public int RequestHeaderSize { get; set; }
    public string RequestHeaderColor { get; set; }
    public string RequestHeaderText { get; set; }
    public int PortfolioMaximumSize { get; set; }
    public bool PortfolioEnabledScrolling { get; set; }
    public int PortfolioColumns { get; set; }
    public bool PortfolioMasonry { get; set; }
    public string PortfolioBackgroundColor { get; set; }
    public string PortfolionHeaderImageUrl { get; set; }
    public bool PortfolionHeaderUseImage { get; set; }
    public int PortfolionHeaderSize { get; set; }
    public string PortfolionHeaderColor { get; set; }
    public string PortfolionHeaderText { get; set; }
    public int DescriptionTextSize { get; set; }
    public string DescriptionTextColor { get; set; }
    public string DescriptionBackgroundColor { get; set; }
    public string DescriptionHeaderImageUrl { get; set; }
    public bool DescriptionHeaderUseImage { get; set; }
    public int DescriptionHeaderSize { get; set; }
    public string DescriptionHeaderColor { get; set; }
    public string DescriptionHeaderText { get; set; }
    public string HeaderImageUrl { get; set; }
    public bool HeaderUseImage { get; set; }
    public int HeaderTextSize { get; set; }
    public string HeaderColor { get; set; }
    public string BackgroundColor { get; set; }
}