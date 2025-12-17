namespace BiometricSimulator.WebApp.Models;

public class UnprocessedActivityDto
{
  public required int Id { get; set; } 
  public required string ProxyCode { get; set; } 
  public required DateTime Timestamp { get; set; }
}
