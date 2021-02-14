using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerEntities : Entities
    {
        private readonly Keys _keys;

        public SqlServerEntities(IDbConnection connection)
            : this(
                new SqlServerKeys(connection)
            )
        {
        }

        public SqlServerEntities(Keys keys)
        {
            _keys = keys;
        }

        public Keys Keys()
        {
            return _keys;
        }
    }
}