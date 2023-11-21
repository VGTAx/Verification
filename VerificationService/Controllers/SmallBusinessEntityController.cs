using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationApp.Data;
using VerificationApp.Models;

namespace VerificationApp.Controllers
{
  /// <summary>
  /// Controller for managing <see cref="SmallBusinessEntity"/>
  /// </summary>
  public class SmallBusinessEntityController : Controller
  {
    private readonly VerificationAppContext _context;
    
    public SmallBusinessEntityController(VerificationAppContext context)
    {
      _context = context;
    }
    /// <summary>
    /// Get collection <see cref="SmallBusinessEntity"/>
    /// </summary>
    /// <returns>View with collection <see cref="SmallBusinessEntity"/> </returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var smallBusinessEntities = 
        await _context.SmallBusinessEntities.ToListAsync();

      return View(smallBusinessEntities);
    }
    /// <summary>
    /// Display information about <see cref="SmallBusinessEntity"/>.
    /// </summary>
    /// <param name="id">Id  <see cref="SmallBusinessEntity"/> object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Details"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
      if(! await IsSmallBusinessEntityExistAsync(id))
      {
        return NotFound();
      }

      var smallBusinessEntity = 
        await _context.SmallBusinessEntities.FirstOrDefaultAsync(x => x.Id == id);

      return View(smallBusinessEntity);
    }

    /// <summary>
    /// Create <see cref="SmallBusinessEntity"/> object
    /// </summary>
    /// <param name="model"><see cref="SmallBusinessEntity"/> model containing the data to create object.</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Create"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind] SmallBusinessEntity model)
    {     
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }     

      if (await IsSmallBusinessEntityNameAvailableAsync(model.Name!)) 
      {
        ModelState.AddModelError("Name", "Название субъекта уже используется!");
        return BadRequest(ModelState);
      }

      await _context.SmallBusinessEntities.AddAsync(model);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index)); 
    }

    /// <summary>
    /// Return partail view to edit <see cref="SmallBusinessEntity"/> object
    /// </summary>
    /// <param name="id">ID <see cref="SmallBusinessEntity"/> object</param>
    /// <returns>Returns PartialView or HTTP status code indicating the result of the <see cref="Edit(string)"/> method</returns>
    [HttpGet]
    public async Task<ActionResult> Edit(string id)
    {
      if(! await IsSmallBusinessEntityExistAsync(id))
      {
        return NotFound();
      }

      var smallBusinessEntity = 
        _context.SmallBusinessEntities.FirstAsync(x => x.Id == id);

      return PartialView(smallBusinessEntity);
    }

    /// <summary>
    /// Edit <see cref="SmallBusinessEntity"/> object
    /// </summary>
    /// <param name="model"><see cref="SmallBusinessEntity"/> model containing the data to edit object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Edit(SmallBusinessEntity)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind] SmallBusinessEntity model)
    {     
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      
      var smallBusinessEntity = await _context.SmallBusinessEntities
          .AsNoTracking()
          .FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);
        
      if(smallBusinessEntity is not null)
      {
        ModelState.AddModelError("Name", "Название субъекта уже используется!");
        return BadRequest(ModelState);
      }

      _context.SmallBusinessEntities.Update(model);
      await _context.SaveChangesAsync();

      return RedirectToAction(nameof(Index));     
    }

    /// <summary>
    /// Return view to delete <see cref="SmallBusinessEntity"/> object
    /// </summary>
    /// <param name="id">ID <see cref="SmallBusinessEntity"/> object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(string)"/> method</returns>
    [HttpGet]
    public async Task<ActionResult> Delete(string id)
    {
      if(! await IsSmallBusinessEntityExistAsync(id))
      {
        return NotFound();
      }

      var smallBusinessEntity = 
        await _context.SmallBusinessEntities.FirstOrDefaultAsync(e => e.Id == id);

      return View(smallBusinessEntity);
    }

    /// <summary>
    /// Delete <see cref="SmallBusinessEntity"/> object
    /// </summary>
    /// <param name="model"><see cref="InspectingOrganization"/> model containing the data to delete object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(SmallBusinessEntity)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete([Bind] SmallBusinessEntity model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      _context.SmallBusinessEntities.Remove(model);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Check avalaible name or not
    /// </summary>
    /// <param name="name">Checking name</param>
    /// <returns>True if name is available, else false</returns>    
    private async Task<bool> IsSmallBusinessEntityNameAvailableAsync(string name) =>
      await _context.SmallBusinessEntities.AnyAsync(x => x.Name == name);

    /// <summary>
    /// Check <see cref="SmallBusinessEntity"/> object exist or not
    /// </summary>
    /// <param name="Id">ID <see cref="SmallBusinessEntity"/> object</param>
    /// <returns>True if business entity exists, else False</returns>
    private async Task<bool > IsSmallBusinessEntityExistAsync(string Id) =>
      await _context.SmallBusinessEntities.AnyAsync(x => x.Id == Id);
  }
}
