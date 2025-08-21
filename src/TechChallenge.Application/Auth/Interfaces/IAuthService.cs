using OperationResult;
using TechChallenge.Application.Auth.Commands;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Auth.Interfaces;

public interface IAuthService : IService
{
    public Task<Result<LoginDto>> Login(LoginRequest request);
}