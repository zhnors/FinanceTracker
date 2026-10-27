using FinanceTracker.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordsController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public RecordsController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    [HttpGet]
    public async Task<IReadOnlyList<RecordResponse>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.GetAllRecordsAsync(cancellationToken);

        return result.Select(r => r.ToResponse()).ToList();
    }


    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        return Ok(new 
        {   Income = await _financeService.IncomeAmountAsync(cancellationToken),

            Expense = await _financeService.ExpenseAmountAsync(cancellationToken),

            Total = await _financeService.TotalAmountAsync(cancellationToken) 
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecordResponse>> GetById(int id, CancellationToken cancellationToken)  
    {
        var result = await _financeService.GetRecordByIdAsync(id, cancellationToken);

        if (result is null) return NotFound();

        else return Ok(result.ToResponse());
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> SearchByCat(string category, CancellationToken cancellationToken)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.SearchByCategoryAsync(category, cancellationToken);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("by-date")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> SearchByDate(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.SearchByDateAsync(from, to, cancellationToken);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("top-expenses")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> TopExpenses(int count = 3, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.GetTopExpensesAsync(count, cancellationToken);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecord(int id, CancellationToken cancellationToken) 
    {
        var result = await _financeService.DeleteRecordAsync(id, cancellationToken);

        if (result) return NoContent();

        else return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<RecordResponse>> Create(CreateRecordRequest request, CancellationToken cancellationToken)
    {
        var record = request.ToRecord();
            
        await _financeService.AddRecordAsync(record, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record.ToResponse());        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateRecordRequest request, CancellationToken cancellationToken)
    {
        var existing = await _financeService.GetRecordByIdAsync(id, cancellationToken);

        if (existing is null) return NotFound();

        var updated = request.ToRecord(existing);

        await _financeService.UpdateRecordAsync(updated, cancellationToken);

        return NoContent();    
    }
}