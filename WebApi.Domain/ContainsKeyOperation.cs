using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface ContainsKeyOperation
    {
        bool Contains(Id id);
        bool Contains(Id id, string organization, bool revoked);
        bool Contains(string organization, string value, bool revoked);
    }
}