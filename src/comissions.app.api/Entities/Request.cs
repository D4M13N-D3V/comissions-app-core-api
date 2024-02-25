namespace comissions.app.database.Entities;

public class Request
{
    public int Id { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
    public string UserId { get; set; }
    public int ArtistId { get; set; }
    public DateTime RequestDate { get; set; }
    public bool Accepted { get; set; } = false;
    public DateTime? AcceptedDate { get; set; }
    public bool Declined { get; set; } = false;
    public DateTime? DeclinedDate { get; set; }
    public string? PaymentUrl { get; set; }
    public bool Paid { get; set; } = false;
    public DateTime? PaidDate { get; set; } = null!;
    public bool Completed { get; set; } = false;
    public DateTime? CompletedDate { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual UserArtist Artist { get; set; } = null!;
    
    public virtual ICollection<RequestAsset> RequestAssets { get; set; } = null!;
    public virtual ICollection<RequestReference> RequestReferences { get; set; } = null!;
}