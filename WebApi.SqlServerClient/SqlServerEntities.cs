using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerEntities : Entities
    {
        private readonly Keys _keys;
        private readonly Repositories _repositories;

        public SqlServerEntities(IDbConnection connection)
            : this(
                new SqlServerKeys(connection),
                new SqlServerRepositories(connection)
            )
        {
        }

        public SqlServerEntities(Keys keys, Repositories repositories)
        {
            _keys = keys;
            _repositories = repositories;
        }

        public Keys Keys()
        {
            return _keys;
        }

        public Repositories Repositories()
        {
            return _repositories;
        }
    }
}