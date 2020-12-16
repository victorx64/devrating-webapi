using System;
using System.Collections.Generic;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerGetWorkOperation : GetWorkOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetWorkOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Work Work(string repository, string start, string end)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50) {Value = start});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = end});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerWork(_connection, new DefaultId(reader["Id"]));
        }

        public Work Work(Id id)
        {
            return new SqlServerWork(_connection, id);
        }

        public IEnumerable<Work> Last(string repository, DateTimeOffset after)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT TOP (50) w.Id
                FROM Work w
                WHERE w.Repository = @Repository AND w.CreatedAt >= @After
                ORDER BY w.Id DESC";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@After", SqlDbType.DateTimeOffset) {Value = after});

            using var reader = command.ExecuteReader();

            var works = new List<SqlServerWork>();

            while (reader.Read())
            {
                works.Add(new SqlServerWork(_connection, new DefaultId(reader["Id"])));
            }

            return works;
        }
    }
}