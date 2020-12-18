using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerOrganizations : Organizations
    {
        private readonly GetOrganizationOperation _get;
        private readonly InsertOrganizationOperation _insert;
        private readonly ContainsOrganizationOperation _contains;

        public SqlServerOrganizations(IDbConnection connection)
            : this(
                new SqlServerGetOrganizationOperation(connection),
                new SqlServerInsertOrganizationOperation(connection),
                new SqlServerContainsOrganizationOperation(connection)
            )
        {
        }

        public SqlServerOrganizations(GetOrganizationOperation get, InsertOrganizationOperation insert, ContainsOrganizationOperation contains)
        {
            _get = get;
            _insert = insert;
            _contains = contains;
        }

        public GetOrganizationOperation GetOperation()
        {
            return _get;
        }

        public InsertOrganizationOperation InsertOperation()
        {
            return _insert;
        }

        public ContainsOrganizationOperation ContainsOperation()
        {
            return _contains;
        }
    }
}