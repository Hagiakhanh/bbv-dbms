using DBMS.API.DTOs.Databases;
using DBMS.API.Services.Databases;
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

        [HttpPatch("{name}")]
        public async Task<ActionResult<DatabaseDto>> UpdateDatabase(string name, [FromBody] UpdateDatabaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var updatedDb = await _databaseService.UpdateDatabaseAsync(name, request, cancellationToken);
                return Ok(updatedDb);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DropDatabase(string name, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _databaseService.DropDatabaseAsync(name, cancellationToken);
                if (!deleted)
                {
                    return NotFound(new { Message = $"Database '{name}' not found." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{name}/state")]
        public async Task<ActionResult<DatabaseDto>> SetDatabaseState(string name, [FromBody] SetDatabaseStateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var updatedDb = await _databaseService.SetStateAsync(name, request, cancellationToken);
                return Ok(updatedDb);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("attach")]
        public async Task<ActionResult<DatabaseDto>> AttachDatabase([FromBody] AttachDatabaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var createdDb = await _databaseService.AttachDatabaseAsync(request, cancellationToken);
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

        [HttpPost("{name}/detach")]
        public async Task<IActionResult> DetachDatabase(string name, CancellationToken cancellationToken)
        {
            try
            {
                var detached = await _databaseService.DetachDatabaseAsync(name, cancellationToken);
                if (!detached)
                {
                    return NotFound(new { Message = $"Database '{name}' not found." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

