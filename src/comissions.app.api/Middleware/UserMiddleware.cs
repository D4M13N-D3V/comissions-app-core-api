using System.Security.Claims;
using comissions.app.api.Entities;
using Microsoft.EntityFrameworkCore;
using Novu;
using Novu.DTO.Subscribers;

namespace comissions.app.api.Middleware;


public class UserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly NovuClient _client;
    private readonly ILogger<UserMiddleware> _logger;

    public UserMiddleware(RequestDelegate next, NovuClient client, ILogger<UserMiddleware> logger)
    {
        _next = next;
        _client = client;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Authenticated principal without a subject claim — nothing to load.
                await _next(context);
                return;
            }

            var user = await dbContext.Users
                .Include(x => x.Bans)
                .Include(x => x.Suspensions)
                .FirstOrDefaultAsync(x => x.Id == userId);

            var email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            if (user == null)
            {
                var displayName = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                if (await dbContext.Users.AnyAsync(x => x.DisplayName == displayName))
                    displayName = $"{displayName}#{Guid.NewGuid().ToString().Substring(0, 4)}";
                user = new User
                {
                    Id = userId,
                    DisplayName = displayName,
                    Biography = string.Empty,
                    Email = email,
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                // Provision the notification subscriber only on first sight of the user,
                // not on every request. Failures must not take down the request pipeline.
                await TrySyncSubscriberAsync(user);
            }
            else if (user.Email != email)
            {
                // Only write to the database when something actually changed.
                user.Email = email;
                await dbContext.SaveChangesAsync();
            }

            var suspension = user.Suspensions.FirstOrDefault(x => x.UnsuspensionDate > DateTime.UtcNow && x.Voided == false);
            if (suspension != null)
            {
                var suspendDate = suspension.SuspensionDate.ToString("MM/dd/yyyy");
                var unsuspendDate = suspension.UnsuspensionDate.ToString("MM/dd/yyyy");
                // Status code must be set before the body is written; once the response
                // has started the headers are flushed and the status can no longer change.
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Suspended on {suspendDate} until {unsuspendDate} for {suspension.Reason}.");
                return;
            }

            var ban = user.Bans.FirstOrDefault(x => x.UnbanDate > DateTime.UtcNow && x.Voided == false);
            if (ban != null)
            {
                var suspendDate = ban.BanDate.ToString("MM/dd/yyyy");
                var unsuspendDate = ban.UnbanDate.ToString("MM/dd/yyyy");
                // Status code must be set before the body is written (see suspension branch above).
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {ban.Reason}.");
                return;
            }
        }

        await _next(context);
    }

    private async Task TrySyncSubscriberAsync(User user)
    {
        try
        {
            await _client.Subscriber.Create(new SubscriberCreateData()
            {
                SubscriberId = user.Id,
                FirstName = user.DisplayName,
                LastName = "",
                Email = user.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync Novu subscriber for user {UserId}", user.Id);
        }
    }
}
