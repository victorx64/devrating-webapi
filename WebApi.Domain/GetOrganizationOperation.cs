using System.Collections.Generic;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface GetOrganizationOperation
    {
        Organization Organization(Id id);
        Organization Organization(string name);
        IEnumerable<Organization> SubjectOrganizations(string subject);
    }
}