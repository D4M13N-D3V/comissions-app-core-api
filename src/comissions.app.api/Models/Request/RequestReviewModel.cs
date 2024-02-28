namespace comissions.app.database.Models.Request;

public class RequestReviewModel
{
    public int? RequestId { get; set; }
    public string Message { get; set; }
    public double Rating { get; set; }
    public DateTime? ReviewDate { get; set; }
}