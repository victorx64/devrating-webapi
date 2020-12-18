using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerEntities : Entities
    {
        private readonly Organizations _organizations;
        private readonly Keys _keys;

        public SqlServerEntities(IDbConnection connection)
            : this(
                new SqlServerOrganizations(connection),
                new SqlServerKeys(connection)
            )
        {
        }

        public SqlServerEntities(Organizations organizations, Keys keys)
        {
            _organizations = organizations;
            _keys = keys;
        }

        public Organizations Organizations()
        {
            return _organizations;
        }

        public Keys Keys()
        {
            return _keys;
        }
    }
}