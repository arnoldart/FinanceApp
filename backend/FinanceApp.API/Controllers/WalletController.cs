using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

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
    [EnableRateLimiting("wallet-read")]
    public IActionResult GetWallets()
    {
        return Ok(_context.Wallets.ToList());
    }

    [HttpPost]
    [EnableRateLimiting("wallet-write")]
    public IActionResult CreateWallet(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        _context.SaveChanges();

        return Ok(wallet);
    }
}
