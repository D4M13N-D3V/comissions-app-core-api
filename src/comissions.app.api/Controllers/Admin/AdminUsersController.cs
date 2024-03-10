using comissions.app.api.Extensions;
using comissions.app.database;
using comissions.app.database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

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
            .Where(x=>x.DisplayName.Contains(search) || x.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        return Ok(users);
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
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);

        if (user == null)
            return NotFound();
        
        return Ok(user);
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
            AdminId = User.GetUserId(),
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
            AdminId = User.GetUserId(),
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