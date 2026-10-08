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
    public IReadOnlyList<FinancialRecord> Get()
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
}