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
        private readonly Database _domainDb;
        private readonly WebApi.Domain.Database _webDb;

        public DbController(ILogger<DbController> log, IConfiguration configuration)
            : this(
                log,
                new SqlServerDatabase(new TransactedDbConnection(new SqlConnection(configuration["ConnectionString"]))),
                new WebApi.SqlServerClient.SqlServerDatabase(new TransactedDbConnection(new SqlConnection(configuration["ConnectionString"])))
            )
        {
        }

        private DbController(ILogger<DbController> log, Database domainDb, WebApi.Domain.Database webDb)
        {
            _log = log;
            _domainDb = domainDb;
            _webDb = webDb;
        }

        [HttpGet("domain")]
        public IActionResult SeedDomainDb()
        {
            _log.LogInformation("Domain Db Seed requested");

            _domainDb.Instance().Connection().Open();

            var transaction = _domainDb.Instance().Connection().BeginTransaction();

            try
            {
                if (_domainDb.Instance().Present())
                {
                    _log.LogInformation("Domain instance already exists");
                    return new OkResult();
                }

                _domainDb.Instance().Create();

                transaction.Commit();

                _log.LogInformation("Domain instance successfully created");
                return new OkResult();
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

        [HttpGet("web")]
        public IActionResult SeedWebDb()
        {
            _log.LogInformation("Web Db Seed requested");

            _webDb.Instance().Connection().Open();

            var transaction = _webDb.Instance().Connection().BeginTransaction();

            try
            {
                if (_webDb.Instance().Present())
                {
                    _log.LogInformation("Web instance already exists");
                    return new OkResult();
                }

                _webDb.Instance().Create();

                transaction.Commit();

                _log.LogInformation("Web instance successfully created");
                return new OkResult();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _webDb.Instance().Connection().Close();
            }
        }
    }
}
