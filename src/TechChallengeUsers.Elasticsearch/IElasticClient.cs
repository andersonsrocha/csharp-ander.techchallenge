using Elastic.Clients.Elasticsearch;

namespace TechChallengeUsers.Elasticsearch;

public interface IElasticClient<T>
{
    Task<IReadOnlyCollection<T>> Get(int page, int size, IndexName index);
    Task<bool> AddOrUpdate(T cropLog, IndexName index);
}