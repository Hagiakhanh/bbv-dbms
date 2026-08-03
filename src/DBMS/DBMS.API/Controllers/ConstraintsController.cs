using DBMS.API.DTOs.Constraints;
using DBMS.API.Services.Constraints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/databases/{db}/schemas/{schema}/tables/{tableName}/constraints")]
    [Route("api/tables/{tableName}/constraints")]
    public class ConstraintsController : ControllerBase
    {
        private readonly IConstraintService _constraintService;

        public ConstraintsController(IConstraintService constraintService)
        {
            _constraintService = constraintService;
        }

        [HttpPost]
        public async Task<ActionResult<ConstraintDto>> AddConstraint(string tableName, [FromBody] CreateConstraintRequest request, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
                var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

                var created = await _constraintService.AddConstraintAsync(targetDb, targetSchema, tableName, request, cancellationToken);
                return CreatedAtAction(nameof(GetConstraintByName), new { db = created.DatabaseName, schema = created.SchemaName, tableName = created.TableName, name = created.Name }, created);
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
        public async Task<ActionResult<IEnumerable<ConstraintDto>>> GetConstraints(string tableName, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var constraints = await _constraintService.GetConstraintsByTableAsync(targetDb, targetSchema, tableName, cancellationToken);
            return Ok(constraints);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ConstraintDto>> GetConstraintByName(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var constraint = await _constraintService.GetConstraintByNameAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (constraint == null)
            {
                return NotFound(new { Message = $"Constraint '{name}' not found on table '{tableName}'." });
            }
            return Ok(constraint);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DropConstraint(string tableName, string name, [FromRoute] string? db = null, [FromRoute] string? schema = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(db) ? db : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schema) ? schema : "dbo";

            var deleted = await _constraintService.DropConstraintAsync(targetDb, targetSchema, tableName, name, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { Message = $"Constraint '{name}' not found on table '{tableName}'." });
            }
            return NoContent();
        }
    }
}
