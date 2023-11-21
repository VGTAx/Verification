using System.ComponentModel.DataAnnotations;

namespace VerificationApp.ViewModels
{
  /// <summary>
  /// View model for importing data.
  /// </summary>
  public class ImportVerificationViewModel
  {
    /// <summary>
    /// File for import
    /// </summary>
    [Required(ErrorMessage = "Выберите файл!")]
    public IFormFile? FileInput { get; set; }
    /// <summary>
    /// checks whether it is necessary to remove duplicates.
    /// </summary>
    public bool CheckDuplicate { get; set; }
    /// <summary>
    /// checks whether it is necessary to remove records with incorrect values.
    /// </summary>
    public bool DeleteIncorrectValue { get; set; }
  }
}
