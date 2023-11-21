using Microsoft.AspNetCore.Mvc;
using VerificationApp.Models;

namespace VerificationApp.ViewComponents
{
  /// <summary>
  /// A view component for creating an <see cref="InspectingOrganization"/>.
  /// </summary> 
  public class CreateInspectingOrganization : ViewComponent
  {    
    public IViewComponentResult Invoke()
    {
      var inspectingOrganization = new InspectingOrganization { Id = Guid.NewGuid().ToString() };
      return View(inspectingOrganization);
    }
  }
}
