using System.Data;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    public sealed class SqlServerDatabase : Database
    {
        private readonly Entities _entities;
        private readonly DbInstance _instance;

        public SqlServerDatabase(IDbConnection connection)
            : this(new SqlServerDbInstance(connection), new SqlServerEntities(connection))
        {
        }

        public SqlServerDatabase(DbInstance instance, Entities entities)
        {
            _instance = instance;
            _entities = entities;
        }

        public DbInstance Instance()
        {
            return _instance;
        }

        public Entities Entities()
        {
            return _entities;
        }
    }
}