using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using FinanceApp.API.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
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
        const int recentTransactionLimit = 5;
        const int walletSummaryLimit = 3;
        const int assetTrendDays = 30;

        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var total = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .SumAsync(w => (decimal?)w.Balance, cancellationToken) ?? 0;

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = monthStart.AddMonths(1);

        var monthTotals = await _context.Transactions
            .AsNoTracking()
            .Where(t =>
                t.UserId == userId &&
                t.DeletedAt == null &&
                t.CreatedAt >= monthStart &&
                t.CreatedAt < nextMonthStart)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Income = g
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                Expense = g
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalIncomeThisMonth = monthTotals?.Income ?? 0;
        var totalExpenseThisMonth = monthTotals?.Expense ?? 0;

        var recentTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.DeletedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .Take(recentTransactionLimit)
            .Select(t => new DashboardRecentTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Note = t.Note
            })
            .ToListAsync(cancellationToken);

        var walletSummaries = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.Balance)
            .Take(walletSummaryLimit)
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
            .ToListAsync(cancellationToken);

        var assetTrendEndDate = now.Date;
        var assetTrendStartDate = assetTrendEndDate.AddDays(-(assetTrendDays - 1));
        var assetTrendNextDate = assetTrendEndDate.AddDays(1);

        var assetTrendTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t =>
                t.UserId == userId &&
                t.DeletedAt == null &&
                t.CreatedAt >= assetTrendStartDate &&
                t.CreatedAt < assetTrendNextDate)
            .Select(t => new
            {
                Date = t.CreatedAt.Date,
                t.Amount,
                t.Type
            })
            .ToListAsync(cancellationToken);

        var netChangeByDate = assetTrendTransactions
            .GroupBy(t => t.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount)
            );

        var netChangeInWindow = netChangeByDate.Values.Sum();
        var openingBalance = total - netChangeInWindow;
        var runningBalance = openingBalance;
        var assetTrend = new List<DashboardAssetTrendPointDto>(assetTrendDays);

        for (var dayIndex = 0; dayIndex < assetTrendDays; dayIndex++)
        {
            var date = assetTrendStartDate.AddDays(dayIndex);
            if (netChangeByDate.TryGetValue(date, out var netChange))
            {
                runningBalance += netChange;
            }

            assetTrend.Add(new DashboardAssetTrendPointDto
            {
                Date = date,
                Balance = runningBalance
            });
        }

        var response = new DashboardResponseDto
        {
            TotalBalance = total,
            TotalIncomeThisMonth = totalIncomeThisMonth,
            TotalExpenseThisMonth = totalExpenseThisMonth,
            RecentTransactions = recentTransactions,
            WalletSummaries = walletSummaries,
            AssetTrend = assetTrend
        };

        return Ok(response);
    }

}
