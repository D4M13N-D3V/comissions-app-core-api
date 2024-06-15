using comissions.app.api.Entities;
using comissions.app.api.Models.Admin;
using comissions.app.api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers.Admin;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminUsersController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminUsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery]string search="", [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var users = await _dbContext.Users
            .Include(x=>x.Requests)
            .Include(x=>x.Suspensions)
            .Include(x=>x.Bans)
            .Include(x=>x.Requests).ThenInclude(x=>x.Artist)
            .Where(x=>x.DisplayName.Contains(search) || x.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = users.Select(x => x.ToAdminUserModel());
        return Ok(result);
    }
    
    [HttpGet("{userId}/Requests")]
    public async Task<IActionResult> GetUserRequests(string artistId, [FromQuery]int offset = 0, [FromQuery]int pageSize = 10)
    {
        var requests = await _dbContext.Requests
            .Include(x=>x.Artist)
            .Where(x=>x.UserId==artistId)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = requests.Select(x=>x.ToModel());
        return Ok(result);
    }
    
    [HttpGet("{userId}/Requests/{requestId:int}")]
    public async Task<IActionResult> GetUserRequest(string artistId, int requestId)
    {
        var request = await _dbContext.Requests
            .Include(x=>x.Artist)
            .FirstOrDefaultAsync(x=>x.Id==requestId);
        if(request==null)
            return NotFound();
        var result = request.ToModel();
        return Ok(result);
    }
    
    [HttpGet("{userId}/Requests/Count")]
    public async Task<IActionResult> GetUserRequestsCount(string artistId)
    {
        var result = await _dbContext.Requests
            .Where(x=>x.UserId==artistId)
            .CountAsync();
        return Ok(result);
    }

    
    [HttpGet("Count")]
    public async Task<IActionResult> GetUsersCount([FromQuery]string search="")
    {
        var result = await _dbContext.Users
            .Where(x=>x.DisplayName.Contains(search) || x.Email.Contains(search))
            .CountAsync();
        return Ok(result);
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _dbContext.Users
            .Include(x=>x.Requests)
            .Include(x=>x.Suspensions)
            .Include(x=>x.Bans)
            .Include(x=>x.Requests).ThenInclude(x=>x.Artist)
            .FirstOrDefaultAsync(x=>x.Id==userId);

        if (user == null)
            return NotFound();
        var result = user.ToAdminUserModel();        
        return Ok(result);
    }
    
    
    [HttpPut("{userId}/Suspend")]
    public async Task<IActionResult> SuspendUser(string userId, [FromQuery]string reason, [FromQuery]int days)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        
        var newSuspension = new Suspension()
        {
            UserId = userId,
            Reason = reason,
            SuspensionDate = DateTime.UtcNow,
            UnsuspensionDate = DateTime.UtcNow.AddDays(days),
            Voided = false
        };
        _dbContext.Suspensions.Add(newSuspension);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Unsuspend")]
    public async Task<IActionResult> UnsuspendUser(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        var suspension = await _dbContext.Suspensions.FirstOrDefaultAsync(x=>x.UserId==userId && x.UnsuspensionDate>DateTime.UtcNow);

        if (suspension == null)
            return BadRequest();
        
        suspension.Voided = true;
        _dbContext.Suspensions.Update(suspension);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Ban")]
    public async Task<IActionResult> BanUser(string userId, [FromQuery]string reason, [FromQuery]int days)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        
        var ban = new Ban()
        {
            UserId = userId,
            Reason = reason,
            BanDate = DateTime.UtcNow,
            UnbanDate = DateTime.UtcNow.AddDays(days),
            Voided = false
        };
        _dbContext.Bans.Add(ban);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Unban")]
    public async Task<IActionResult> UnbanUser(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        
        var ban = await _dbContext.Bans.FirstOrDefaultAsync(x=>x.UserId==userId && x.UnbanDate>DateTime.UtcNow);

        if (ban == null)
            return BadRequest();
        
        ban.Voided = true;
        _dbContext.Bans.Update(ban);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/SetDisplayName")]
    public async Task<IActionResult> SetDisplayName(string userId, [FromBody]string displayName)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        
        user.DisplayName = displayName;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/SetBiography")]
    public async Task<IActionResult> SetBiography(string userId, [FromBody]string biography)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound();
        
        user.Biography = biography;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
}