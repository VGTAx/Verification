using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationApp.Data;
using VerificationApp.Models;

namespace VerificationApp.Controllers
{
  /// <summary>
  /// Controller for managing <see cref="InspectingOrganization"/>
  /// </summary>
  public class InspectingOrganizationController : Controller
  {
    private readonly VerificationAppContext _context;

    public InspectingOrganizationController(VerificationAppContext context)
    {
      _context = context;
    }
    /// <summary>
    /// Get collection <see cref="InspectingOrganization"/>
    /// </summary>
    /// <returns>View with collection <see cref="InspectingOrganization"/> </returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var inspectingOrganization = 
        await _context.InspectingOrganizations.ToListAsync();

      return View(inspectingOrganization);
    }
    /// <summary>
    /// Display information about <see cref="InspectingOrganization"/>.
    /// </summary>
    /// <param name="id">Id object <see cref="InspectingOrganization"/></param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Details"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
      if (!await IsInspectingOrganizationExistAsync(id))
      {
        return NotFound();
      }

      var inspectingOrganization = 
        await _context.InspectingOrganizations.FirstOrDefaultAsync(x => x.Id == id);

      return View(inspectingOrganization);
    }

    /// <summary>
    /// Create <see cref="InspectingOrganization"/> object
    /// </summary>
    /// <param name="model"><see cref="InspectingOrganization"/> model containing the data to create object.</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Create"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind] InspectingOrganization model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (await IsInspectionOrganizationNameAvailableAsync(model.Name!))
      {
        ModelState.AddModelError("Name", "Проверяющая организация уже добавлена!");
        return BadRequest(ModelState);
      }

      await _context.InspectingOrganizations.AddAsync(model);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Return partail view to edit <see cref="InspectingOrganization"/> object
    /// </summary>
    /// <param name="id">ID <see cref="InspectingOrganization"/> object</param>
    /// <returns>Returns PartialView or HTTP status code indicating the result of the <see cref="Edit(string)"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
      if (!await IsInspectingOrganizationExistAsync(id))
      {
        return NotFound();
      }
      var inspectingOrganization =
        await _context.InspectingOrganizations.FirstAsync(x => x.Id == id);

      return PartialView(inspectingOrganization);
    }

    /// <summary>
    /// Edit <see cref="InspectingOrganization"/> object
    /// </summary>
    /// <param name="model"><see cref="InspectingOrganization"/> model containing the data to edit object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Edit(InspectingOrganization)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind] InspectingOrganization model)
    {
      if(!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var inspectingOrganization = await _context.InspectingOrganizations
          .AsNoTracking()
          .FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);

      if (inspectingOrganization is not null)
      {
        ModelState.AddModelError("Name", "Название организации уже занято!");
        return BadRequest(ModelState);
      }

      _context.InspectingOrganizations.Update(model);
      await _context.SaveChangesAsync();

      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Return view to delete <see cref="InspectingOrganization"/> object
    /// </summary>
    /// <param name="id">ID <see cref="InspectingOrganization"/> object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(string)"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
      if (!await IsInspectingOrganizationExistAsync(id))
      {
        return NotFound();
      }

      var inspectingOrganization = 
          await _context.InspectingOrganizations.FirstOrDefaultAsync(e => e.Id == id);

      return View(inspectingOrganization);
    }

    /// <summary>
    /// Delete <see cref="InspectingOrganization"/> object
    /// </summary>
    /// <param name="model"><see cref="InspectingOrganization"/> model containing the data to delete object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(InspectingOrganization)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([Bind] InspectingOrganization model)
    {
      if (!ModelState.IsValid) 
      {
        return BadRequest(ModelState);
      }
      _context.InspectingOrganizations.Remove(model);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Check avalaible name or not
    /// </summary>
    /// <param name="name">Checking name</param>
    /// <returns>True if name is available, else false</returns>    
    private async Task<bool> IsInspectionOrganizationNameAvailableAsync(string name) =>
        await _context.InspectingOrganizations.AnyAsync(x => x.Name == name);

    /// <summary>
    /// Check <see cref="InspectingOrganization"/> object exist or not
    /// </summary>
    /// <param name="Id">ID <see cref="InspectingOrganization"/> object</param>
    /// <returns>True if organizaton exists, else false</returns>
    private async Task<bool> IsInspectingOrganizationExistAsync(string Id) => 
        await _context.InspectingOrganizations.AnyAsync(x => x.Id == Id);


    
  }
}
