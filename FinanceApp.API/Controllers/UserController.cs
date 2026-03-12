using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs.Auth;
using FinanceApp.API.Models;
using FinanceApp.API.Services;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly PasswordService _passwordService;

    public UserController(FinanceDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

        if(user == null)
        {
            return Unauthorized();
        }

        var passwordService = new PasswordService();

        var isValid = passwordService.VerifyPassword(user.PasswordHash, dto.Password);

        if(!isValid)
        {
            return Unauthorized();
        }

        return Ok("Login Success");
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
        {
            return Conflict(new { message = "Email already exists." });
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new
        {
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt
        });
    }
}
