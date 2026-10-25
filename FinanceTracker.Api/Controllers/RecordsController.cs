using FinanceTracker;
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
    public async Task<IReadOnlyList<RecordResponse>> GetAll()
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.GetAllRecordsAsync();

        return result.Select(r => r.ToResponse()).ToList();
    }


    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        return Ok(new 
        {   Income = await _financeService.IncomeAmountAsync(),

            Expense = await _financeService.ExpenseAmountAsync(),

            Total = await _financeService.TotalAmountAsync() 
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecordResponse>> GetById(int id)  
    {
        var result = await _financeService.GetRecordByIdAsync(id);

        if (result is null) return NotFound();

        else return Ok(result.ToResponse());
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> SearchByCat(string category)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.SearchByCategoryAsync(category);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("by-date")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> SearchByDate(DateTime from, DateTime to)
    {
        try
        {
            IReadOnlyList<FinancialRecord> result = await _financeService.SearchByDateAsync(from, to);

            return Ok(result.Select(r => r.ToResponse()).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("top-expenses")]
    public async Task<ActionResult<IReadOnlyList<RecordResponse>>> TopExpenses(int count = 3)
    {
        IReadOnlyList<FinancialRecord> result = await _financeService.GetTopExpensesAsync(count);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecord(int id) 
    {
        var result = await _financeService.DeleteRecordAsync(id);

        if (result) return NoContent();

        else return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<RecordResponse>> Create(CreateRecordRequest request)
    {
        try
        {
            var record = request.ToRecord();
            
            await _financeService.AddRecordAsync(record);

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record.ToResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateRecordRequest request)
    {
        var existing = await _financeService.GetRecordByIdAsync(id);

        if (existing is null) return NotFound();

        try
        {
            var updated = request.ToRecord(existing);

            await _financeService.UpdateRecordAsync(updated);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}