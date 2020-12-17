using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface ContainsKeyOperation
    {
        bool Contains(Id id);
    }
}