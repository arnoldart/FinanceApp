using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs.Auth;
using FinanceApp.API.Models;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public UserController(FinanceDbContext context, PasswordService passwordService, JwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    [Authorize]
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

        var isValid = _passwordService.VerifyPassword(user.PasswordHash, dto.Password);

        if(!isValid)
        {
            return Unauthorized();
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return Ok(new
        {
            token
        });
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
