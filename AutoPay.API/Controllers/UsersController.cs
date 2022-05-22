using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoPay.API.DB;
using AutoPay.API.Services.Abstractions;
using AutoPay.API.Services.Configuration;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoPay.API.Controllers;

[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IAppConfiguration _configuration;
    private readonly IPasswordHashService _passwordHashService;
    private readonly DataContext _dataContext;

    public UsersController(IAppConfiguration configuration, IPasswordHashService passwordHashService,
        DataContext dataContext)
    {
        _configuration = configuration;
        _passwordHashService = passwordHashService;
        _dataContext = dataContext;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }

        if (user.UserName == "LocalAdmin")
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.DeletingLocalAdmin));
        }

        _dataContext.Users.Remove(user);
        await _dataContext.SaveChangesAsync();
        
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("edit/{userId}")]
    public async Task<ActionResult<UserResponse>> EditUser(Guid userId, [FromBody] UserInformationRequest userInformation)
    {
        var user = await _dataContext.Users.Include(x => x.AccessRights).FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }

        if (user.UserName == "LocalAdmin")
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.EditingLocalAdmin));
        }
        
        user.UserName = userInformation.UserName ?? string.Empty;
        user.FullName = userInformation.FullName ?? string.Empty;
        if (userInformation.Password is not null)
        {
            user.PasswordHash = _passwordHashService.CalculateHash(userInformation.Password);
        }
        
        var isUserWithUserExist = _dataContext.Users.Any(x => x.UserName == user.UserName && x.Id != userId);
        if (isUserWithUserExist)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.AlreadyExist));
        }
        
        var accessRights = await _dataContext.AccessRights.ToListAsync();
        if (!userInformation.IsAdmin)
        {
            var adminRight = accessRights.First(x => x.Type == AccessRightType.Admin);
            accessRights.Remove(adminRight);
        }

        user.AccessRights.Clear();

        foreach (var accessRight in accessRights)
        {
            user.AccessRights.Add(accessRight);
        }

        _dataContext.Users.Update(user);
        await _dataContext.SaveChangesAsync();

        return new UserResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            UserName = user.UserName
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _dataContext.Users.ToListAsync();
        var usersResponse = users.Select(u => new UserResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            UserName = u.UserName,
            PasswordHash = u.PasswordHash,
        });

        return new JsonResult(usersResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegistrationRequest credentialsRequest)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == credentialsRequest.UserName);
        if (user is not null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.AlreadyExist));
        }

        var accessRights = await _dataContext.AccessRights.ToListAsync();
        if (!credentialsRequest.IsAdmin)
        {
            var adminRight = accessRights.First(x => x.Type == AccessRightType.Admin);
            accessRights.Remove(adminRight);
        }
        
        user = new User
        {
            Id = Guid.NewGuid(),
            UserName = credentialsRequest.UserName,
            PasswordHash = _passwordHashService.CalculateHash(credentialsRequest.Password),
            FullName = credentialsRequest.FullName,
            AccessRights = accessRights
        };
        
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName,
            PasswordHash = user.PasswordHash
        };
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate(
        [FromBody] AuthenticationCredentialsRequest credentialsRequest)
    {
        var users = await _dataContext.Users.Include(x => x.AccessRights).ToListAsync();
        var user = users.FirstOrDefault(u =>
            _passwordHashService.ValidateHash(u.PasswordHash, credentialsRequest.Password) && u.UserName == credentialsRequest.UserName);

        if (user is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.UserName),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
        };

        if (user.AccessRights is not null)
        {
            foreach (var userAccess in user.AccessRights)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, userAccess.Type.ToString()));
            }
        }

        var secretKey = _configuration.GetValue(Startup.JwtSecretKey)!;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(null, null, claims, null, null, signingCred);
        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var userResponse = new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName
        };

        return new AuthenticationResponse()
        {
            User = userResponse,
            Token = jwtString
        };
    }
}