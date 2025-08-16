using OperationResult;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Requests.Auth;

namespace TechChallenge.Domain.Interfaces.Services;

public interface IAuthService : IService
{
    public Task<Result<LoginDto>> Login(LoginRequest request);
}