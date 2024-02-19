namespace comissions.app.database.Models.Request;

public class RequestModel
{
    public int Id { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
    public string UserId { get; set; }
    public DateTime RequestDate { get; set; }
    public bool Accepted { get; set; } = false;
    public DateTime? AcceptedDate { get; set; }
    public bool Declined { get; set; } = false;
    public DateTime? DeclinedDate { get; set; }
    public bool Completed { get; set; } = false;
    public DateTime? CompletedDate { get; set; }
}