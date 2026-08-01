using DBMS.API.DTOs.Tables;
using DBMS.API.Services.Tables;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Route("api/databases/{db}/schemas/{schema}/tables")]
    [Route("api/schemas/{schema}/tables")]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable([FromBody] CreateTableRequest request, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : request.DatabaseName;
                var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : request.SchemaName;

                var createdTable = await _tableService.CreateTableAsync(targetDb, targetSchema, request, cancellationToken);
                return CreatedAtAction(nameof(GetTableByName), new { db = createdTable.DatabaseName, schema = createdTable.SchemaName, name = createdTable.Name }, createdTable);
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
        public async Task<ActionResult<IEnumerable<TableDto>>> GetTables([FromRoute] string? db = null, [FromRoute] string? schema = null, [FromQuery] string? databaseName = null, [FromQuery] string? schemaName = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : (!string.IsNullOrWhiteSpace(databaseName) ? databaseName : "master");
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : (!string.IsNullOrWhiteSpace(schemaName) ? schemaName : "dbo");

            var tables = await _tableService.GetTablesBySchemaAsync(targetDb, targetSchema, cancellationToken);
            return Ok(tables);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<TableDto>> GetTableByName(string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var table = await _tableService.GetTableByNameAsync(targetDb, targetSchema, name, cancellationToken);
            if (table == null)
            {
                return NotFound(new { Message = $"Table '{name}' not found." });
            }
            return Ok(table);
        }

        [HttpPatch("{name}")]
        public async Task<ActionResult<TableDto>> UpdateTable(string name, [FromBody] UpdateTableRequest request, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
                var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

                var updated = await _tableService.UpdateTableAsync(targetDb, targetSchema, name, request, cancellationToken);
                return Ok(updated);
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
        public async Task<IActionResult> DropTable(string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var deleted = await _tableService.DropTableAsync(targetDb, targetSchema, name, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { Message = $"Table '{name}' not found." });
            }
            return NoContent();
        }
    }
}
