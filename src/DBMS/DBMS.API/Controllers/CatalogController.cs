using DBMS.API.DTOs.Catalog;
using DBMS.API.Services.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<CatalogTreeNodeDto>>> GetCatalogTree([FromQuery] string? database = null, [FromQuery] int? depth = null, CancellationToken cancellationToken = default)
        {
            var tree = await _catalogService.GetCatalogTreeAsync(database, depth, cancellationToken);
            return Ok(tree);
        }

        [HttpGet("databases/{db}")]
        public async Task<ActionResult<DatabaseMetadataDto>> GetDatabaseMetadata([FromRoute] string db, CancellationToken cancellationToken = default)
        {
            var metadata = await _catalogService.GetDatabaseMetadataAsync(db, cancellationToken);
            if (metadata == null)
            {
                return NotFound(new { Message = $"Database '{db}' not found." });
            }
            return Ok(metadata);
        }

        [HttpGet("schemas/{schema}")]
        public async Task<ActionResult<SchemaMetadataDto>> GetSchemaMetadata([FromRoute] string schema, [FromQuery] string? db = null, CancellationToken cancellationToken = default)
        {
            var metadata = await _catalogService.GetSchemaMetadataAsync(schema, db, cancellationToken);
            if (metadata == null)
            {
                return NotFound(new { Message = $"Schema '{schema}' not found." });
            }
            return Ok(metadata);
        }

        [HttpGet("tables/{table}")]
        public async Task<ActionResult<TableMetadataDto>> GetTableMetadata([FromRoute] string table, [FromQuery] string? schema = null, [FromQuery] string? db = null, CancellationToken cancellationToken = default)
        {
            var metadata = await _catalogService.GetTableMetadataAsync(table, schema, db, cancellationToken);
            if (metadata == null)
            {
                return NotFound(new { Message = $"Table '{table}' not found." });
            }
            return Ok(metadata);
        }
    }
}
