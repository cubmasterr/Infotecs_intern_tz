using Infotecs_intern_tz.Dtos;
using Infotecs_intern_tz.Models;
using Infotecs_intern_tz.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Infotecs_intern_tz.Controllers
{
    [Route("results")]
    public class ResultsController : Controller
    {
        private readonly ResultsService _resultsService;
        public ResultsController(ResultsService resultsService)
        {
            _resultsService = resultsService;
        }
        /// <summary>
        ///   список записей из таблицы Results, подходящих под фильтры
         /// </summary>
               [HttpGet]
        [SwaggerResponse(200, "", typeof(List<ResultEntry>))]
        public IActionResult GetFilteredResults([FromQuery] ResultsFilterDto filters)
        {
           return _resultsService.GetFilteredResults(filters);
        }
        /// <summary>
        ///   Последниие 10 записей из Values по имени файла
        /// </summary>
        [HttpGet("files/{fileName}/last-ten")]
        [SwaggerResponse(200, "", typeof(List <ValueEntry>))]
        public IActionResult GetLastTenValuesByFileName(string fileName)
        {
            return _resultsService.GetLastTenValuesByFileName(fileName);//not complited need tests
        }
    }
}
