using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FinanceApp.API.DTOs.Transaction;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public TransactionController(FinanceDbContext context)
    {
        _context = context;
    }

    // Create Transaction By User
    [HttpPost("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto, Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token user tidak valid." });
        }

        if (dto.Amount <= 0)
        {
            return BadRequest(new { message = "Amount harus lebih dari 0." });
        }

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);

        if (wallet is null)
        {
            return NotFound(new { message = "Wallet tidak ditemukan." });
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WalletId = wallet.Id,
            Amount = dto.Amount,
            Type = dto.Type,
            Note = dto.Note
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            message = "Transaction berhasil dibuat.",
            data = new
            {
                transaction.Id,
                transaction.WalletId,
                transaction.Amount,
                transaction.Type,
                transaction.Note,
                transaction.CreatedAt,
                transaction.UpdatedAt
            }
        }
        );
    }
    
    // Get All Transaction by User
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyTransaction(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var transaction = await _context.Transactions
        .Where(w => w.UserId == userId)
        .Select(w => new Transaction
        {
            Id= w.Id,
            WalletId = w.WalletId,
            Amount = w.Amount,
            Type = w.Type,
            Note = w.Note,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        }).ToListAsync(cancellationToken);

        return Ok(transaction);
    }

    // Get Transaction By Id
    // Patch Transaction By Id
    // Delete Transaction By Id

}
