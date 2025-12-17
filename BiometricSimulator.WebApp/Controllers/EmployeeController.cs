using BiometricSimulator.WebApp.Entities;
using BiometricSimulator.WebApp.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricSimulator.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public EmployeeController(ApplicationDbContext context)
  {
    _context = context;
  }
  
  [HttpPost("register")]
  public async Task<ActionResult<Employee>> Register([FromBody] RegisterEmployeeDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.ProxyCode))
    {
      return BadRequest(new { message = "Proxy code is required" });
    }

    // Check if proxy code already exists
    var existingEmployee = await _context.Employees
        .FirstOrDefaultAsync(e => e.ProxyCode == dto.ProxyCode);

    if (existingEmployee != null)
    {
      return Conflict(new { message = "Proxy code already exists" });
    }

    var employee = new Employee
    {
      ProxyCode = dto.ProxyCode
    };

    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
  }

  /// <summary>
  /// Get all employees
  /// </summary>
  [HttpGet("all")]
  public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
  {
    var employees = await _context.Employees
        .OrderBy(e => e.ProxyCode)
        .ToListAsync();

    return Ok(employees);
  }

  /// <summary>
  /// Get employee by ID
  /// </summary>
  [HttpGet("{id}")]
  public async Task<ActionResult<Employee>> GetById(int id)
  {
    var employee = await _context.Employees.FindAsync(id);

    if (employee == null)
    {
      return NotFound();
    }

    return Ok(employee);
  }
}

public class RegisterEmployeeDto
{
  public string ProxyCode { get; set; } = string.Empty;
}
