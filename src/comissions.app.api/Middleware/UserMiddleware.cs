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

            var user = await dbContext.Users.Include(x=>x.UserArtist)
                .Include(x=>x.Bans).ThenInclude(x=>x.Admin)
                .Include(x=>x.Suspensions).ThenInclude(x=>x.Admin)
                .FirstOrDefaultAsync(x=>x.Id==userId);

            if (user == null)
            {
                var displayName = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Name)?.Value ?? "Anonymous";
                if(dbContext.Users.Any(x=>x.DisplayName==displayName))
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
            var suspension = user.Suspensions.FirstOrDefault(x => x.UnsuspensionDate > DateTime.UtcNow && x.Voided==false);
            if (suspension!=null)
            {
                var suspendDate = suspension.SuspensionDate.ToString("MM/dd/yyyy");
                var unsuspendDate = suspension.UnsuspensionDate.ToString("MM/dd/yyyy");
                await context.Response.WriteAsync($"Suspended on {suspendDate} until {unsuspendDate} for {suspension.Reason} by {suspension.Admin.DisplayName}.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            var ban = user.Bans.FirstOrDefault(x => x.UnbanDate > DateTime.UtcNow && x.Voided==false);
            if (ban!=null)
            {
                var suspendDate = ban.BanDate.ToString("MM/dd/yyyy");
                var unsuspendDate = ban.UnbanDate.ToString("MM/dd/yyyy");
                await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {ban.Reason} by {ban.Admin.DisplayName}.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }
}