namespace VerificationApp.ViewModels
{
  /// <summary>
  /// View model for filtering data
  /// </summary>
  public class FilterViewModel
  {
    /// <summary>
    /// Business entity name
    /// </summary>
    public string? Business { get; set; } = "";
    /// <summary>
    /// Inspecting organization name
    /// </summary>
    public string? Org { get; set; } = "";
    /// <summary>
    /// Begin period verification
    /// </summary>
    public DateOnly BegPeriod { get; set; } = default;
    /// <summary>
    /// End period verification
    /// </summary>
    public DateOnly EndPeriods { get; set; } = default;
    /// <summary>
    /// Number page for pagination
    /// </summary>
    public int Page { get; set; } = 1;
  }
}
