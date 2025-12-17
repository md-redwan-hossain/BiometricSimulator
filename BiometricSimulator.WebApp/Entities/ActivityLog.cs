namespace BiometricSimulator.WebApp.Entities;

public class ActivityLog
{
  public int Id { get; }
  public required int EmployeeId { get; set; }
  public required DateTime Timestamp { get; set; }
  public required bool IsProcessed { get; set; }
  public required string? DeviceCode { get; set; }
}
