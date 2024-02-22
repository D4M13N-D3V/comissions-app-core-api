namespace comissions.app.database.Models.Request;

public class RequestCreateModel
{
    public int UserId { get; set; }
    public int ArtistId { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
}