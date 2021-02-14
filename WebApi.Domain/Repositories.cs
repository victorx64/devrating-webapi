using System.Collections.Generic;

namespace DevRating.WebApi.Domain
{
    public interface Repositories
    {
        IEnumerable<string> Repositories(string organization);
    }
}