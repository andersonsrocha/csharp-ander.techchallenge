using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OperationResult;
using TechChallengeUsers.Application.Users.Commands;
using TechChallengeUsers.Application.Users.Interfaces;
using TechChallengeUsers.Domain.Dto;
using TechChallengeUsers.Domain.Interfaces;
using TechChallengeUsers.Domain.Models;
using TechChallengeUsers.Elasticsearch;
using TechChallengeUsers.Security;

namespace TechChallengeUsers.Application.Users.Handlers;

public class UserService(SignInManager<User> signInManager, UserManager<User> userManager, IUserStore<User> userStore, IJwtService jwtService, IElasticClient<UserLog> elastic, IUserRepository repository, ILogger<UserService> logger) : IUserService
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public async Task<Result<LoginDto>> Login(LoginUserRequest request)
    {
        logger.LogInformation("Attempting to sign in user with email: {Email}", request.Email);
        var user = await signInManager.UserManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Error<LoginDto>(new Exception("User not found."));
        
        logger.LogInformation("User found. Verifying password.");
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (result.IsNotAllowed)
            return Result.Error<LoginDto>(new Exception("User not allowed to login."));
        
        logger.LogInformation("User is allowed.");
        if (result.IsLockedOut)
            return Result.Error<LoginDto>(new Exception("User is locked out."));
        
        logger.LogInformation("User is not locked out.");
        if (!result.Succeeded)
            return Result.Error<LoginDto>(new Exception("User or password is incorrect."));
        
        logger.LogInformation("Password verified. Retrieving user roles.");
        var roles = await signInManager.UserManager.GetRolesAsync(user);
        
        logger.LogInformation("Roles retrieved. Generate JWT token.");
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
        logger.LogInformation("Creating a new user with email: {Email}", request.Email);
        if (!userManager.SupportsUserEmail)
            throw new NotSupportedException($"{nameof(UserService)} requires a user store with email support.");

        var user = (User)request;
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = request.Email;
        
        logger.LogInformation("Validating email format.");
        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            return Result.Error<Guid>(new Exception(userManager.ErrorDescriber.InvalidEmail(email).Description));
        
        logger.LogInformation("Checking if password is valid.");
        var passwordIsValid = await userManager.PasswordValidators[0].ValidateAsync(userManager, user, request.Password);
        if(!passwordIsValid.Succeeded)
            return Result.Error<Guid>(new Exception(passwordIsValid.Errors.First().Description));;
            
        await userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, request.Password);
    
        if (!result.Succeeded)
            return Result.Error<Guid>(new Exception(result.Errors.First().Description));
        
        logger.LogInformation("User created successfully. Assigning 'User' role.");
        await userManager.AddToRoleAsync(user, "User");
        await elastic.AddOrUpdate(new UserLog(user.Id, user.Name, email), nameof(UserLog).ToLower());
        
        return Result.Success(user.Id);
    }
}