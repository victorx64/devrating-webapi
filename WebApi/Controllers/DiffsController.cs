using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.EloRating;
using DevRating.SqlServerClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevRating.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiffsController : ControllerBase
    {
        private readonly ILogger<DiffsController> _log;
        private readonly Database _db;

        public DiffsController(ILogger<DiffsController> log, IConfiguration configuration)
            : this (log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private DiffsController(ILogger<DiffsController> log, Database db)
        {
            _log = log;
            _db = db;
        }

        [Authorize]
        [HttpPost("{key}")]
        public IActionResult Post(string key, Dto diff)
        {
            _db.Instance().Connection().Open();

            using var transaction = _db.Instance().Connection().BeginTransaction();

            try
            {
                var factory = new DefaultEntityFactory(_db.Entities(), new EloFormula());

                var createdAt = DateTimeOffset.UtcNow;

                var work = factory.InsertedWork(
                    diff.Organization,
                    diff.Key,
                    diff.Start,
                    diff.End,
                    diff.Since,
                    diff.Email,
                    diff.Additions,
                    diff.Link,
                    createdAt
                );

                factory.InsertRatings(
                    diff.Organization,
                    diff.Email,
                    diff.Deletions.Select(d => new DefaultDeletion(d.Email, d.Counted, d.Ignored)),
                    work.Id(),
                    createdAt
                );

                transaction.Commit();

                return new OkObjectResult(work.ToJson());
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        public sealed class Dto
        {
            public string Email { get; set; } = string.Empty;
            public string Start { get; set; } = string.Empty;
            public string End { get; set; } = string.Empty;
            public string Organization { get; set; } = string.Empty;
            public string? Since { get; set; } = default;
            public string Key { get; set; } = string.Empty;
            public string? Link { get; set; } = default;
            public uint Additions { get; set; } = default;
            public IEnumerable<DeletionDto> Deletions { get; set; } = Array.Empty<DeletionDto>();
            public class DeletionDto
            {
                public string Email { get; set; } = string.Empty;
                public uint Counted { get; set; } = default;
                public uint Ignored { get; set; } = default;
            }
        }

        private string ToJsonArray(IEnumerable<Entity> entities)
        {
            var builder = new StringBuilder("[");

            if (entities.Any())
            {
                foreach (var author in entities)
                {
                    builder.Append(author.ToJson());
                    builder.Append(",");
                }

                builder.Remove(builder.Length - 1, 1);
            }

            builder.Append("]");

            return builder.ToString();
        }
    }
}
