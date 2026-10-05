using LibrarySystem.Api.DTOs.Loans;
using LibrarySystem.Api.Services.Interfaces;
using LibrarySystem.Api.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<ActionResult<LoanResponse>> Create(
        CreateLoanRequest request)
    {
        var result = await _loanService.CreateAsync(request);

        return result.Error switch
        {
            CreateLoanError.UserNotFound =>
                Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found",
                    detail: "The specified user does not exist."),

            CreateLoanError.BookNotFound =>
                Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Book not found",
                    detail: "The specified book does not exist."),

            CreateLoanError.BookNotAvailable =>
                Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Book is not available",
                    detail: "The book is currently on loan."),

            null when result.Loan is not null =>
                CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Loan.Id },
                    result.Loan),

            _ =>
                StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanResponse>> GetById(Guid id)
    {
        var loan = await _loanService.GetByIdAsync(id);

        if (loan is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Loan not found",
                detail: "The specified loan does not exist.");
        }

        return Ok(loan);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<LoanResponse>>> GetActive()
    {
        var loans = await _loanService.GetActiveAsync();

        return Ok(loans);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<LoanResponse>> Return(Guid id)
    {
        var result = await _loanService.ReturnAsync(id);

        return result.Error switch
        {
            ReturnLoanError.LoanNotFound =>
                Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Loan not found",
                    detail: "The specified loan does not exist."),

            ReturnLoanError.BookNotFound =>
                Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Book not found",
                    detail: "The book associated with the loan does not exist."),

            ReturnLoanError.LoanAlreadyReturned =>
                Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Loan already returned",
                    detail: "The loan has already been returned."),

            null when result.Loan is not null =>
                Ok(result.Loan),

            _ =>
                StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}