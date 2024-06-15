namespace comissions.app.api.Models.Request;

public class RequestCreateModel
{
    public int ArtistId { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
}