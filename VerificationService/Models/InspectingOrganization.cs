using System.ComponentModel.DataAnnotations;

namespace VerificationApp.Models
{
  /// <summary>
  /// InspectingOrganization model
  /// </summary>
  public class InspectingOrganization
  {
    /// <summary>
    /// ID
    /// </summary>
    [Required]
    public string? Id { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    [Display(Name = "Название организации")]
    [Required(ErrorMessage = "Введите название организации")]
    public string? Name { get; set; }
  }
}
