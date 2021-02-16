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
    public class StatusesController : ControllerBase
    {
        private readonly ILogger<StatusesController> _log;
        private readonly Database _domainDb;
        private readonly WebApi.Domain.Database _webDb;

        public StatusesController(ILogger<StatusesController> log, IConfiguration configuration)
            : this(
                log,
                new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])),
                new WebApi.SqlServerClient.SqlServerDatabase(new SqlConnection(configuration["ConnectionString"]))
            )
        {
        }

        private StatusesController(ILogger<StatusesController> log, Database domainDb, WebApi.Domain.Database webDb)
        {
            _log = log;
            _domainDb = domainDb;
            _webDb = webDb;
        }

        [HttpGet("domain")]
        public IActionResult DomainStatus()
        {
            _domainDb.Instance().Connection().Open();

            try
            {
                if (_domainDb.Instance().Present())
                {
                    return new JsonResult(new { message = "Domain DB is present" });
                }

                return new JsonResult(new { message = "Domain DB is not present" });
            }
            finally
            {
                _domainDb.Instance().Connection().Close();
            }
        }

        [HttpGet("web")]
        public IActionResult WebStatus()
        {
            _webDb.Instance().Connection().Open();

            try
            {
                if (_webDb.Instance().Present())
                {
                    return new JsonResult(new { message = "Web DB is present" });
                }

                return new JsonResult(new { message = "Web DB is not present" });
            }
            finally
            {
                _webDb.Instance().Connection().Close();
            }
        }
    }
}
