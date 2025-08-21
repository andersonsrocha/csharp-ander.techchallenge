using OperationResult;
using TechChallenge.Application.Games.Commands;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Games.Interfaces;

public interface IGameService : IService
{
    GameDto? Find(Guid id);
    IEnumerable<GameDto> Find();
    Task<Result<Guid>> CreateAsync(CreateGameRequest request);
    Task<Result> UpdateAsync(UpdateGameRequest request);
    Task<Result> DeleteAsync(Guid id);
}