using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Entities;

[PrimaryKey(nameof(Id))]
public record User
{
    public string Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Biography { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    public int? UserArtistId { get; set; }
    public string AdminNotes { get; set; }
    [JsonIgnore] public virtual UserArtist? UserArtist { get; set; }
    [JsonIgnore] public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    [JsonIgnore] public virtual ICollection<Suspension> Suspensions { get; set; } = new List<Suspension>();
    [JsonIgnore] public virtual ICollection<Ban> Bans { get; set; } = new List<Ban>();
}