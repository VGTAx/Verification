using System.ComponentModel.DataAnnotations;

namespace VerificationApp.Models
{
  /// <summary>
  /// Verification model
  /// </summary>
  public class Verification
  {
    /// <summary>
    /// ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Business entity name
    /// </summary>
    [Display(Name = "Проверяемый субъект")]
    public string? BusinessEntity { get; set; }
    /// <summary>
    /// Inspecting organization name
    /// </summary>
    [Display(Name = "Проверяющая организация")]
    public string? Organization { get; set; }
    /// <summary>
    /// Begin period verification
    /// </summary>
    [Display(Name = "Начало периода")]
    public DateOnly BeginPeriod { get; set; }
    /// <summary>
    /// End period verification
    /// </summary>
    [Display(Name = "Конец периода")]
    public DateOnly EndPeriod { get; set; }
    /// <summary>
    /// Duration verification
    /// </summary>
    [Display(Name = "Длительность (дней)")]
    public int Duration { get; set; }
  }
}
