using System.Data;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerEntities : Entities
    {
        private readonly Works _works;
        private readonly Ratings _ratings;
        private readonly Authors _authors;

        public SqlServerEntities(IDbConnection connection)
            : this(new SqlServerWorks(connection),
                new SqlServerRatings(connection),
                new SqlServerAuthors(connection))
        {
        }

        public SqlServerEntities(Works works, Ratings ratings, Authors authors)
        {
            _works = works;
            _ratings = ratings;
            _authors = authors;
        }

        public Works Works()
        {
            return _works;
        }

        public Ratings Ratings()
        {
            return _ratings;
        }

        public Authors Authors()
        {
            return _authors;
        }
    }
}