using BiometricSimulator.WebApp.Entities;
using BiometricSimulator.WebApp.Models;
using BiometricSimulator.WebApp.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricSimulator.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ActivityController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("unprocessed")]
    public async Task<ActionResult<IEnumerable<UnprocessedActivityDto>>> GetUnprocessedActivities()
    {
        var unprocessedActivities = await (from activityLog in _context.ActivityLogs
                join employee in _context.Employees
                    on activityLog.EmployeeId equals employee.Id
                where !activityLog.IsProcessed
                select new UnprocessedActivityDto
                {
                    ProxyCode = employee.ProxyCode,
                    Timestamp = activityLog.Timestamp
                })
            .ToListAsync();

        return Ok(unprocessedActivities);
    }

    [HttpPost("punch")]
    public async Task<ActionResult<ActivityLog>> RecordPunch([FromBody] PunchDto dto)
    {
        // Validate employee exists
        var employee = await _context.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
        {
            return NotFound(new { message = "Employee not found" });
        }

        var activityLog = new ActivityLog
        {
            EmployeeId = dto.EmployeeId,
            Timestamp = dto.Timestamp,
            DeviceCode = null,
            IsProcessed = false
        };

        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnprocessedActivities), new { id = activityLog.Id }, activityLog);
    }
}