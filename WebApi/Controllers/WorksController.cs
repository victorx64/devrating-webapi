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
    public class WorksController : ControllerBase
    {
        private readonly ILogger<WorksController> _log;
        private readonly Database _db;

        public WorksController(ILogger<WorksController> log, IConfiguration configuration)
            : this (log, new SqlServerDatabase(new SqlConnection(configuration["ConnectionString"])))
        {
        }

        private WorksController(ILogger<WorksController> log, Database db)
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
                if (_db.Entities().Works().ContainsOperation().Contains(new DefaultId(id)))
                {
                    return new EntityAsJson(_db.Entities().Works().GetOperation().Work(new DefaultId(id)));
                }

                return new NotFoundResult();
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }

        [HttpGet]
        public IActionResult GetByRepository(string organization, string repository, DateTimeOffset after)
        {
            _db.Instance().Connection().Open();

            try
            {
                return new EntityAsJson(
                    _db.Entities().Works().GetOperation()
                        .Last(
                            organization,
                            repository,
                            after
                        )
                );
            }
            finally
            {
                _db.Instance().Connection().Close();
            }
        }
    }
}
