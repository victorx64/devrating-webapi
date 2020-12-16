using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient.Test
{
    public sealed class TemporalLocalSqlConnection : IDbConnection
    {
        private readonly string _file;
        private readonly string _member;
        private readonly IDbConnection _origin;

        // To start SQL Server in Docker: 
        // docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=65awe_5f651323d' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
        // Connection string:
        // Server=localhost;User Id=sa;Password=65awe_5f651323d;
        public TemporalLocalSqlConnection([CallerFilePath] string file = "", [CallerMemberName] string member = "")
            : this(
                new SqlConnection("Server=localhost;User Id=sa;Password=65awe_5f651323d;"),
                // new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Pooling=False;Connection Timeout=45"),
                file,
                member
            )
        {
        }

        public TemporalLocalSqlConnection(
            IDbConnection origin,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = ""
        )
        {
            _origin = origin;
            _member = member;
            _file = Path.GetFileNameWithoutExtension(file);
        }

        public void Dispose()
        {
            _origin.Dispose();
        }

        public IDbTransaction BeginTransaction()
        {
            return _origin.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _origin.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _origin.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            ChangeDatabase("master");

            DropDatabase();

            _origin.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _origin.CreateCommand();
        }

        public void Open()
        {
            _origin.Open();

            CreateDatabase();

            ChangeDatabase(Name());
        }

        private string Name()
        {
            return $"{_file}_{_member}";
        }

        private void CreateDatabase()
        {
            using var command = _origin.CreateCommand();

            command.CommandText = $"CREATE DATABASE {Name()}";

            command.ExecuteNonQuery();
        }

        private void DropDatabase()
        {
            using var command = _origin.CreateCommand();

            command.CommandText = $"DROP DATABASE {Name()}";

            command.ExecuteNonQuery();
        }

#nullable disable
        public string ConnectionString
        {
            get => _origin.ConnectionString;
            set => _origin.ConnectionString = value;
        }
#nullable enable

        public int ConnectionTimeout => _origin.ConnectionTimeout;
        public string Database => _origin.Database;
        public ConnectionState State => _origin.State;
    }
}