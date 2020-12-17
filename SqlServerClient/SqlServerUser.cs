using System;
using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerUser : User
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerUser(IDbConnection connection, Id id)
        {
            _connection = connection;
            _id = id;
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM User WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset) reader["CreatedAt"];
        }

        public string ForeignId()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT ForeignId FROM User WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string) reader["ForeignId"];
        }

        public Id Id()
        {
            return _id;
        }

        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}