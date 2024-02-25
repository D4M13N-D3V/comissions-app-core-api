namespace comissions.app.database.Entities;

public class RequestAsset
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public string FileReference { get; set; }
    public virtual Request Request { get; set; } = null!;
}