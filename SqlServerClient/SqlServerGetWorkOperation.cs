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

        public Work Work(string organization, string repository, string start, string end)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT w.Id
                FROM Work w
                INNER JOIN Author a on a.Id = w.AuthorId
                WHERE a.Organization = @Organization
                AND a.Repository = @Repository 
                AND w.StartCommit = @StartCommit
                AND w.EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});
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

        public IEnumerable<Work> Last(string organization, string repository, DateTimeOffset after)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT w.Id
                FROM Work w
                INNER JOIN Author a on a.Id = w.AuthorId
                WHERE a.Organization = @Organization AND a.Repository = @Repository AND w.CreatedAt >= @After
                ORDER BY w.Id DESC";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});
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