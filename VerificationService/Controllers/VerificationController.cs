using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationApp.Data;
using VerificationApp.Models;
using VerificationApp.Services;
using VerificationApp.ViewModels;
using X.PagedList;

namespace VerificationApp.Controllers
{
  /// <summary>
  /// Controller for managing <see cref="Verification"/>
  /// </summary>
  public class VerificationController : Controller
  {
    private readonly ExcelService _excelService;
    private readonly VerificationAppContext _context;
    
    public VerificationController(VerificationAppContext context, ExcelService excelService)
    {
      _context = context;
      _excelService = excelService;
    }

    /// <summary>
    /// Get collection <see cref="Verification"/>
    /// </summary>
    /// <param name="filter">Model <see cref="FilterViewModel"/> for filtering results.</param>
    /// <returns>Returns View with collection <see cref="Verification"/> or HTTP status code indicating the result of the <see cref="Index"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] FilterViewModel filter)
    {
      if (filter.BegPeriod > filter.EndPeriods && filter.EndPeriods != default)
      {
        ModelState.AddModelError("begPeriod", "Указан неверный период проверки");
        return BadRequest(ModelState);
      }

      var verifications = await FilteringCollectionAsync(filter);

      ViewBag.Filter = filter;

      ViewBag.IndexViewUrl = GetIndexViewUrl();

      var pageSize = 10;

      if (filter.Page < 1)
      {
        filter.Page = 1;
      }

      var verifPage = verifications.ToPagedList(filter.Page, pageSize);

      return View(verifPage);
    }
    /// <summary>
    /// Export <see cref="Verification"/> collection  to Excel
    /// </summary>
    /// <param name="filter">Model <see cref="FilterViewModel"/> for filtering results</param>
    /// <returns>Excel file with collection <see cref="Verification"/> or HTTP status code indicating the result of the <see cref="ExportToExcel"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> ExportToExcel([FromQuery] FilterViewModel filter)
    {      
      var verifications = await FilteringCollectionAsync(filter);

      ViewBag.Filter = filter;

      ViewBag.IndexViewUrl = GetIndexViewUrl();

      var test = await _excelService.ExportToFileAsync(verifications);

      return File(test, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "verification.xlsx");
    }
    /// <summary>
    /// Import data from Excel
    /// </summary>
    /// <param name="model">Model <see cref="InspectingOrganization"/> for import data from Excel</param>
    /// <returns>Returns View with collection <see cref="Verification"/> or HTTP status code indicating the result of the <see cref="ImportFromExcel"/> method</returns>
    [HttpPost]
    public async Task<IActionResult> ImportFromExcel([Bind] ImportVerificationViewModel model)    
    {
      if(!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      if (model.FileInput != null && model.FileInput.Length > 0)
      {
        
        var allowedExtensions = new[] { ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(model.FileInput.FileName).ToLower();
        // проверяем формат переданного файла
        if (!allowedExtensions.Contains(fileExtension))
        {
          ModelState.AddModelError("FileInput", "Выберите файл с расширением .xlsx или .xls");
          return BadRequest(ModelState); 
        }

        var verifications = await _excelService.ImportFromFileAsync(model.FileInput);
        
        if (model.CheckDuplicate)
        {
          // удаление дубликатов
          verifications = await _excelService.RemoveDuplicatesAsync(verifications);
        }

        if (model.DeleteIncorrectValue)
        {
          // удаление некорректных данных (период и длительность проверки)
          verifications = _excelService.DeleteIncorrectValues(verifications);
        }

        await _context.AddRangeAsync(verifications);
        await _context.SaveChangesAsync();
      }

      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Display information about <see cref="Verification"/>.
    /// </summary>
    /// <param name="id">Id object <see cref="Verification"/></param>
    /// <param name="returnUrl">Url for return to Index view</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Details"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Details(string id, string returnUrl = null)
    {
      if (!await IsVerificationExistAsync(id))
      {
        return NotFound();
      }

      var verification = await _context.Verifications.FirstAsync(x => x.Id == id);

      var smallBusinessEntities = await _context.SmallBusinessEntities
        .Select(s => s.Name)
        .ToListAsync();

      var organizations = await _context.InspectingOrganizations
        .Select(s => s.Name)
        .ToListAsync();

      var verificationViewModel = new VerificationViewModel
      {
        Id = verification.Id,
        BusinessEntity = verification.BusinessEntity,
        Organization = verification.Organization,
        BeginPeriod = verification.BeginPeriod,
        EndPeriod = verification.EndPeriod,
        Duration = verification.Duration,
        BusinessEntitiesList = smallBusinessEntities!,
        OrganizationsList = organizations!,
      };

      // ссылка для перехода на прошлую страницу
      ViewBag.IndexViewUrl = returnUrl;

      return View(verificationViewModel);
    }

    /// <summary>
    /// Create <see cref="Verification"/> object
    /// </summary>
    /// <param name="model"><see cref="Verification"/> model containing the data to create object.</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Create"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind] VerificationViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (!await IsBusinessEntityExist(model.BusinessEntity!))
      {
        ModelState.AddModelError("BusinessEntity", "Введенный субъект не существует!");
        return BadRequest(ModelState);
      }

      if (!await IsOrganizationExist(model.Organization!))
      {
        ModelState.AddModelError("Organization", "Введенная организация не существует!");
        return BadRequest(ModelState);
      }

      if (model.BeginPeriod > model.EndPeriod)
      {
        ModelState.AddModelError("BeginPeriod", "Неверный период проведения проверки!");
        return BadRequest(ModelState);
      }

      var period = model.EndPeriod.DayNumber - model.BeginPeriod.DayNumber + 1;

      if (period < model.Duration)
      {
        ModelState.AddModelError("Duration", $"Длительность проверки не может быть больше {period}");
        return BadRequest(ModelState);
      }
      else if (model.Duration < 1)
      {
        ModelState.AddModelError("Duration", "Длительность проверки должна быть больше 0");
        return BadRequest(ModelState);
      }

      var verification = new Verification
      {
        Id = model.Id,
        BusinessEntity = model.BusinessEntity,
        Organization = model.Organization,
        BeginPeriod = model.BeginPeriod,
        EndPeriod = model.EndPeriod,
        Duration = model.Duration
      };

      await _context.Verifications.AddAsync(verification);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// Return partail view to edit <see cref="Verification"/> object
    /// </summary>
    /// <param name="id">ID <see cref="Verification"/> object</param>
    /// <param name="returnUrl">Url for return to Index view</param>
    /// <returns>Returns PartialView or HTTP status code indicating the result of the <see cref="Edit"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(string id, string returnUrl = null)
    {
      if (!await IsVerificationExistAsync(id))
      {
        return NotFound();
      }

      var verification =
        await _context.Verifications.FirstAsync(x => x.Id == id);

      var smallBusinessEntities = await _context.SmallBusinessEntities
          .Select(s => s.Name)
          .ToListAsync();

      var organizations = await _context.InspectingOrganizations
          .Select(s => s.Name)
          .ToListAsync();

      var verificationViewModel = new VerificationViewModel
      {
        Id = verification.Id,
        BusinessEntity = verification.BusinessEntity,
        Organization = verification.Organization,
        BeginPeriod = verification.BeginPeriod,
        EndPeriod = verification.EndPeriod,
        Duration = verification.Duration,
        BusinessEntitiesList = smallBusinessEntities!,
        OrganizationsList = organizations!,
      };
      // ссылка для перехода на прошлую страницу
      ViewBag.IndexViewUrl = returnUrl;

      return PartialView(verificationViewModel);
    }

    /// <summary>
    /// Edit <see cref="Verification"/> object
    /// </summary>
    /// <param name="model"><see cref="Verification"/> model containing the data to edit object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Edit(VerificationViewModel)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind] VerificationViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      // проверяем период проверки
      if (model.BeginPeriod > model.EndPeriod)
      {
        ModelState.AddModelError("BeginPeriod", "Неверный период проведения проверки!");
        return BadRequest(ModelState);
      }

      var period = model.EndPeriod.DayNumber - model.BeginPeriod.DayNumber + 1;
      // проверяем длительность проверки
      if (period < model.Duration)
      {
        ModelState.AddModelError("Duration", $"Длительность проверки не может быть больше {period}");
        return BadRequest(ModelState);
      }
      else if (model.Duration < 1)
      {
        ModelState.AddModelError("Duration", "Длительность проверки должна быть больше 0");
        return BadRequest(ModelState);
      }

      var verification = new Verification
      {
        Id = model.Id,
        BusinessEntity = model.BusinessEntity,
        Organization = model.Organization,
        BeginPeriod = model.BeginPeriod,
        EndPeriod = model.EndPeriod,
        Duration = model.Duration
      };
      // ссылка для перехода на прошлую страницу
      _context.Verifications.Update(verification);
      await _context.SaveChangesAsync();
      ViewBag.IndexViewUrl = Request.Form["returnUrl"];
      return Ok();
    }

    /// <summary>
    /// Return view to delete <see cref="Verification"/> object
    /// </summary>
    /// <param name="id">ID <see cref="Verification"/> object</param>
    /// <param name="returnUrl">Url for return to Index view</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(string, string)"/> method</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(string id, string returnUrl = null)
    {
      if (!await IsVerificationExistAsync(id))
      {
        return NotFound();
      }

      var verification =
        await _context.Verifications.FirstOrDefaultAsync(e => e.Id == id);
      ViewBag.IndexViewUrl = returnUrl;
      return View(verification);
    }

    /// <summary>
    /// Delete <see cref="Verification"/> object
    /// </summary>
    /// <param name="model"><see cref="Verification"/> model containing the data to delete object</param>
    /// <returns>Returns View or HTTP status code indicating the result of the <see cref="Delete(Verification)"/> method</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([Bind] Verification model)
    {
      _context.Verifications.Remove(model);
      await _context.SaveChangesAsync();
      ViewBag.IndexViewUrl = Request.Form["returnUrl"];
      return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Check <see cref="Verification"/> object exist or not
    /// </summary>
    /// <param name="Id">ID <see cref="Verification"/> object</param>
    /// <returns>True if <see cref="SmallBusinessEntity"/> object exists, else false</returns>
    private async Task<bool> IsVerificationExistAsync(string Id) =>
      await _context.Verifications.AnyAsync(x => x.Id == Id);

    /// <summary>
    /// Check <see cref="SmallBusinessEntity"/> object exist or not
    /// </summary>
    /// <param name="Id">ID <see cref="SmallBusinessEntity"/> object</param>
    /// <returns>True if <see cref="SmallBusinessEntity"/> object exists, else false</returns>
    private async Task<bool> IsBusinessEntityExist(string name) =>
      await _context.SmallBusinessEntities.AnyAsync(x => x.Name == name);

    /// <summary>
    /// Check <see cref="InspectingOrganization"/> object exist or not
    /// </summary>
    /// <param name="Id">ID <see cref="InspectingOrganization"/> object</param>
    /// <returns>True if <see cref="InspectingOrganization"/> object exists, else false</returns>
    private async Task<bool> IsOrganizationExist(string name) =>
      await _context.InspectingOrganizations.AnyAsync(x => x.Name == name);

    /// <summary>
    /// Get Index View URL
    /// </summary>
    /// <returns>Index view URL</returns>
    private string GetIndexViewUrl() =>
      $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

    /// <summary>
    /// Filter collection <see cref="Verification"/>
    /// </summary>
    /// <param name="verification"><see cref="FilterViewModel"/> model containing parametres to filter collection</param>
    /// <returns>Filtered collection <see cref="Verification"/></returns>
    private async Task<IEnumerable<Verification>> FilteringCollectionAsync(FilterViewModel verification)
    {
      var verifications =
        await _context.Verifications.ToListAsync();
      if (!String.IsNullOrEmpty(verification.Business))
      {
        verifications = verifications
          .Where(v => v.BusinessEntity!.Contains(verification.Business, StringComparison.OrdinalIgnoreCase))
          .ToList();
      }

      if (!String.IsNullOrEmpty(verification.Org))
      {
        verifications = verifications
           .Where(v => v.Organization!.Contains(verification.Org, StringComparison.OrdinalIgnoreCase))
           .ToList();
      }

      if (verification.BegPeriod != default)
      {
        verifications = verifications
            .Where(v => v.BeginPeriod >= verification.BegPeriod)
            .ToList();
      }

      if (verification.EndPeriods != default)
      {
        verifications = verifications
            .Where(v => v.EndPeriod <= verification.EndPeriods)
            .ToList();
      }

      return verifications;
    }
  }
}
