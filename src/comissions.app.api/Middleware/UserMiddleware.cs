using System.Security.Claims;
using comissions.app.api.Services.Payment;
using comissions.app.database;
using comissions.app.database.Entities;
using Microsoft.EntityFrameworkCore;
using Novu;
using Novu.Interfaces;
using Novu.DTO;
using Novu.DTO.Subscribers;

namespace comissions.app.api.Middleware;


public class UserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly NovuClient _client;
    public UserMiddleware(RequestDelegate next, NovuClient client)
    {
        _next = next;
        _client = client;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, IPaymentService paymentService)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await dbContext.Users.Include(x=>x.UserArtist).FirstOrDefaultAsync(x=>x.Id==userId);

            if (user == null)
            {
                var displayName = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Name)?.Value ?? "Anonymous";
                    displayName = $"{displayName}#{Guid.NewGuid().ToString().Substring(0, 4)}";
                user = new User
                {
                    Id = userId, 
                    DisplayName = displayName, 
                    Biography = string.Empty,
                    Email = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value ?? string.Empty,
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
            else 
            {   
                user.Email= context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value ?? string.Empty;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
            }

            var newSubscriberDto = new SubscriberCreateData()
            {
                SubscriberId = userId, //replace with system_internal_user_id
                FirstName = user.DisplayName,
                LastName = "",
                Email = user.Email
            };
            var subscriber = await _client.Subscriber.Create(newSubscriberDto);
            
            if (user.Suspended)
            {
                if (user.UnsuspendDate < DateTime.UtcNow)
                {
                    user.Suspended = false;
                    user.SuspendedDate = null;
                    user.UnsuspendDate = null;
                    user.SuspendedReason = null;
                    user.SuspendAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.SuspendedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UnsuspendDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Suspended on {suspendDate} until {unsuspendDate} for {user.SuspendedReason} by {user.SuspendAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            if (user.Banned)
            {
                if (user.UnsuspendDate < DateTime.UtcNow)
                {
                    user.Banned = false;
                    user.BannedDate = null;
                    user.BannedDate = null;
                    user.BannedReason = null;
                    user.BanAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.BannedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UnbanDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {user.BannedReason} by {user.BanAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            if (user.UserArtist != null && user.UserArtist.Suspended)
            {
                if (user.UserArtist.UnsuspendDate < DateTime.UtcNow)
                {
                    user.UserArtist.Suspended = false;
                    user.UserArtist.SuspendedDate = null;
                    user.UserArtist.UnsuspendDate = null;
                    user.UserArtist.SuspendedReason = null;
                    user.UserArtist.SuspendAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.UserArtist.SuspendedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UserArtist.UnsuspendDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {user.UserArtist.SuspendedReason} by {user.UserArtist.SuspendAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        await _next(context);
    }
}