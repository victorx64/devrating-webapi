using System;
using System.Collections.Generic;
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
    public class OrganizationsController : ControllerBase
    {
        private readonly ILogger<OrganizationsController> _log;
        private readonly Database _db;

        public OrganizationsController(ILogger<OrganizationsController> log, IConfiguration configuration)
            : this (log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private OrganizationsController(ILogger<OrganizationsController> log, Database db)
        {
            _log = log;
            _db = db;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _db.Instance().Connection().Open();

            try
            {
                if (_db.Entities().Organizations().ContainsOperation().Contains(new DefaultId(id)))
                {
                    return new OkObjectResult(
                        _db.Entities().Organizations().GetOperation().Organization(new DefaultId(id)).ToJson()
                    );
                }

                return new NotFoundResult();
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetByUser()
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                return new OkObjectResult(
                    ToJsonArray(
                        _db.Entities().Organizations().GetOperation().SubjectOrganizations(subject)
                    )
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [Authorize]
        [HttpPost("{name}")]
        public IActionResult Post(string name)
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                return new OkObjectResult(
                    _db.Entities().Organizations().InsertOperation().Insert(name, subject, DateTimeOffset.UtcNow).ToJson()
                );
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
