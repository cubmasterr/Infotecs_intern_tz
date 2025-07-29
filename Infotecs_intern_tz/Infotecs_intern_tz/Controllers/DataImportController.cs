using Microsoft.AspNetCore.Mvc;
using Infotecs_intern_tz.Services;

namespace Infotecs_intern_tz.Controllers
{   
    [Route("dataImport")]
    public class DataImportController : Controller
    {
        private readonly DataImportService _dataImportService;
        public DataImportController( DataImportService dataImportService)
        {
            _dataImportService = dataImportService;
        }
        /// <summary>
        ///  Импорт Csv файла
        /// </summary>
        [HttpPost("importCsv")]
        [Consumes("multipart/form-data")]
        public IActionResult ImportCsv(IFormFile file)
        {
            return _dataImportService.ImportCsv(file);
        }
    }
}
