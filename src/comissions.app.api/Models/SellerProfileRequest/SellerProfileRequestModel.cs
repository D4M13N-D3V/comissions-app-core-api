using comissions.app.api.Models.User;

namespace comissions.app.api.Models.ArtistRequest;

public class ArtistRequestModel
{
    public int Id { get; set; }
    public DateTime RequestDate { get; set; }
    public string UserId { get; set; }
    public bool Accepted { get; set; }
    
    public virtual UserInfoModel User { get; set; } = null!;
    public string Message { get; set; }
}