using comissions.app.database.Entities;

namespace comissions.app.database.Models;

public class PenaltyModel
{
    public DateTime Date { get; set; }
    public string Reason { get; set; }
}

public static class PenaltyModelExtensions
{
    public static PenaltyModel ToModel(this Ban ban)
    {
        return new PenaltyModel()
        {
            Date = ban.BanDate,
            Reason = ban.Reason,
        };
    }
    
    public static PenaltyModel ToModel(this Suspension suspension)
    {
        return new PenaltyModel()
        {
            Date = suspension.SuspensionDate,
            Reason = suspension.Reason,
        };
    }
}