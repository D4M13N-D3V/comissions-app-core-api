namespace comissions.app.api.Models.Artist;

public class ArtistStatsModel
{
    public int AcceptedRequests { get; set; }
    public int DeclinedRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int PendingRequests { get; set; }
    public decimal Revenue { get; set; }
}