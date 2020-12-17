using System.Data;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerRatings : Ratings
    {
        private readonly InsertRatingOperation _insert;
        private readonly GetRatingOperation _get;
        private readonly ContainsRatingOperation _contains;

        public SqlServerRatings(IDbConnection connection)
            : this(
                new SqlServerInsertRatingOperation(connection),
                new SqlServerGetRatingOperation(connection),
                new SqlServerContainsRatingOperation(connection)
            )
        {
        }

        public SqlServerRatings(InsertRatingOperation insert, GetRatingOperation get, ContainsRatingOperation contains)
        {
            _insert = insert;
            _get = get;
            _contains = contains;
        }

        public InsertRatingOperation InsertOperation()
        {
            return _insert;
        }

        public GetRatingOperation GetOperation()
        {
            return _get;
        }

        public ContainsRatingOperation ContainsOperation()
        {
            return _contains;
        }
    }
}