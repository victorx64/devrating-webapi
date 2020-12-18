using System;
using System.Linq;
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
        [HttpPost("{organizationId}/{value}")]
        public IActionResult Post(int organizationId, string value)
        {
            _db.Instance().Connection().Open();

            try
            {
                var foreignId = User!.Claims!.First(c => c.Type.Equals("user_id")).Value;

                if (_db.Entities().Users().ContainsOperation().Contains(foreignId))
                {
                    var userId = _db.Entities().Users().GetOperation().User(foreignId).Id();

                    if (_db.Entities().Organizations().ContainsOperation().Contains(new DefaultId(organizationId), userId))
                    {
                        _db.Entities().Keys().InsertOperation().Insert(value, new DefaultId(organizationId), DateTimeOffset.UtcNow);

                        return new OkResult();
                    }
                    else
                    {
                        return new BadRequestObjectResult("Organization not found");
                    }
                }
                else
                {
                    return new BadRequestObjectResult("User not found");
                }
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
