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

        ApplyTransactionToWalletBalance(wallet, dto.Type, dto.Amount);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            message = "Transaction berhasil dibuat.",
            data = new
            {
                transaction.Id,
                transaction.WalletId,
                WalletName = wallet.Name,
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
        .Where(w => w.UserId == userId && w.DeletedAt == null)
        .Select(w => new TransactionResponseDto
        {
            Id = w.Id,
            WalletId = w.WalletId,
            WalletName = w.Wallet.Name,
            Amount = w.Amount,
            Type = w.Type,
            Note = w.Note,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        }).ToListAsync(cancellationToken);

        return Ok(transaction);
    }

    // Get Transaction By Id
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var transaction = await _context.Transactions
        .Where(w => w.Id == id && w.UserId == userId && w.DeletedAt == null)
        .Select(w => new TransactionResponseDto
        {
            Id = w.Id,
            WalletId = w.WalletId,
            WalletName = w.Wallet.Name,
            Amount = w.Amount,
            Type = w.Type,
            Note = w.Note,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        })
        .FirstOrDefaultAsync(cancellationToken);

        return Ok(transaction);
    }

    // Patch Transaction By Id
    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionDto dto, Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var transaction = await _context.Transactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId && w.DeletedAt == null, cancellationToken);

        if (transaction is null)
        {
            return NotFound(new { message = "Transaksi tidak ditemukan." });
        }

        var originalWallet = transaction.Wallet;
        var originalType = transaction.Type;
        var originalAmount = transaction.Amount;
        var targetWallet = originalWallet;

        if (dto.WalletId.HasValue && dto.WalletId.Value != transaction.WalletId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == dto.WalletId.Value && w.UserId == userId, cancellationToken);

            if (wallet is null)
            {
                return NotFound(new { message = "Wallet tidak ditemukan." });
            }

            targetWallet = wallet;
        }

        if (dto.Amount.HasValue)
        {
            if (dto.Amount.Value <= 0)
            {
                return BadRequest(new { message = "Amount harus lebih dari 0." });
            }

            transaction.Amount = dto.Amount.Value;
        }

        if (dto.Type.HasValue)
        {
            transaction.Type = dto.Type.Value;
        }

        if (dto.Note is not null)
        {
            transaction.Note = dto.Note;
        }

        RevertTransactionFromWalletBalance(originalWallet, originalType, originalAmount);

        transaction.WalletId = targetWallet.Id;
        transaction.Wallet = targetWallet;

        ApplyTransactionToWalletBalance(targetWallet, transaction.Type, transaction.Amount);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            // WalletId = transaction.WalletId,
            WalletName = transaction.Wallet.Name,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Ok(response);
    }

    // Delete Transaction By Id
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteTransaction(Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var transaction = await _context.Transactions
        .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId && w.DeletedAt == null, cancellationToken);

        if (transaction is null)
        {
            return NotFound(new { message = "Transaksi tidak ditemukan." });
        }

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == transaction.WalletId && w.UserId == userId, cancellationToken);

        if (wallet is null)
        {
            return NotFound(new { message = "Wallet tidak ditemukan." });
        }

        RevertTransactionFromWalletBalance(wallet, transaction.Type, transaction.Amount);
        transaction.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static void ApplyTransactionToWalletBalance(Wallet wallet, TransactionType type, decimal amount)
    {
        wallet.Balance += GetSignedAmount(type, amount);
    }

    private static void RevertTransactionFromWalletBalance(Wallet wallet, TransactionType type, decimal amount)
    {
        wallet.Balance -= GetSignedAmount(type, amount);
    }

    private static decimal GetSignedAmount(TransactionType type, decimal amount)
    {
        return type == TransactionType.Income ? amount : -amount;
    }
}
