using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using FinanceApp.API.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public DashboardController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var total = await _context.Wallets
        .Where(w => w.UserId == userId)
        .SumAsync(w => (decimal?)w.Balance, cancellationToken) ?? 0;

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = monthStart.AddMonths(1);

        var totalIncomeThisMonth = await _context.Transactions
            .Where(t =>
                t.UserId == userId &&
                t.DeletedAt == null &&
                t.Type == TransactionType.Income &&
                t.CreatedAt >= monthStart &&
                t.CreatedAt < nextMonthStart)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        var totalExpenseThisMonth = await _context.Transactions
            .Where(t =>
                t.UserId == userId &&
                t.DeletedAt == null &&
                t.Type == TransactionType.Expense &&
                t.CreatedAt >= monthStart &&
                t.CreatedAt < nextMonthStart)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        var recentTransactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.DeletedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new DashboardRecentTransactionDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                WalletName = t.Wallet.Name,
                Amount = t.Amount,
                Type = t.Type,
                Note = t.Note,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var walletSummaries = await _context.Wallets
            .Where(w => w.UserId == userId)
            .Select(w => new DashboardWalletSummaryDto
            {
                WalletId = w.Id,
                WalletName = w.Name,
                Balance = w.Balance,
                TransactionCount = w.Transactions.Count(t => t.DeletedAt == null),
                IncomeThisMonth = w.Transactions
                    .Where(t =>
                        t.DeletedAt == null &&
                        t.Type == TransactionType.Income &&
                        t.CreatedAt >= monthStart &&
                        t.CreatedAt < nextMonthStart)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                ExpenseThisMonth = w.Transactions
                    .Where(t =>
                        t.DeletedAt == null &&
                        t.Type == TransactionType.Expense &&
                        t.CreatedAt >= monthStart &&
                        t.CreatedAt < nextMonthStart)
                    .Sum(t => (decimal?)t.Amount) ?? 0
            })
            .OrderByDescending(w => w.Balance)
            .ToListAsync(cancellationToken);

        var response = new DashboardResponseDto
        {
            TotalBalance = total,
            TotalIncomeThisMonth = totalIncomeThisMonth,
            TotalExpenseThisMonth = totalExpenseThisMonth,
            RecentTransactions = recentTransactions,
            WalletSummaries = walletSummaries
        };

        return Ok(response);
    }

}
