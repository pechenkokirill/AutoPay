using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using Refit;

namespace AutoPay.UI.Common;

public interface IRefitUsers
{
    [Post("/api/users/authenticate")]
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationCredentialsRequest authenticationCredentialsRequest);
    
    [Post("/api/users/register")]
    Task<UserResponse> RegisterAsync(RegistrationRequest registrationRequest);
    
    [Get("/api/users")]
    Task<IEnumerable<UserResponse>> GetUsersAsync();
    
    [Post("/api/users/edit/{userId}")]
    Task<UserResponse> EditUsersAsync(Guid userId, UserInformationRequest userInformationRequest);
    
    [Delete("/api/users/{userId}")]
    Task DeleteUserAsync(Guid userId);
}