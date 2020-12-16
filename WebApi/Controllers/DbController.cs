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

        public DbController(ILogger<DbController> log, IConfiguration configuration)
            : this (log, new SqlServerDatabase(new TransactedDbConnection(new SqlConnection(configuration["ConnectionString"]))))
        {
        }

        private DbController(ILogger<DbController> log, Database db)
        {
            _log = log;
            _db = db;
        }

        [HttpGet("Seed")]
        public IActionResult Seed()
        {
            _log.LogInformation("Db Seed requested");

            _db.Instance().Connection().Open();

            var transaction = _db.Instance().Connection().BeginTransaction();

            try
            {
                if (_db.Instance().Present())
                {
                    _log.LogInformation("Instance already exists");
                    return new OkObjectResult("Instance already exists");
                }
                
                _db.Instance().Create();
                
                transaction.Commit();
                
                _log.LogInformation("Instance successfully created");
                return new OkObjectResult("Instance successfully created");
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
