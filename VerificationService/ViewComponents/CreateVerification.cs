using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationApp.Data;
using VerificationApp.ViewModels;
using VerificationApp.Models;

namespace VerificationApp.ViewComponents
{
  /// <summary>
  /// A view component for creating an <see cref="Verification"/>.
  /// </summary> 
  public class CreateVerification : ViewComponent
  {
    private readonly VerificationAppContext _context;

    public CreateVerification(VerificationAppContext context)
    {
      _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      var smallBusinessEntities = await _context.SmallBusinessEntities
          .Select(s => s.Name)
          .ToListAsync();

      var organizations = await _context.InspectingOrganizations
          .Select(s => s.Name)
          .ToListAsync();

      var verification = new VerificationViewModel 
      { 
        Id = Guid.NewGuid().ToString(),
        BusinessEntitiesList = smallBusinessEntities!,
        OrganizationsList = organizations!       
      };
      return View(verification);
    }
  }
}
