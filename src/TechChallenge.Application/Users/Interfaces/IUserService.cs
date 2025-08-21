using OperationResult;
using TechChallenge.Application.Users.Commands;
using TechChallenge.Domain.Dto;

namespace TechChallenge.Application.Users.Interfaces;

public interface IUserService : IService
{
    Task<UserDto?> Find(Guid id);
    Task<IEnumerable<UserDto>> Find();
    Task<Result<Guid>> CreateAsync(CreateUserRequest request);
}