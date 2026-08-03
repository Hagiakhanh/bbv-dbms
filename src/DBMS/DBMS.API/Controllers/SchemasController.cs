using DBMS.API.DTOs.Schemas;
using DBMS.API.Services.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/databases/{db}/schemas")]
    [Route("api/[controller]")]
    [Route("api/v1/[controller]")]
    public class SchemasController : ControllerBase
    {
        private readonly ISchemaService _schemaService;

        public SchemasController(ISchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        [HttpPost]
        public async Task<ActionResult<SchemaDto>> CreateSchema([FromBody] CreateSchemaRequest request, CancellationToken cancellationToken = default, [FromRoute] string? db = null)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : (!string.IsNullOrWhiteSpace(request.DatabaseName) ? request.DatabaseName : "master");
                var createdSchema = await _schemaService.CreateSchemaAsync(targetDb, request, cancellationToken);
                return CreatedAtAction(nameof(GetSchemaByName), new { db = createdSchema.DatabaseName, name = createdSchema.Name }, createdSchema);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
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
        public async Task<ActionResult<IEnumerable<SchemaDto>>> GetSchemas([FromQuery] string? databaseName = null, CancellationToken cancellationToken = default, [FromRoute] string? db = null)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : (!string.IsNullOrWhiteSpace(databaseName) ? databaseName : "master");
                var schemas = await _schemaService.GetSchemasByDatabaseAsync(targetDb, cancellationToken);
                return Ok(schemas);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<SchemaDto>> GetSchemaByName([FromRoute] string name, CancellationToken cancellationToken = default, [FromRoute] string? db = null, [FromQuery] string? databaseName = null)
        {
            var schema = await _schemaService.GetSchemaByNameAsync(name, cancellationToken);
            if (schema == null)
            {
                return NotFound(new { Message = $"Schema '{name}' not found." });
            }
            return Ok(schema);
        }



        [HttpPatch("{name}")]
        public async Task<ActionResult<SchemaDto>> RenameSchema(string name, [FromBody] RenameSchemaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var updatedSchema = await _schemaService.RenameSchemaAsync(name, request, cancellationToken);
                return Ok(updatedSchema);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DropSchema(string name, CancellationToken cancellationToken)
        {
            var deleted = await _schemaService.DropSchemaAsync(name, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { Message = $"Schema '{name}' not found." });
            }
            return NoContent();
        }
    }
}

