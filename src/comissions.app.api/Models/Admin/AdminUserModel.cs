using comissions.app.database.Entities;

namespace comissions.app.database.Models.Admin;

public class AdminUserModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int NumberOfRequests { get; set; } = 0;
    public int NumberOfSuspensions { get; set; } = 0;
    public int NumberOfBans { get; set; } = 0;
    public int NumberOfReviews { get; set; } = 0;
    public decimal AmountSpent { get; set; } = 0;
}

public static class AdminUserModelExtensions
{
    public static AdminUserModel ToAdminUserModel(this User user)
    {
        return new AdminUserModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Biography = user.Biography,
            Email = user.Email,
            NumberOfRequests = user.Requests.Count,
            NumberOfSuspensions = user.Suspensions.Count,
            NumberOfBans = user.Bans.Count,
            NumberOfReviews = user.Requests.Count(x => x.Reviewed),
            AmountSpent = user.Requests.Sum(r => r.Amount)
        };
    }
}
