using Microsoft.AspNetCore.Identity;
using OperationResult;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces.Services;
using TechChallenge.Domain.Models;
using TechChallenge.Domain.Requests.Auth;
using TechChallenge.Security;

namespace TechChallenge.Application.Auth;

public class AuthService(SignInManager<User> signInManager, IJwtService jwtService) : IAuthService
{
    public async Task<Result<LoginDto>> Login(LoginRequest request)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Error<LoginDto>(new Exception("User not found."));
        
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (result.IsNotAllowed)
            return Result.Error<LoginDto>(new Exception("User not allowed to login."));
        
        if (result.IsLockedOut)
            return Result.Error<LoginDto>(new Exception("User is locked out."));
        
        if (!result.Succeeded)
            return Result.Error<LoginDto>(new Exception("User or password is incorrect."));
        
       
        var token = jwtService.Generate(user);
        return Result.Success(new LoginDto(token));
    }
}