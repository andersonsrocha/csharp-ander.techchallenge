using TechChallenge.Domain.Interfaces.Repositories;
using TechChallenge.Domain.Models;

namespace TechChallenge.Data.Repositories;

public class GameRepository(TechChallengeContext context) : Repository<Game>(context), IGameRepository;