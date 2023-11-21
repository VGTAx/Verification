using System.ComponentModel.DataAnnotations;

namespace VerificationApp.Models
{
  /// <summary>
  /// SmallBusinessEntity model
  /// </summary>
  public class SmallBusinessEntity
  {
    /// <summary>
    /// ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [Display(Name = "Название субъекта")]
    [Required(ErrorMessage = "Введите название субъекта")]    
    public string? Name { get; set; } 
  }
}
