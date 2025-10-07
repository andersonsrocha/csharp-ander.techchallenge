using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OperationResult;
using TechChallengeUsers.Application.Users.Commands;
using TechChallengeUsers.Application.Users.Interfaces;
using TechChallengeUsers.Domain.Dto;
using TechChallengeUsers.Domain.Interfaces;
using TechChallengeUsers.Domain.Models;
using TechChallengeUsers.Security;

namespace TechChallengeUsers.Application.Users.Handlers;

public class UserService(SignInManager<User> signInManager, UserManager<User> userManager, IUserStore<User> userStore, IJwtService jwtService, IUserRepository repository) : IUserService
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public async Task<Result<LoginDto>> Login(LoginUserRequest request)
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
        
        var roles = await signInManager.UserManager.GetRolesAsync(user);
       
        var token = jwtService.Generate(user, roles);
        return Result.Success(new LoginDto(token));
    }

    public async Task<UserDto?> Find(Guid id)
    {
        var user = await repository.Find(id);
        if (user is null)
            return null;

        return new UserDto(user.Id, user.Name, user.Email!);
    }

    public async Task<IEnumerable<UserDto>> Find()
    {
        var users = await repository.Find();
        return users.Select(user => new UserDto(user.Id, user.Name, user.Email!));
    }

    public async Task<Result<Guid>> CreateAsync(CreateUserRequest request)
    {
        if (!userManager.SupportsUserEmail)
            throw new NotSupportedException($"{nameof(UserService)} requires a user store with email support.");

        var user = (User)request;
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = request.Email;
        
        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            return Result.Error<Guid>(new Exception(userManager.ErrorDescriber.InvalidEmail(email).Description));
        
        var passwordIsValid = await userManager.PasswordValidators[0].ValidateAsync(userManager, user, request.Password);
        if(!passwordIsValid.Succeeded)
            return Result.Error<Guid>(new Exception(passwordIsValid.Errors.First().Description));;
            
        await userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Error<Guid>(new Exception(result.Errors.First().Description));

        await userManager.AddToRoleAsync(user, "User");
        return Result.Success(user.Id);
    }
}