using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerUsers : Users
    {
        private readonly GetUserOperation _get;
        private readonly InsertUserOperation _insert;
        private readonly ContainsUserOperation _contains;

        public SqlServerUsers(IDbConnection connection)
            : this(
                new SqlServerGetUserOperation(connection),
                new SqlServerInsertUserOperation(connection),
                new SqlServerContainsUserOperation(connection)
            )
        {
        }

        public SqlServerUsers(GetUserOperation get, InsertUserOperation insert, ContainsUserOperation contains)
        {
            _get = get;
            _insert = insert;
            _contains = contains;
        }

        public GetUserOperation GetOperation()
        {
            return _get;
        }

        public InsertUserOperation InsertOperation()
        {
            return _insert;
        }

        public ContainsUserOperation ContainsOperation()
        {
            return _contains;
        }
    }
}