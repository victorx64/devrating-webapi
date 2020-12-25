using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using DevRating.DefaultObject;
using DevRating.EloRating;
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
        private readonly DevRating.Domain.Database _domainDb;
        private readonly DevRating.WebApi.Domain.Database _webDb;

        public DiffsController(ILogger<DiffsController> log, IConfiguration configuration)
            : this (
                log,
                new DevRating.SqlServerClient.SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])),
                new DevRating.WebApi.SqlServerClient.SqlServerDatabase(new SqlConnection(configuration["ConnectionString"]))
            )
        {
        }

        private DiffsController(
            ILogger<DiffsController> log, 
            DevRating.Domain.Database domainDb,
            DevRating.WebApi.Domain.Database webDb
        )
        {
            _log = log;
            _domainDb = domainDb;
            _webDb = webDb;
        }

        public sealed class Diff
        {
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            public string Start { get; set; } = string.Empty;
            [Required]
            public string End { get; set; } = string.Empty;
            [Required]
            public string Organization { get; set; } = string.Empty;
            public string? Since { get; set; } = default;
            [Required]
            public string Repository { get; set; } = string.Empty;
            public string? Link { get; set; } = default;
            [Required]
            public uint Additions { get; set; } = default;
            [Required]
            public IEnumerable<Deletion> Deletions { get; set; } = Array.Empty<Deletion>();
            public class Deletion
            {
                [Required]
                public string Email { get; set; } = string.Empty;
                [Required]
                public uint Counted { get; set; } = default;
                [Required]
                public uint Ignored { get; set; } = default;
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post(Diff diff)
        {
            _domainDb.Instance().Connection().Open();

            using var transaction = _domainDb.Instance().Connection().BeginTransaction();

            try
            {
                var factory = new DefaultEntityFactory(_domainDb.Entities(), new EloFormula());

                var createdAt = DateTimeOffset.UtcNow;

                var work = factory.InsertedWork(
                    diff.Organization,
                    diff.Repository,
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
                _domainDb.Instance().Connection().Close();
            }
        }

        [HttpPost("{key}")]
        public IActionResult Post(string key, Diff diff)
        {
            _webDb.Instance().Connection().Open();

            try
            {
                if (_webDb.Entities().Organizations().ContainsOperation().Contains(diff.Organization))
                {
                    var org = _webDb.Entities().Organizations().GetOperation().Organization(diff.Organization).Id();

                    if (!_webDb.Entities().Keys().ContainsOperation().Contains(org, key))
                    {
                        return new UnauthorizedObjectResult("Key not found");
                    }
                }
                else
                {
                    return new BadRequestObjectResult("Organization not found");
                }
            }
            finally
            {
                _webDb.Instance().Connection().Close();
            }

            return Post(diff);
        }

        private string ToJsonArray(IEnumerable<DevRating.Domain.Entity> entities)
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
