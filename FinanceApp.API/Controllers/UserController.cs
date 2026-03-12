using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.Models;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public UserController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [HttpPost]
    public IActionResult CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }
}