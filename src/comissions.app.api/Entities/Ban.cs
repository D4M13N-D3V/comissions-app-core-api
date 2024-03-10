using System.ComponentModel.DataAnnotations.Schema;

namespace comissions.app.database.Entities;

public class Ban
{
    public int Id { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public DateTime BanDate { get; set; }
    public DateTime UnbanDate { get; set; }
    public bool Voided { get; set; } = false;
    public string Reason { get; set; }
    public virtual User User { get; set; }
}