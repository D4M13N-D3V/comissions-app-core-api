namespace comissions.app.database.Entities;

public class ArtistRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public bool Accepted { get; set; }
    public bool Reviewed { get; set; }
    public string? Review { get; set; }
    public double? ReviewRating { get; set; }
    
    public virtual User User { get; set; } = null!;
}