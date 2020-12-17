using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseInstanceTest
    {
        [Fact]
        public void DoesntHaveInstanceByDefault()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();

            try
            {
                Assert.False(database.Instance().Present());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void CreatesInstance()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();

            try
            {
                database.Instance().Create();

                Assert.True(database.Instance().Present());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}