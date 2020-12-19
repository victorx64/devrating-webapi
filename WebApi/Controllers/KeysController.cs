using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DevRating.DefaultObject;
using DevRating.WebApi.Domain;
using DevRating.WebApi.SqlServerClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevRating.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeysController : ControllerBase
    {
        private readonly ILogger<KeysController> _log;
        private readonly Database _db;

        public KeysController(ILogger<KeysController> log, IConfiguration configuration)
            : this(log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private KeysController(ILogger<KeysController> log, Database db)
        {
            _log = log;
            _db = db;
        }

        [Authorize]
        [HttpGet("{organization}")]
        public IActionResult Get(string organization)
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                if (_db.Entities().Organizations().ContainsOperation().Contains(organization, subject))
                {
                    return new OkObjectResult(
                        ToJsonArray(
                            _db.Entities()
                                .Keys()
                                .GetOperation()
                                .OrganizationKeys(organization)
                        )
                    );
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        public sealed class Dto
        {
            public string? Name { get; set; } = default;
            [Required]
            public string Value { get; set; } = string.Empty;
        }

        [Authorize]
        [HttpPost("{organization}")]
        public IActionResult Post(string organization, Dto key)
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                if (_db.Entities().Organizations().ContainsOperation().Contains(organization, subject))
                {
                    return new OkObjectResult(
                        _db.Entities()
                            .Keys()
                            .InsertOperation()
                            .Insert(
                                key.Name,
                                key.Value,
                                _db.Entities()
                                    .Organizations()
                                    .GetOperation()
                                    .Organization(organization)
                                    .Id(),
                                DateTimeOffset.UtcNow
                            )
                            .ToJson()
                    );
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _db.Instance().Connection().Open();

            try
            {
                if (_db.Entities().Keys().ContainsOperation().Contains(new DefaultId(id)))
                {
                    var key = _db.Entities().Keys().GetOperation().Key(new DefaultId(id));

                    var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                    if (key.Organization().AuthorizedSubject().Equals(subject, StringComparison.Ordinal))
                    {
                        return new OkObjectResult(
                            _db.Entities()
                                .Keys()
                                .InsertOperation()
                                .Revoke(new DefaultId(id), DateTimeOffset.UtcNow)
                                .ToJson()
                        );
                    }

                    return new NotFoundResult();
                }

                return new NotFoundResult();

            }
            finally
            {
                _db.Instance().Connection().Close();
            }
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
