using System.Data;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerContainsRatingOperation : ContainsRatingOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsRatingOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool ContainsRatingOf(Id author)
        {
            using var command = _connection.CreateCommand();

            command.CommandText =
                "SELECT TOP (1) Id FROM Rating WHERE AuthorId = @AuthorId ORDER BY Id DESC";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}