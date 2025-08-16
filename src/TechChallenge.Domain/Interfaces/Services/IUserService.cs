using OperationResult;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Requests.Users;

namespace TechChallenge.Domain.Interfaces.Services;

public interface IUserService : IService
{
    Task<UserDto?> Find(Guid id);
    Task<IEnumerable<UserDto>> Find();
    Task<Result<Guid>> CreateAsync(CreateUserRequest request);
}