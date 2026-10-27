using FinanceTracker.Api.Data;
using FinanceTracker.Api.Dtos;
using FinanceTracker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        public CategoriesController(FinanceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(RecordTypes? recordType, CancellationToken cancellationToken)
        {
            var query = _context.Categories.AsQueryable();

            if (recordType is not null)
            {
                var typeName = recordType.ToString();
                query = query.Where(c => c.RecordType.Name == typeName);
            }

            var result = await query
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    RecordType = c.RecordType.Name
                })
                .ToListAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    RecordType = c.RecordType.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null) return NotFound();

            return Ok(result);
        }
    }
}
