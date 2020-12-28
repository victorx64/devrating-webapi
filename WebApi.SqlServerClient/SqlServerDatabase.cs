using System.Data;
using DevRating.WebApi.Domain;

namespace DevRating.WebApi.SqlServerClient
{
    public sealed class SqlServerDatabase : Database
    {
        private readonly Entities _entities;
        private readonly DevRating.Domain.DbInstance _instance;

        public SqlServerDatabase(IDbConnection connection)
            : this(new SqlServerDbInstance(connection), new SqlServerEntities(connection))
        {
        }

        public SqlServerDatabase(DevRating.Domain.DbInstance instance, Entities entities)
        {
            _instance = instance;
            _entities = entities;
        }

        public Entities Entities()
        {
            return _entities;
        }

        DevRating.Domain.DbInstance Database.Instance()
        {
            return _instance;
        }
    }
}