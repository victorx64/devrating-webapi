using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class RatingsController : ControllerBase
    {
        private readonly ILogger<RatingsController> _log;
        private readonly Database _db;

        public RatingsController(ILogger<RatingsController> log, IConfiguration configuration)
            : this (log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private RatingsController(ILogger<RatingsController> log, Database db)
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
                if (_db.Entities().Ratings().ContainsOperation().Contains(new DefaultId(id)))
                {
                    return new EntityAsJson(_db.Entities().Ratings().GetOperation().Rating(new DefaultId(id)));
                }

                return new NotFoundResult();
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [HttpGet("works/{work}")]
        public IActionResult GetByWork(int work)
        {
            _db.Instance().Connection().Open();

            try
            {
                return new EntityAsJson(
                    _db.Entities().Ratings().GetOperation().RatingsOf(new DefaultId(work))
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
