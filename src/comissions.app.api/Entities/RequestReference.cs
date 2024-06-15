namespace comissions.app.api.Entities;

public class RequestReference
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public string FileReference { get; set; }
    public virtual Request Request { get; set; } = null!;
}