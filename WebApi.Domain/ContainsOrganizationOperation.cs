using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface ContainsOrganizationOperation
    {
        bool Contains(Id id);
        bool Contains(Id id, string subject);
        bool Contains(string name);
        bool Contains(string name, string subject);
    }
}