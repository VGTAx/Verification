using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using VerificationApp.Data;
using VerificationApp.Models;

namespace VerificationApp.Services
{
  /// <summary>
  /// Service for working with Excel files
  /// </summary>
  public class ExcelService
  {
    private readonly VerificationAppContext _context;

    public ExcelService(VerificationAppContext context)
    {
      _context = context;
    }

    /// <summary>
    /// Import data from Excel file
    /// </summary>
    /// <param name="excelFile">Excel file</param>
    /// <returns>Collection of data</returns>
    public async Task<IEnumerable<Verification>> ImportFromFileAsync(IFormFile excelFile)
    {
      var verifications = new List<Verification>();

      using (var stream = new MemoryStream())
      {
        await excelFile.CopyToAsync(stream);
        using (var package = new ExcelPackage(stream))
        {
          // находим первый лист в книге Excel
          var ws = package.Workbook.Worksheets.First();
          // находим первый таблицу на листе
          var table = ws.Tables.FirstOrDefault();

          if (table is null)
          {
            return verifications;
          }
          // считываем значения из таблицы
          for (int row = table.Address.Start.Row + 1; row <= table.Address.End.Row; row++)
          {
            var verification = new Verification { Id = Guid.NewGuid().ToString() };
            for (int col = table.Address.Start.Column; col <= table.Address.End.Column; col++)
            {
              switch (col)
              {
                case 1:
                  verification.BusinessEntity = ws.Cells[row, col].Text;
                  break;
                case 2:
                  verification.Organization = ws.Cells[row, col].Text;
                  break;
                case 3:
                  verification.BeginPeriod = ParseToDateOnly(ws.Cells[row, col].Text);
                  break;
                case 4:
                  verification.EndPeriod = ParseToDateOnly(ws.Cells[row, col].Text);
                  break;
                case 5:
                  verification.Duration = int.TryParse(ws.Cells[row, col].Text, out int result) ? result : 0;
                  break;
                default:
                  break;
              }
            }
            verifications.Add(verification);
          }
        }
      }
      return verifications;
    }

    /// <summary>
    /// Export data to Excel file
    /// </summary>
    /// <param name="verifications">Collection of data for export.</param>
    /// <returns>A byte array representing the contents of the Excel file.</returns>
    public async Task<byte[]> ExportToFileAsync(IEnumerable<Verification> verifications)
    {
      using (var package = new ExcelPackage())
      {
        // создаем лист в книге Excel
        var ws = package.Workbook.Worksheets.Add("Реестр проверок");
        // устанавливаем заголовки
        ws.Cells[1, 1].Value = "Субъект малого предпринимательства";
        ws.Cells[1, 2].Value = "Проверяющая организация";
        ws.Cells[1, 3].Value = "Начало периода";
        ws.Cells[1, 4].Value = "Конец периода";
        ws.Cells[1, 5].Value = "Длительность проверки";

        int row = 2;
        //добавляем значения в столбцы
        foreach (var verification in verifications)
        {
          ws.Cells[$"A{row}"].Value = verification.BusinessEntity;
          ws.Cells[$"B{row}"].Value = verification.Organization;
          ws.Cells[$"C{row}"].Value = verification.BeginPeriod;
          ws.Cells[$"D{row}"].Value = verification.EndPeriod;
          ws.Cells[$"E{row}"].Value = verification.Duration;
          row++;
        }

        ws.Cells.AutoFitColumns();

        var tableRange = ws.Cells[1, 1, row, 5];
        // создаем таблицу
        ws.Tables.Add(tableRange, "Table");
        return await package.GetAsByteArrayAsync();
      }
    }

    /// <summary>
    /// Removes duplicates from a data collection.
    /// </summary>
    /// <param name="importVerification">Data collection</param>
    /// <returns>Collection of data after removing duplicates</returns>
    public async Task<IEnumerable<Verification>> RemoveDuplicatesAsync(IEnumerable<Verification> importVerification)
    {
      var verifications = new List<Verification>();
      // удаляем дубликаты в исходном файле
      importVerification = importVerification
              .GroupBy(x => new { x.BusinessEntity, x.Organization, x.BeginPeriod, x.EndPeriod, x.Duration })
              .Select(x => x.First())
              .ToList();
      // удаляем дубликаты, которые уже добавлены в БД
      foreach (var verification in importVerification)
      {
        if (!await IsDuplicateAsync(verification))
        {
          verifications.Add(verification);
        }
      }
      return verifications;
    }

    /// <summary>
    /// Removes records with incorrect values from the data collection.
    /// </summary>
    /// <param name="importVerification">Data collection</param>
    /// <returns>Collection of data after removing records with incorrect values</returns>
    public IEnumerable<Verification> DeleteIncorrectValues(IEnumerable<Verification> importVerification)
    {
      var verifications = new List<Verification>();

      foreach (var verification in importVerification)
      {
        var period = verification.EndPeriod.DayNumber - verification.BeginPeriod.DayNumber + 1;
        // если не указан начальный период или длительность равна = 0, то элемент не добавляется
        if (verification.BeginPeriod == default || verification.Duration == 0)
        {
          continue;
        }
        // если не указан начальный период больше конечного или длительность проверки больше периода проверки, то элемент не добавляется
        if (verification.BeginPeriod > verification.EndPeriod || period < verification.Duration)
        {
          continue;
        }
        verifications.Add(verification);
      }
      return verifications;
    }

    /// <summary>
    /// Converts a string representation of a date  to a <see cref="DateOnly"/> object.
    /// </summary>
    /// <param name="dateTime">String representation of date</param>
    /// <returns>Object <see cref="DateOnly"/></returns>
    private static DateOnly ParseToDateOnly(string dateTime)
    {
      if (DateTime.TryParse(dateTime, out DateTime dateTimeValue))
      {
        return new DateOnly(dateTimeValue.Year, dateTimeValue.Month, dateTimeValue.Day);
      }
      else
      {
        return new DateOnly();
      }
    }

    /// <summary>
    /// Checks whether the passed value is a duplicate in the collection.
    /// </summary>
    /// <param name="verification"><see cref="Verification"/> object to check for duplicates.</param>
    /// <returns>Returns true if the value is a duplicate, false otherwise.</returns>
    private async Task<bool> IsDuplicateAsync(Verification verification)
    {
      return await _context.Verifications
        .AnyAsync(x =>
          x.BusinessEntity!.ToLower() == verification.BusinessEntity!.ToLower() &&
          x.Organization!.ToLower() == verification.Organization!.ToLower() &&
          x.BeginPeriod == verification.BeginPeriod &&
          x.EndPeriod == verification.EndPeriod &&
          x.Duration == verification.Duration);
    }
  }
}
