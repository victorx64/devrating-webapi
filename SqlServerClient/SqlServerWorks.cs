using System.Data;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerWorks : Works
    {
        private readonly InsertWorkOperation _insert;
        private readonly GetWorkOperation _get;
        private readonly ContainsWorkOperation _contains;

        public SqlServerWorks(IDbConnection connection)
            : this(new SqlServerInsertWorkOperation(connection),
                new SqlServerGetWorkOperation(connection),
                new SqlServerContainsWorkOperation(connection))
        {
        }

        public SqlServerWorks(InsertWorkOperation insert, GetWorkOperation get, ContainsWorkOperation contains)
        {
            _insert = insert;
            _get = get;
            _contains = contains;
        }

        public InsertWorkOperation InsertOperation()
        {
            return _insert;
        }

        public GetWorkOperation GetOperation()
        {
            return _get;
        }

        public ContainsWorkOperation ContainsOperation()
        {
            return _contains;
        }
    }
}