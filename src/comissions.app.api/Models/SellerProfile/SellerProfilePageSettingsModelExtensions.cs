using comissions.app.database.Entities;

namespace comissions.app.api.Models.SellerProfile;

public static class SellerProfilePageSettingsModelExtensions
{
    
    public static SellerProfilePageSettingsModel ToModel(this SellerProfilePageSettings sellerProfile)
    {
        return new SellerProfilePageSettingsModel()
        {
            BackgroundColor = sellerProfile.BackgroundColor,
            HeaderColor = sellerProfile.HeaderColor,
            HeaderTextSize = sellerProfile.HeaderTextSize,
            HeaderUseImage = sellerProfile.HeaderUseImage,
            HeaderImageUrl = sellerProfile.HeaderImageUrl,
            DescriptionHeaderColor = sellerProfile.DescriptionHeaderColor,
            DescriptionHeaderSize = sellerProfile.DescriptionHeaderSize,
            DescriptionHeaderUseImage = sellerProfile.DescriptionHeaderUseImage,
            DescriptionHeaderImageUrl = sellerProfile.DescriptionHeaderImageUrl,
            DescriptionBackgroundColor = sellerProfile.DescriptionBackgroundColor,
            DescriptionTextColor = sellerProfile.DescriptionTextColor,
            DescriptionTextSize = sellerProfile.DescriptionTextSize,
            PortfolionHeaderText = sellerProfile.PortfolionHeaderText,
            PortfolionHeaderColor = sellerProfile.PortfolionHeaderColor,
            PortfolionHeaderSize = sellerProfile.PortfolionHeaderSize,
            PortfolionHeaderUseImage = sellerProfile.PortfolionHeaderUseImage,
            PortfolionHeaderImageUrl = sellerProfile.PortfolionHeaderImageUrl,
            PortfolioBackgroundColor = sellerProfile.PortfolioBackgroundColor,
            PortfolioMasonry = sellerProfile.PortfolioMasonry,
            PortfolioColumns = sellerProfile.PortfolioColumns,
            PortfolioEnabledScrolling = sellerProfile.PortfolioEnabledScrolling,
            PortfolioMaximumSize = sellerProfile.PortfolioMaximumSize,
            RequestHeaderText = sellerProfile.RequestHeaderText,
            RequestHeaderColor = sellerProfile.RequestHeaderColor,
            RequestHeaderSize = sellerProfile.RequestHeaderSize,
            RequestHeaderUseImage = sellerProfile.RequestHeaderUseImage,
            RequestHeaderImageUrl = sellerProfile.RequestHeaderImageUrl,
            RequestBackgroundColor = sellerProfile.RequestBackgroundColor,
            RequestTermsColor = sellerProfile.RequestTermsColor,
            RequestButtonBGColor = sellerProfile.RequestButtonBGColor,
            RequestButtonTextColor = sellerProfile.RequestButtonTextColor,
            RequestButtonHoverBGColor = sellerProfile.RequestButtonHoverBGColor,
            RequestButtonHoverTextColor = sellerProfile.RequestButtonHoverTextColor
        };
    }
    public static SellerProfilePageSettings ToModel(this SellerProfilePageSettingsModel sellerProfile, SellerProfilePageSettings existing)
    {
            existing.BackgroundColor = sellerProfile.BackgroundColor;
            existing.HeaderColor = sellerProfile.HeaderColor;
            existing.HeaderTextSize = sellerProfile.HeaderTextSize;
            existing.HeaderUseImage = sellerProfile.HeaderUseImage;
            existing.HeaderImageUrl = sellerProfile.HeaderImageUrl;
            existing.DescriptionHeaderColor = sellerProfile.DescriptionHeaderColor;
            existing.DescriptionHeaderSize = sellerProfile.DescriptionHeaderSize;
            existing.DescriptionHeaderUseImage = sellerProfile.DescriptionHeaderUseImage;
            existing.DescriptionHeaderImageUrl = sellerProfile.DescriptionHeaderImageUrl;
            existing.DescriptionBackgroundColor = sellerProfile.DescriptionBackgroundColor;
            existing.DescriptionTextColor = sellerProfile.DescriptionTextColor;
            existing.DescriptionTextSize = sellerProfile.DescriptionTextSize;
            existing.PortfolionHeaderText = sellerProfile.PortfolionHeaderText;
            existing.PortfolionHeaderColor = sellerProfile.PortfolionHeaderColor;
            existing.PortfolionHeaderSize = sellerProfile.PortfolionHeaderSize;
            existing.PortfolionHeaderUseImage = sellerProfile.PortfolionHeaderUseImage;
            existing.PortfolionHeaderImageUrl = sellerProfile.PortfolionHeaderImageUrl;
            existing.PortfolioBackgroundColor = sellerProfile.PortfolioBackgroundColor;
            existing.PortfolioMasonry = sellerProfile.PortfolioMasonry;
            existing.PortfolioColumns = sellerProfile.PortfolioColumns;
            existing.PortfolioEnabledScrolling = sellerProfile.PortfolioEnabledScrolling;
            existing.PortfolioMaximumSize = sellerProfile.PortfolioMaximumSize;
            existing.RequestHeaderText = sellerProfile.RequestHeaderText;
            existing.RequestHeaderColor = sellerProfile.RequestHeaderColor;
            existing.RequestHeaderSize = sellerProfile.RequestHeaderSize;
            existing.RequestHeaderUseImage = sellerProfile.RequestHeaderUseImage;
            existing.RequestHeaderImageUrl = sellerProfile.RequestHeaderImageUrl;
            existing.RequestBackgroundColor = sellerProfile.RequestBackgroundColor;
            existing.RequestTermsColor = sellerProfile.RequestTermsColor;
            existing.RequestButtonBGColor = sellerProfile.RequestButtonBGColor;
            existing.RequestButtonTextColor = sellerProfile.RequestButtonTextColor;
            existing.RequestButtonHoverBGColor = sellerProfile.RequestButtonHoverBGColor;
            existing.RequestButtonHoverTextColor = sellerProfile.RequestButtonHoverTextColor;
            return existing;
    }
}