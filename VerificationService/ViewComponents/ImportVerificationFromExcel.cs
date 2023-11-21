using Microsoft.AspNetCore.Mvc;
using VerificationApp.ViewModels;

namespace VerificationApp.ViewComponents
{
  /// <summary>
  /// A view component for import data from Excel
  /// </summary>
  public class ImportVerificationFromExcel : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      var model = new ImportVerificationViewModel();
      return View(model);
    }
  }
}
