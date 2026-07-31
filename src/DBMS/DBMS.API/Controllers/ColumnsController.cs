using DBMS.API.DTOs.Columns;
using DBMS.API.Services.Columns;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Route("api/tables/{tableName}/columns")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnService _columnService;

        public ColumnsController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpPost]
        public async Task<ActionResult<ColumnDto>> AddColumn(string tableName, [FromBody] CreateColumnRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.TableName = tableName;
                var created = await _columnService.CreateColumnAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetColumnByName), new { tableName = created.TableName, name = created.Name }, created);
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
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetColumns(string tableName, CancellationToken cancellationToken)
        {
            var columns = await _columnService.GetColumnsByTableAsync(tableName, cancellationToken);
            return Ok(columns);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ColumnDto>> GetColumnByName(string tableName, string name, CancellationToken cancellationToken)
        {
            var column = await _columnService.GetColumnByNameAsync(tableName, name, cancellationToken);
            if (column == null)
            {
                return NotFound(new { Message = $"Column '{name}' not found in table '{tableName}'." });
            }
            return Ok(column);
        }

        [HttpPatch("{name}")]
        public async Task<ActionResult<ColumnDto>> UpdateColumn(string tableName, string name, [FromBody] UpdateColumnRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.TableName = tableName;
                var updated = await _columnService.UpdateColumnAsync(name, request, cancellationToken);
                return Ok(updated);
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
        public async Task<IActionResult> DropColumn(string tableName, string name, CancellationToken cancellationToken)
        {
            var deleted = await _columnService.DropColumnAsync(tableName, name, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { Message = $"Column '{name}' not found in table '{tableName}'." });
            }
            return NoContent();
        }
    }
}
