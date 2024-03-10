namespace comissions.app.database.Entities;

public class ArtistRequestMessage
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime SentDate { get; set; }
    public string UserId { get; set; }
    public virtual User User { get; set; }
}