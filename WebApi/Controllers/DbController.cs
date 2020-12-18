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
    public class DbController : ControllerBase
    {
        private readonly ILogger<DbController> _log;
        private readonly Database _db;
        private readonly WebApi.Domain.Database _webDb;

        public DbController(ILogger<DbController> log, IConfiguration configuration)
            : this(
                log,
                new SqlServerDatabase(new TransactedDbConnection(new SqlConnection(configuration["ConnectionString"]))),
                new WebApi.SqlServerClient.SqlServerDatabase(new TransactedDbConnection(new SqlConnection(configuration["ConnectionString"])))
            )
        {
        }

        private DbController(ILogger<DbController> log, Database db, WebApi.Domain.Database webDb)
        {
            _log = log;
            _db = db;
            _webDb = webDb;
        }

        [HttpGet("SeedDomainDb")]
        public IActionResult SeedDomainDb()
        {
            _log.LogInformation("Domain Db Seed requested");

            _db.Instance().Connection().Open();

            var transaction = _db.Instance().Connection().BeginTransaction();

            try
            {
                if (_db.Instance().Present())
                {
                    _log.LogInformation("Domain instance already exists");
                    return new OkObjectResult("Domain instance already exists");
                }

                _db.Instance().Create();

                transaction.Commit();

                _log.LogInformation("Domain instance successfully created");
                return new OkObjectResult("Domain instance successfully created");
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

        [HttpGet("SeedWebDb")]
        public IActionResult SeedWebDb()
        {
            _log.LogInformation("Web Db Seed requested");

            _db.Instance().Connection().Open();

            var transaction = _db.Instance().Connection().BeginTransaction();

            try
            {
                if (_db.Instance().Present())
                {
                    _log.LogInformation("Web instance already exists");
                    return new OkObjectResult("Web instance already exists");
                }

                _db.Instance().Create();

                transaction.Commit();

                _log.LogInformation("Web instance successfully created");
                return new OkObjectResult("Web instance successfully created");
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
    }
}
