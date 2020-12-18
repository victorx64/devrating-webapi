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
                var user = User!.Claims!.First(c => c.Type.Equals("user_id")).Value;

                if (_db.Entities().Organizations().ContainsOperation().Contains(new DefaultId(organizationId), user))
                {
                    return new OkObjectResult(
                        _db.Entities().Keys().InsertOperation().Insert(value, new DefaultId(organizationId), DateTimeOffset.UtcNow).ToJson()
                    );
                }
                else
                {
                    return new BadRequestObjectResult("Organization not found");
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
                var user = User!.Claims!.First(c => c.Type.Equals("user_id")).Value;

                return new OkObjectResult(
                    _db.Entities().Keys().InsertOperation().Revoke(new DefaultId(id), DateTimeOffset.UtcNow).ToJson()
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
