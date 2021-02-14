using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
        [HttpGet]
        public IActionResult Get()
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                return new EntityAsJson(
                    _db.Entities()
                        .Keys()
                        .GetOperation()
                        .OrganizationKeys(subject)
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        public sealed class Key
        {
            public string? Name { get; set; } = default;
            [Required]
            public string Value { get; set; } = string.Empty;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post(Key key)
        {
            _db.Instance().Connection().Open();

            try
            {
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                return new EntityAsJson(
                    _db.Entities()
                        .Keys()
                        .InsertOperation()
                        .Insert(
                            key.Name,
                            key.Value,
                            subject,
                            DateTimeOffset.UtcNow
                        )
                );
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
                var subject = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                if (_db.Entities().Keys().ContainsOperation().Contains(new DefaultId(id), subject))
                {
                    return new EntityAsJson(
                        _db.Entities()
                            .Keys()
                            .InsertOperation()
                            .Revoke(new DefaultId(id), DateTimeOffset.UtcNow)
                    );
                }

                return new NotFoundResult();

            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
