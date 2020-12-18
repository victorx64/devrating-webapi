using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface ContainsOrganizationOperation
    {
        bool Contains(Id id);
        bool Contains(Id id, string user);
        bool Contains(string name);
    }
}