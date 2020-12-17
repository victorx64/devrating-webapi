using System.Data;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerAuthors : Authors
    {
        private readonly GetAuthorOperation _get;
        private readonly InsertAuthorOperation _insert;
        private readonly ContainsAuthorOperation _contains;

        public SqlServerAuthors(IDbConnection connection)
            : this(new SqlServerGetAuthorOperation(connection),
                new SqlServerInsertAuthorOperation(connection),
                new SqlServerContainsAuthorOperation(connection))
        {
        }

        public SqlServerAuthors(GetAuthorOperation get, InsertAuthorOperation insert, ContainsAuthorOperation contains)
        {
            _get = get;
            _insert = insert;
            _contains = contains;
        }

        public GetAuthorOperation GetOperation()
        {
            return _get;
        }

        public InsertAuthorOperation InsertOperation()
        {
            return _insert;
        }

        public ContainsAuthorOperation ContainsOperation()
        {
            return _contains;
        }
    }
}