using OperationResult;
using TechChallengeUsers.Application.Users.Commands;
using TechChallengeUsers.Domain.Dto;

namespace TechChallengeUsers.Application.Users.Interfaces;

public interface IUserService : IService
{
    Task<Result<LoginDto>> Login(LoginUserRequest request);
    Task<UserDto?> Find(Guid id);
    Task<IEnumerable<UserDto>> Find();
    Task<Result<Guid>> CreateAsync(CreateUserRequest request);
}