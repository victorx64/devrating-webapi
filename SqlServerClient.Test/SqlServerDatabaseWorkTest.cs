using System;
using DevRating.DefaultObject;
using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseWorkTest
    {
        [Fact]
        public void ReturnsValidAdditions()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var additions = 1u;
                var createdAt = DateTimeOffset.UtcNow;

                Assert.Equal(
                    additions,
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt).Id(),
                        additions,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Additions()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidRepository()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var repository = "repo";
                var createdAt = DateTimeOffset.UtcNow;

                Assert.Equal(
                    repository,
                    database.Entities().Works().InsertOperation().Insert(
                        repository,
                        "startCommit",
                        "endCommit",
                        null,
                        database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt).Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Repository()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidStartCommit()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var start = "startCommit";
                var createdAt = DateTimeOffset.UtcNow;

                Assert.Equal(
                    start,
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        start,
                        "endCommit",
                        null,
                        database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt).Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Start()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidEndCommit()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var end = "endCommit";
                var createdAt = DateTimeOffset.UtcNow;

                Assert.Equal(
                    end,
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        end,
                        null,
                        database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt).Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).End()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidSinceCommit()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var since = "sinceCommit";
                var createdAt = DateTimeOffset.UtcNow;

                Assert.Equal(
                    since,
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        since,
                        database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt).Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Since()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidAuthor()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                Assert.Equal(
                    author.Id(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        author.Id(),
                        2u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Author().Id()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidUsedRating()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var previous = database.Entities().Ratings().InsertOperation().Insert(
                    3423,
                    null,
                    null,
                    new DefaultId(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit1",
                        "endCommit1",
                        null,
                        author.Id(),
                        2u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Id(),
                    author.Id(),
                    createdAt
                );

                Assert.Equal(
                    previous.Id(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        author.Id(),
                        2u,
                        previous.Id(),
                        null,
                        createdAt
                    ).UsedRating().Id()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsJson()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                Assert.NotNull(
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        "sinceCommit",
                        database.Entities().Authors().InsertOperation()
                            .Insert("organization", "email@domain", createdAt).Id(),
                        2u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).ToJson()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}