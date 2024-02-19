namespace comissions.app.database.Entities;

public class ArtistPortfolioPiece
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public string FileReference { get; set; }
    public virtual UserArtist Artist { get; set; } = null!;
}