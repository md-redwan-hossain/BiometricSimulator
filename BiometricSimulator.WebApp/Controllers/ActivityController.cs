using System.ComponentModel.DataAnnotations;
using BiometricSimulator.WebApp.Entities;
using BiometricSimulator.WebApp.Models;
using BiometricSimulator.WebApp.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    [HttpGet("pending")]
    public async Task<IActionResult> GetUnprocessedActivities(
        [FromQuery, BindRequired] bool forward, [FromQuery] int? cursor,
        [FromQuery, BindRequired, Range(1, int.MaxValue)]
        int limit)
    {
        var query = from activityLog in _context.ActivityLogs
            join employee in _context.Employees
                on activityLog.EmployeeId equals employee.Id
            where !activityLog.IsProcessed
            select new UnprocessedActivityDto
            {
                ProxyCode = employee.ProxyCode,
                Timestamp = activityLog.Timestamp,
                Id = activityLog.Id
            };

        if (forward)
        {
            query = query.OrderBy(x => x.Id);

            if (cursor.HasValue)
            {
                query = query.Where(x => x.Id > cursor.Value);
            }
        }
        
        else
        {
            query = query.OrderByDescending(x => x.Id);

            if (cursor.HasValue)
            {
                query = query.Where(x => x.Id < cursor.Value);
            }
        }

        var data = await query
            .Take(limit + 1)
            .ToListAsync();

        var hasMore = data.Count > limit;

        if (hasMore)
        {
            data.RemoveAt(data.Count - 1);
        }

        return Ok(new CursorPagedData<UnprocessedActivityDto, int>
        {
            Payload = data,
            NextCursor = hasMore ? data[^1].Id : null,
            PreviousCursor = data.Any() ? data[0].Id : null
        });
    }

    [HttpPost("mark-as-processed")]
    public async Task<ActionResult<IEnumerable<UnprocessedActivityDto>>> MarkProcessedActivities(
        [FromBody] HashSet<int> ids)
    {
        var count = await _context.ActivityLogs.Where(x => ids.Contains(x.Id) && !x.IsProcessed)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(genericPlan => genericPlan.IsProcessed, true));

        return count > 0 ? Ok() : BadRequest("No activities processed");
    }

    [HttpPost("")]
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