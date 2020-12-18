using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerGetUserOperation : GetUserOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetUserOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public User User(string foreignId)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM User WHERE ForeignId = @ForeignId";

            command.Parameters.Add(new SqlParameter("@ForeignId", SqlDbType.NVarChar) {Value = foreignId});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerUser(_connection, new DefaultId(reader["Id"]));
        }

        public User User(Id id)
        {
            return new SqlServerUser(_connection, id);
        }
    }
}