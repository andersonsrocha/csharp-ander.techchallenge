using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models;

namespace TechChallenge.Data.Repositories;

public class GameRepository(TechChallengeContext context) : Repository<Game>(context), IGameRepository;