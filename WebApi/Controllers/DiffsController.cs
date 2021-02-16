using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Linq;
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
            : this(
                log,
                new DevRating.SqlServerClient.SqlServerDatabase(
                    new TransactedDbConnection(
                        new SqlConnection(configuration["ConnectionString"])
                    )
                ),
                new DevRating.WebApi.SqlServerClient.SqlServerDatabase(
                    new TransactedDbConnection(
                        new SqlConnection(configuration["ConnectionString"])
                    )
                )
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
            [Required]
            public DateTimeOffset CreatedAt { get; set; } = default;
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
        [HttpPost("auth")]
        public IActionResult Post(Diff diff)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != diff.Organization)
            {
                return new UnauthorizedObjectResult(new {message = "You can only post diffs of your organization"});
            }

            _domainDb.Instance().Connection().Open();

            using var transaction = _domainDb.Instance().Connection().BeginTransaction();

            try
            {
                var factory = new DefaultEntityFactory(_domainDb.Entities(), new EloFormula());

                var work = factory.InsertedWork(
                    diff.Organization,
                    diff.Repository,
                    diff.Start,
                    diff.End,
                    diff.Since,
                    diff.Email,
                    diff.Additions,
                    diff.Link,
                    diff.CreatedAt
                );

                factory.InsertRatings(
                    diff.Organization,
                    diff.Repository,
                    diff.Email,
                    diff.Deletions.Select(d => new DefaultDeletion(d.Email, d.Counted, d.Ignored)),
                    work.Id(),
                    diff.CreatedAt
                );

                transaction.Commit();

                return new EntityAsJson(work);
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

        [HttpPost("key")]
        public IActionResult Post([FromHeader] string key, Diff diff)
        {
            var toPost = false;

            _webDb.Instance().Connection().Open();

            try
            {
                toPost = _webDb.Entities().Keys().ContainsOperation().Contains(diff.Organization, key, false);
            }
            finally
            {
                _webDb.Instance().Connection().Close();
            }

            if (toPost)
            {
                return Post(diff);
            }
            else
            {
                return new UnauthorizedObjectResult("Key not found");
            }
        }
    }
}
