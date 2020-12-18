using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerEntities : Entities
    {
        private readonly Users _users;
        private readonly Organizations _organizations;
        private readonly Keys _keys;

        public SqlServerEntities(IDbConnection connection)
            : this(
                new SqlServerUsers(connection),
                new SqlServerOrganizations(connection),
                new SqlServerKeys(connection)
            )
        {
        }

        public SqlServerEntities(Users users, Organizations organizations, Keys keys)
        {
            _users = users;
            _organizations = organizations;
            _keys = keys;
        }

        public Users Users()
        {
            return _users;
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