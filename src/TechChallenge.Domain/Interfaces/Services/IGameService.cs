using OperationResult;
using TechChallenge.Domain.Requests.Games;
using TechChallenge.Domain.Dto;

namespace TechChallenge.Domain.Interfaces.Services;

public interface IGameService : IService
{
    GameDto? Find(Guid id);
    IEnumerable<GameDto> Find();
    Task<Result<Guid>> CreateAsync(CreateGameRequest request);
    Task<Result> UpdateAsync(UpdateGameRequest request);
    Task<Result> DeleteAsync(Guid id);
}