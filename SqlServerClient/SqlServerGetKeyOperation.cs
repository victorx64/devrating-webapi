using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;

namespace DevRating.SqlServerClient
{
    internal class SqlServerGetKeyOperation : GetKeyOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetKeyOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Key Key(Id id)
        {
            return new SqlServerKey(_connection, id);
        }
    }
}