using System.Security.Claims;
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
    public class RepositoriesController : ControllerBase
    {
        private readonly ILogger<RepositoriesController> _log;
        private readonly Database _db;

        public RepositoriesController(ILogger<RepositoriesController> log, IConfiguration configuration)
            : this(log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private RepositoriesController(ILogger<RepositoriesController> log, Database db)
        {
            _log = log;
            _db = db;
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserRepositories()
        {
            _db.Instance().Connection().Open();

            try
            {
                return new JsonResult(_db.Entities().Repositories().Repositories(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
