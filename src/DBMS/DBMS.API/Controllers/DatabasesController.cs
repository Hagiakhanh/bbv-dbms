using DBMS.API.DTOs;
using DBMS.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabasesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public DatabasesController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost]
        public async Task<ActionResult<DatabaseDto>> CreateDatabase([FromBody] CreateDatabaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var createdDb = await _databaseService.CreateDatabaseAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetDatabaseByName), new { name = createdDb.Name }, createdDb);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatabaseDto>>> GetDatabases(CancellationToken cancellationToken)
        {
            var databases = await _databaseService.GetAllDatabasesAsync(cancellationToken);
            return Ok(databases);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<DatabaseDto>> GetDatabaseByName(string name, CancellationToken cancellationToken)
        {
            var database = await _databaseService.GetDatabaseByNameAsync(name, cancellationToken);
            if (database == null)
            {
                return NotFound(new { Message = $"Database '{name}' not found." });
            }
            return Ok(database);
        }
    }
}


