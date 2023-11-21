using Microsoft.AspNetCore.Mvc;
using VerificationApp.Models;

namespace VerificationApp.ViewComponents
{
  /// <summary>
  /// A view component for creating an <see cref="SmallBusinessEntity"/>.
  /// </summary>  
  public class CreateSmallBusinessEntity : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      var smallBusinessEntity = new SmallBusinessEntity { Id = Guid.NewGuid().ToString() };
      return View(smallBusinessEntity);
    }
  }
}
