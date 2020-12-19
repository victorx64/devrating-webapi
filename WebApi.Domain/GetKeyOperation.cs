using System.Collections.Generic;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface GetKeyOperation
    {
        Key Key(Id id);
        IEnumerable<Key> OrganizationKeys(Id organization);
        IEnumerable<Key> OrganizationKeys(string organization);
    }
}