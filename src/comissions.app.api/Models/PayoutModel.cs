namespace comissions.app.database.Models;

public class PayoutModel
{
    public int PayoutDelayDays { get; set; }
    public string Interval { get; set; }
    public double Balance { get; set; }
    public bool Enabled { get; set; }
    public string PayoutUrl { get; set; }
    public double PendingBalance { get; set; }
}