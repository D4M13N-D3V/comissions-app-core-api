using comissions.app.api.Entities;

namespace comissions.app.api.Models;

public class ArtistAccessRequestMessage
{
    public string SenderId { get; set; }
    public string Message { get; set; }
    public DateTime SentDate { get; set; }
}
public static class ArtistAccessRequestMessageExtensions
{
    public static ArtistAccessRequestMessage ToModel(this ArtistRequestMessage message)
    {
        return new ArtistAccessRequestMessage()
        {
            SenderId = message.UserId,
            Message = message.Message,
            SentDate = message.SentDate
        };
    }
}