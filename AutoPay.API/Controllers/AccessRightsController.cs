using AutoPay.API.DB;
using AutoPay.Common.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoPay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/rights")]
public class AccessRightsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public AccessRightsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<AccessRightResponse>>> GetUserAccessRights(Guid userId)
    {
        var user = await _dataContext.Users.Include(x => x.AccessRights).FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return BadRequest();
        }

        var rights = user.AccessRights.Select(x => new AccessRightResponse {Right = x.Type});
        return new JsonResult(rights);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessRightResponse>>> GetAllAccessRights()
    {
        var rights = await _dataContext.AccessRights.ToListAsync();
        var rightsResponse = rights.Select(x => new AccessRightResponse
        {
            Right = x.Type
        });

        return new JsonResult(rightsResponse);
    }

    [HttpGet("current")]
    public async Task<ActionResult<IEnumerable<AccessRightResponse>>> GetCurrentAccessRights()
    {
        var userId = Guid.Parse(User.FindFirst("id")!.Value);
        var user = await _dataContext.Users.Include(x => x.AccessRights).FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return BadRequest();
        }

        var rights = user.AccessRights.Select(x => new AccessRightResponse {Right = x.Type});
        return new JsonResult(rights);
    }
}