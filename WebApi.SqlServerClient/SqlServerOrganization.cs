using System;
using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerOrganization : Organization
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerOrganization(IDbConnection connection, Id id)
        {
            _connection = connection;
            _id = id;
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset) reader["CreatedAt"];
        }

        public Id Id()
        {
            return _id;
        }

        public string Name()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Name FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string) reader["Name"];
        }

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public User User()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT UserId FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerUser(_connection, new DefaultObject.DefaultId(reader["ForeignId"]));
        }
    }
}