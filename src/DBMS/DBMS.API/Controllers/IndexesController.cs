using DBMS.API.DTOs.Indexes;
using DBMS.API.Services.Indexes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/databases/{db}/schemas/{schema}/tables/{tableName}/indexes")]
    [Route("api/tables/{tableName}/indexes")]
    public class IndexesController : ControllerBase
    {
        private readonly IIndexService _indexService;

        public IndexesController(IIndexService indexService)
        {
            _indexService = indexService;
        }

        [HttpPost]
        public async Task<ActionResult<IndexDto>> CreateIndex(string tableName, [FromBody] CreateIndexRequest request, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
                var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

                var created = await _indexService.CreateIndexAsync(targetDb, targetSchema, tableName, request, cancellationToken);
                return CreatedAtAction(nameof(GetIndexByName), new { db = created.DatabaseName, schema = created.SchemaName, tableName = created.TableName, name = created.Name }, created);
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
        public async Task<ActionResult<IEnumerable<IndexDto>>> GetIndexes(string tableName, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var indexes = await _indexService.GetIndexesByTableAsync(targetDb, targetSchema, tableName, cancellationToken);
            return Ok(indexes);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<IndexDto>> GetIndexByName(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var index = await _indexService.GetIndexByNameAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (index == null)
            {
                return NotFound(new { Message = $"Index '{name}' not found on table '{tableName}'." });
            }
            return Ok(index);
        }

        [HttpPost("{name}/rebuild")]
        public async Task<IActionResult> RebuildIndex(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var result = await _indexService.RebuildIndexAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (!result)
            {
                return NotFound(new { Message = $"Index '{name}' not found on table '{tableName}'." });
            }
            return Accepted();
        }

        [HttpPost("{name}/enable")]
        public async Task<IActionResult> EnableIndex(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var result = await _indexService.EnableIndexAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (!result)
            {
                return NotFound(new { Message = $"Index '{name}' not found on table '{tableName}'." });
            }
            return Ok();
        }

        [HttpPost("{name}/disable")]
        public async Task<IActionResult> DisableIndex(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var result = await _indexService.DisableIndexAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (!result)
            {
                return NotFound(new { Message = $"Index '{name}' not found on table '{tableName}'." });
            }
            return Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DropIndex(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var deleted = await _indexService.DropIndexAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { Message = $"Index '{name}' not found on table '{tableName}'." });
            }
            return NoContent();
        }
    }
}
