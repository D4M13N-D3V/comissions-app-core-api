using comissions.app.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class AdminDatabaseController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminDatabaseController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateDatabase()
    {
        await _dbContext.Database.MigrateAsync();
        return Ok();
    }
}