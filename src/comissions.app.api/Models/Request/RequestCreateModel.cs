namespace comissions.app.database.Models.Request;

public class RequestCreateModel
{
    public string ArtistDisplayName { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
}