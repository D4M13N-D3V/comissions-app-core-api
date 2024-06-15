namespace comissions.app.api.Entities;

public class ArtistRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public bool Accepted { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ArtistRequestMessage> ArtistRequestMessages { get; set; } = null!;
}