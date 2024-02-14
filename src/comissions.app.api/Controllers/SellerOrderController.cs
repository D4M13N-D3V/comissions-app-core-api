using comissions.app.api.Extensions;
using comissions.app.api.Models.Order;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerOrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    
    public SellerOrderController(IStorageService storageService, ApplicationDbContext dbContext)
    {
        _storageService = storageService;
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("/api/SellerOrders")]
    [Authorize("read:seller-orders")]
    public async Task<IActionResult> GetOrders(int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var orders = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Where(x => x.Seller.UserId == userId && status==null ? true : status==x.Status)
            .Skip(offset).Take(pageSize).ToListAsync();
        
        var result = orders.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }
    [HttpGet]
    [Route("/api/SellerOrders/{orderId:int}")]
    [Authorize("read:seller-orders")]
    public async Task<IActionResult> GetOrder(int orderId, int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x => x.Seller)
            .FirstAsync(x => x.Id==orderId && x.Seller.UserId == userId && status == null ? true : status == x.Status);
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(x=>x.UserId==userId);
        if(seller==null)
            return NotFound();
        if(seller.Suspended)
            return BadRequest();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound();
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Declined)
            return BadRequest();
        if(order.BuyerId!=userId)
            return BadRequest();
        if(order.Status!=EnumOrderStatus.Completed && order.Status!= EnumOrderStatus.Declined)
            return BadRequest();
        order.Status = EnumOrderStatus.Declined;
        order.EndDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }

    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Accept")]
    public async Task<IActionResult> AcceptOrder(int orderId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        if (seller == null)
            return NotFound();
        if (seller.Suspended)
            return BadRequest();
        var order = await _dbContext.SellerServiceOrders
            .Include(x => x.SellerService)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.Seller.UserId == userId);
        if (order == null)
            return NotFound();
        if (order.Status == EnumOrderStatus.Completed || order.Status == EnumOrderStatus.Declined)
            return BadRequest();
        if (order.BuyerId != userId)
            return BadRequest();
        if (order.Status != EnumOrderStatus.PendingAcceptance)
            return BadRequest();
        if (order.Status == EnumOrderStatus.Declined)
            return BadRequest();
        order.Status = EnumOrderStatus.WaitingForPayment;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }

    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Decline")]
    public async Task<IActionResult> DeclineOrder(int orderId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        if (seller == null)
            return NotFound();
        if (seller.Suspended)
            return BadRequest();
        var order = await _dbContext.SellerServiceOrders
            .Include(x => x.SellerService)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.Seller.UserId == userId);
        if (order == null)
            return NotFound();
        if (order.Status == EnumOrderStatus.Completed || order.Status == EnumOrderStatus.Declined)
            return BadRequest();
        if (order.BuyerId != userId)
            return BadRequest();
        if (order.Status != EnumOrderStatus.PendingAcceptance)
            return BadRequest();
        order.Status = EnumOrderStatus.Declined;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }


    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/CompleteRevision")]
    public async Task<IActionResult> CompleteRevision(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(x=>x.UserId==userId);
        if(seller==null)
            return NotFound();
        if(seller.Suspended)
            return BadRequest();
        if(order==null)
            return NotFound();
        if(order.Seller.UserId!=userId)
            return BadRequest();
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Declined)
            return BadRequest();
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest();
        if(order.Status>EnumOrderStatus.InProgress)
            return BadRequest();
        order.Status = EnumOrderStatus.Completed;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
}