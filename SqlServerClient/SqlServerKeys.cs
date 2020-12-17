using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerKeys : Keys
    {
        private readonly GetKeyOperation _get;
        private readonly InsertKeyOperation _insert;
        private readonly ContainsKeyOperation _contains;

        public SqlServerKeys(IDbConnection connection)
            : this(
                new SqlServerGetKeyOperation(connection),
                new SqlServerInsertKeyOperation(connection),
                new SqlServerContainsKeyOperation(connection)
            )
        {
        }

        public SqlServerKeys(GetKeyOperation get, InsertKeyOperation insert, ContainsKeyOperation contains)
        {
            _get = get;
            _insert = insert;
            _contains = contains;
        }

        public GetKeyOperation GetOperation()
        {
            return _get;
        }

        public InsertKeyOperation InsertOperation()
        {
            return _insert;
        }

        public ContainsKeyOperation ContainsOperation()
        {
            return _contains;
        }
    }
}