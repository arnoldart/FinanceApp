using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using FinanceApp.API.DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public WalletController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    [EnableRateLimiting("wallet-read")]
    public async Task<IActionResult> GetMyWallets(CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var wallet = await _context.Wallets
        .Where(w => w.UserId == userId)
        .Select(w => new WalletResponseDto
        {
            Id = w.Id,
            Name = w.Name,
            Balance = w.Balance,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        })
        .ToListAsync(cancellationToken);

        return Ok(wallet);
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("wallet-write")]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "Nama wallet wajib diisi." });
        }

        if (dto.Balance < 0)
        {
            return BadRequest(new { message = "Saldo awal tidak boleh negatif" });
        }

        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token user tidak valid." });
        }

        var userExist = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExist)
        {
            return Unauthorized(new { message = "User tidak ditemukan" });
        }

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name.Trim(),
            Balance = dto.Balance
        };

        _context.Wallets.Add(wallet);
        _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetWalletById),
            new { id = wallet.Id },
            ToDto(wallet)
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [EnableRateLimiting("wallet-read")]
    public async Task<IActionResult> GetWalletById(Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var wallet = await _context.Wallets
        .Where(w => w.Id == id && w.UserId == userId)
        .Select(w => new WalletResponseDto
        {
            Id = w.Id,
            Name = w.Name,
            Balance = w.Balance,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        })
        .FirstOrDefaultAsync(cancellationToken);

        return wallet is null ? NotFound() : Ok(wallet);
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    [EnableRateLimiting("wallet-write")]
    public async Task<IActionResult> UpdateWallet(Guid id, [FromBody] UpdateWalletDto dto, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var wallet = await _context.Wallets
        .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);

        if (dto.Name is not null)
        {
            var trimmed = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return BadRequest(new { message = "Nama Wallet tidak boleh kosong" });
            }

            wallet.Name = trimmed;
        }

        if (dto.Balance < 0)
        {
            return BadRequest(new { message = "Balance tidak boleh negatif." });
        }

        wallet.Balance = dto.Balance;

        await _context.SaveChangesAsync(cancellationToken);

        var response = new WalletResponseDto
        {
            Id = wallet.Id,
            Name = wallet.Name,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [EnableRateLimiting("wallet-write")]
    public async Task<IActionResult> DeleteWallet(Guid id, CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var wallet = await _context.Wallets
        .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == id);

        if(wallet is null)
        {
            return NotFound(new { message = "Wallet tidak ditemukan." });
        }

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static WalletResponseDto ToDto(Wallet w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Balance = w.Balance,
        CreatedAt = w.CreatedAt,
        UpdatedAt = w.UpdatedAt
    };

}
