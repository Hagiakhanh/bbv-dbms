using DBMS.API.DTOs.Schemas;
using DBMS.API.Services.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchemasController : ControllerBase
    {
        private readonly ISchemaService _schemaService;

        public SchemasController(ISchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        [HttpPost]
        public async Task<ActionResult<SchemaDto>> CreateSchema([FromBody] CreateSchemaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var createdSchema = await _schemaService.CreateSchemaAsync(request.DatabaseName, request, cancellationToken);
                return CreatedAtAction(nameof(GetSchemaByName), new { name = createdSchema.Name, databaseName = createdSchema.DatabaseName }, createdSchema);
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
        public async Task<ActionResult<IEnumerable<SchemaDto>>> GetSchemas([FromQuery] string? databaseName, CancellationToken cancellationToken)
        {
            try
            {
                var targetDb = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
                var schemas = await _schemaService.GetSchemasByDatabaseAsync(targetDb, cancellationToken);
                return Ok(schemas);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<SchemaDto>> GetSchemaByName(string name, [FromQuery] string? databaseName, CancellationToken cancellationToken)
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
