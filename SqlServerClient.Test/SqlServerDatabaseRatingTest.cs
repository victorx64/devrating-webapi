using System;
using System.Globalization;
using DevRating.DefaultObject;
using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseRatingTest
    {
        [Fact]
        public void ReturnsValidValue()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var value = 1100d;

                Assert.Equal(
                    value,
                    database.Entities().Ratings().InsertOperation().Insert(
                        value,
                        null,
                        null,
                        new DefaultId(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit",
                            "endCommit",
                            null,
                            author.Id(),
                            1u,
                            new DefaultId(),
                            null, createdAt
                        ).Id(),
                        author.Id(), createdAt
                    ).Value()
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

                var value = 1100d;

                Assert.Equal(
                    author.Id(),
                    database.Entities().Ratings().InsertOperation().Insert(
                        value,
                        null,
                        null,
                        new DefaultId(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit",
                            "endCommit",
                            null,
                            author.Id(),
                            1u,
                            new DefaultId(),
                            null,
                            createdAt
                        ).Id(),
                        author.Id(),
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
        public void ReturnsValidWork()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var work = database.Entities().Works().InsertOperation().Insert(
                    "repo",
                    "startCommit",
                    "endCommit",
                    null,
                    author.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                Assert.Equal(
                    work.Id(),
                    database.Entities().Ratings().InsertOperation().Insert(
                        1100d,
                        null,
                        null,
                        new DefaultId(),
                        work.Id(),
                        author.Id(),
                        createdAt
                    ).Work().Id()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidCountedDeletions()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var deletions = 12312u;

                Assert.Equal(
                    deletions,
                    database.Entities().Ratings().InsertOperation().Insert(
                        1100d,
                        deletions,
                        null,
                        new DefaultId(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit",
                            "endCommit",
                            null,
                            author.Id(),
                            1u,
                            new DefaultId(),
                            null,
                            createdAt
                        ).Id(),
                        author.Id(),
                        createdAt
                    )
                    .CountedDeletions()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidIgnoredDeletions()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var deletions = 1231u;

                Assert.Equal(
                    deletions,
                    database.Entities().Ratings().InsertOperation().Insert(
                        1100d,
                        null,
                        deletions,
                        new DefaultId(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit",
                            "endCommit",
                            null,
                            author.Id(),
                            1u,
                            new DefaultId(),
                            null,
                            createdAt
                        ).Id(),
                        author.Id(),
                        createdAt
                    )
                    .IgnoredDeletions()
                );
            }
            finally

            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidPreviousRating()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var previous = database.Entities().Ratings().InsertOperation().Insert(
                    12d,
                    null,
                    null,
                    new DefaultId(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit1",
                        "endCommit1",
                        null,
                        author.Id(),
                        12u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Id(),
                    author.Id(),
                    createdAt
                );

                Assert.Equal(
                    previous.Id(),
                    database.Entities().Ratings().InsertOperation().Insert(
                        1100d,
                        null,
                        null,
                        previous.Id(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit2",
                            "endCommit2",
                            null,
                            author.Id(),
                            1u,
                            new DefaultId(),
                            null,
                            createdAt
                        ).Id(),
                        author.Id(),
                        createdAt
                    ).PreviousRating().Id()
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
                var author = database.Entities().Authors().InsertOperation()
                    .Insert("organization", "email@domain", createdAt);

                Assert.NotNull(database.Entities().Ratings().InsertOperation().Insert(
                        12d,
                        null,
                        null,
                        new DefaultId(),
                        database.Entities().Works().InsertOperation().Insert(
                            "repo",
                            "startCommit1",
                            "endCommit1",
                            null,
                            author.Id(),
                            12u,
                            new DefaultId(),
                            null,
                            createdAt
                        ).Id(),
                        author.Id(),
                        createdAt
                    )
                    .ToJson());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}