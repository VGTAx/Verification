using System.ComponentModel.DataAnnotations;
using VerificationApp.Models;

namespace VerificationApp.ViewModels
{
  public class VerificationViewModel
  {
    /// <summary>
    /// ID
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// List of business entites for checking
    /// </summary>
    [Display(Name = "Проверяемый субъект")]
    public List<string> BusinessEntitiesList { get; set; } = new List<string>();
    /// <summary>
    /// List of inspecting organuzations
    /// </summary>
    [Display(Name = "Проверяющая организация")]
    public List<string> OrganizationsList { get; set; } = new List<string>();
    /// <summary>
    /// Business entity name
    /// </summary>
    [Display(Name = "Проверяемый субъект")]
    [Required(ErrorMessage = "Выберите субъект!")]
    public string? BusinessEntity { get; set; }
    /// <summary>
    /// Inspecting organization name
    /// </summary>
    [Display(Name = "Проверяющая организация")]
    [Required(ErrorMessage = "Выберите организацию!")]
    public string? Organization { get; set; }
    /// <summary>
    /// Begin period of verification
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Начало периода")]    
    [Required(ErrorMessage = "Укажите начало периода")]
    public DateOnly BeginPeriod { get; set; }
    /// <summary>
    /// End period of verification
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Конец периода")]    
    [Required(ErrorMessage = "Укажите начало периода")]
    public DateOnly EndPeriod { get; set; }
    /// <summary>
    /// Duration of verification
    /// </summary>
    [Display(Name = "Длительность")]
    [Required(ErrorMessage = "Укажите длительность проверки")]
    public int Duration { get; set; }

  }
}
