using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BiometricSimulator.WebApp.Models;
using BiometricSimulator.WebApp.Entities;
using BiometricSimulator.WebApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiometricSimulator.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterEmployee(string proxyCode)
    {
        if (string.IsNullOrWhiteSpace(proxyCode))
        {
            TempData["ErrorMessage"] = "Proxy code is required.";
            return RedirectToAction("Registration");
        }

        // Check if proxy code already exists
        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.ProxyCode == proxyCode);

        if (existingEmployee != null)
        {
            TempData["ErrorMessage"] = "This proxy code already exists.";
            return RedirectToAction("Registration");
        }

        var employee = new Employee
        {
            ProxyCode = proxyCode
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Employee registered successfully!";
        return RedirectToAction("Registration");
    }

    public IActionResult BioPunchSimulate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RecordPunch(int employeeId, DateTime timestamp)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            TempData["ErrorMessage"] = "Employee not found.";
            return RedirectToAction("BioPunchSimulate");
        }

        var activityLog = new ActivityLog
        {
            EmployeeId = employeeId,
            Timestamp = timestamp,
            IsProcessed = false,
            DeviceCode = null
        };

        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Punch recorded successfully!";
        return RedirectToAction("BioPunchSimulate");
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .OrderBy(e => e.ProxyCode)
            .Select(e => new { e.Id, e.ProxyCode })
            .ToListAsync();

        return Json(employees);
    }
}