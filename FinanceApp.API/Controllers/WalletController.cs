using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public WalletController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetWallets()
    {
        return Ok(_context.Wallets.ToList());
    }

    [HttpPost]
    public IActionResult CreateWallet(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        _context.SaveChanges();

        return Ok(wallet);
    }
}