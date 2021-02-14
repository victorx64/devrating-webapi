using System.Collections.Generic;
using System.Data;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerRepositories : Repositories
    {
        private readonly IDbConnection _connection;

        public SqlServerRepositories(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<string> Repositories(string organization)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT DISTINCT Repository
                FROM Author
                WHERE Organization = @Organization";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) { Value = organization });

            using var reader = command.ExecuteReader();

            var repos = new List<string>();

            while (reader.Read())
            {
                repos.Add((string)reader["Repository"]);
            }

            return repos;
        }
    }
}