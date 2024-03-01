using Microsoft.AspNetCore.Mvc;

namespace comissions.app.database.Models.Request;

public class RequestCreateModel
{
    [FromForm]public int ArtistId { get; set; }
    [FromForm]public string Message { get; set; }
    [FromForm]public decimal Amount { get; set; }
}