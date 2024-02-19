namespace comissions.app.api.Models.ArtistRequest;

public class ArtistRequestModel
{
    public int Id { get; set; }
    public DateTime RequestDate { get; set; }
    public string UserId { get; set; }
    public bool Accepted { get; set; }
    
    public virtual database.Entities.User User { get; set; } = null!;
}