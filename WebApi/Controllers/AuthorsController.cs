using System;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.SqlServerClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevRating.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _log;
        private readonly Database _db;

        public AuthorsController(ILogger<AuthorsController> log, IConfiguration configuration)
            : this(log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private AuthorsController(ILogger<AuthorsController> log, Database db)
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
                if (_db.Entities().Authors().ContainsOperation().Contains(new DefaultId(id)))
                {
                    return new EntityAsJson(
                        _db.Entities().Authors().GetOperation().Author(new DefaultId(id))
                    );
                }

                return new NotFoundResult();
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [HttpGet("top")]
        public IActionResult GetTop(string organization, string repository, DateTimeOffset after)
        {
            _db.Instance().Connection().Open();

            try
            {
                return new EntityAsJson(
                    _db.Entities().Authors().GetOperation().Top(organization, repository, after)
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [HttpGet]
        public IActionResult GetByCreds(string organization, string repository, string email)
        {
            _db.Instance().Connection().Open();

            try
            {
                if (_db.Entities().Authors().ContainsOperation().Contains(organization, repository, email))
                {
                    return new EntityAsJson(
                        _db.Entities().Authors().GetOperation().Author(organization, repository, email)
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
