using FinanceTracker;
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
    public IReadOnlyList<FinancialRecord> GetAll()
    {
        return _financeService.GetAllRecords();
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
    public ActionResult<FinancialRecord> GetById(int id)  
    {
        var result = _financeService.GetRecordById(id);

        if (result is null) return NotFound();

        else return Ok(result);
    }

    [HttpGet("by-category")]
    public ActionResult<IReadOnlyList<FinancialRecord>> SearchByCat(string category)
    {
        return Ok(_financeService.SearchByCategory(category));
    }

    [HttpGet("by-date")]
    public ActionResult<IReadOnlyList<FinancialRecord>> SearchByDate(DateTime from, DateTime to)
    {
        try
        {
            return Ok(_financeService.SearchByDate(from, to));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("top-expenses")]
    public ActionResult<IReadOnlyList<FinancialRecord>> TopExpenses(int count = 3)
    {
        return Ok(_financeService.GetTopExpenses(count));
    }


}