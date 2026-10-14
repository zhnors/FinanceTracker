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
    public IReadOnlyList<RecordResponse> GetAll()
    {
        IReadOnlyList<FinancialRecord> result = _financeService.GetAllRecords();

        return result.Select(r => r.ToResponse()).ToList();
    }


    [HttpGet("summary")]
    public IActionResult Summary()
    {
        return Ok(new 
        {   Income = _financeService.IncomeAmount(),
            Expense = _financeService.ExpenseAmount(),
            Total = _financeService.TotalAmount() 
        });
    }

    [HttpGet("{id}")]
    public ActionResult<RecordResponse> GetById(int id)  
    {
        var result = _financeService.GetRecordById(id);

        if (result is null) return NotFound();

        else return Ok(result.ToResponse());
    }

    [HttpGet("by-category")]
    public ActionResult<IReadOnlyList<RecordResponse>> SearchByCat(string category)
    {
        IReadOnlyList<FinancialRecord> result = _financeService.SearchByCategory(category);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("by-date")]
    public ActionResult<IReadOnlyList<RecordResponse>> SearchByDate(DateTime from, DateTime to)
    {
        try
        {
            IReadOnlyList<FinancialRecord> result = _financeService.SearchByDate(from, to);

            return Ok(result.Select(r => r.ToResponse()).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("top-expenses")]
    public ActionResult<IReadOnlyList<RecordResponse>> TopExpenses(int count = 3)
    {
        IReadOnlyList<FinancialRecord> result = _financeService.GetTopExpenses(count);

        return Ok(result.Select(r => r.ToResponse()).ToList());
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteRecord(int id) 
    {
        var result = _financeService.DeleteRecord(id);
        if (result) return NoContent();
        else return NotFound();
    }

    [HttpPost]
    public ActionResult<RecordResponse> Create(CreateRecordRequest request)
    {
        try
        {
            var record = request.ToRecord();
            if (_financeService.GetRecordById(record.Id) != null) return Conflict();
            _financeService.AddRecord(record);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record.ToResponse());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRecordRequest request)
    {
        var existing = _financeService.GetRecordById(id);
        if (existing is null) return NotFound();

        try
        {
            var updated = request.ToRecord(existing);
            _financeService.UpdateRecord(updated);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}