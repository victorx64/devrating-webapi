using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface ContainsUserOperation
    {
        bool Contains(string foreignId);
        bool Contains(Id id);
    }
}