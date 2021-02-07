using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevRating.DefaultObject;
using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseAuthorTest
    {
        private sealed class AuthorDto
        {
            public object Id { get; set; } = new object();
            public string Email { get; set; } = string.Empty;
            [JsonPropertyName("ratings")]
            public List<RatingDto> Ratings { get; set; } = new List<RatingDto>();
        }

        private sealed class RatingDto
        {
            public object Id { get; set; } = new object();
            public double Value { get; set; } = default;
            public object? PreviousRatingId { get; set; }
            public double? PreviousRating { get; set; }
            public object WorkId { get; set; } = new object();
            public object AuthorId { get; set; } = new object();
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
            public string AuthorEmail { get; set; } = string.Empty;
            public uint? CountedDeletions { get; set; } = default;
            public uint? IgnoredDeletions { get; set; } = default;
        }

        [Fact]
        public void ReturnsValidOrganization()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var email = "email";
                var organization = "organization";

                Assert.Equal(organization,
                    database.Entities().Authors().InsertOperation().Insert(organization, email, DateTimeOffset.UtcNow)
                        .Organization());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsValidEmail()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var email = "email";
                var organization = "organization";

                Assert.Equal(email,
                    database.Entities().Authors().InsertOperation().Insert(organization, email, DateTimeOffset.UtcNow)
                        .Email());
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
                var organization = "organization";

                Assert.NotNull(
                    JsonSerializer.Deserialize<AuthorDto>(
                        database.Entities().Authors().InsertOperation()
                            .Insert(organization, "email@domain", DateTimeOffset.UtcNow)
                            .ToJson()
                    )
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsJsonWithRatings()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";

                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation()
                    .Insert(organization, "email@domain", createdAt);

                var work = database.Entities().Works().InsertOperation().Insert(
                    "repo",
                    "start",
                    "end",
                    null,
                    author.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    100,
                    null,
                    null,
                    new DefaultId(),
                    work.Id(),
                    author.Id()
                );

                Assert.Equal(
                    createdAt,
                    JsonSerializer.Deserialize<AuthorDto>(author.ToJson())!.Ratings.Single().CreatedAt
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}
