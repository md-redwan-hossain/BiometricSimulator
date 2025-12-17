namespace BiometricSimulator.WebApp.Models;

public class PunchDto
{
    public required int EmployeeId { get; set; }
    public required DateTime Timestamp { get; set; }
}