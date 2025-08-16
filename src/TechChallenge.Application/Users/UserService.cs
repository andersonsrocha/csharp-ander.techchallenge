using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OperationResult;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces.Services;
using TechChallenge.Domain.Interfaces.Repositories;
using TechChallenge.Domain.Models;
using TechChallenge.Domain.Requests.Users;

namespace TechChallenge.Application.Users;

public class UserService(UserManager<User> userManager, IUserStore<User> userStore, IUserRepository repository) : IUserService
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

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

        var emailStore = (IUserEmailStore<User>)userStore;
        var email = request.Email;
        
        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            return Result.Error<Guid>(new Exception(userManager.ErrorDescriber.InvalidEmail(email).Description));

        var user = (User)request;
        await userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, request.Password);
        
        return !result.Succeeded ? Result.Error<Guid>(new Exception(result.Errors.First().Description)) : Result.Success(user.Id);
    }
}