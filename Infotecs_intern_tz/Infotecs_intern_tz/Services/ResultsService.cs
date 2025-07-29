using Infotecs_intern_tz.Dtos;
using Infotecs_intern_tz.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs_intern_tz.Services
{
    public class ResultsService
    {
        private readonly DatabaseContext _databaseContext;  
        public ResultsService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public IActionResult GetFilteredResults(ResultsFilterDto filters)
        {
            if (filters == null) return new JsonResult("Тело метода пустое");
            if (filters.FileName != null)
            {
                var result = _databaseContext.results.FirstOrDefault(t => t.fileName == filters.FileName);
                return result != null ? new JsonResult(result) : new JsonResult("Файл с таким именем не найден");
            }
            if (filters.StartDateFrom != null && filters.StartDateTo != null)
            {
                if (filters.StartDateFrom > filters.StartDateTo) return new JsonResult("Неверно указан диапазон времени запуска первой операции");
                var results = _databaseContext.results.Where(t => t.result.minDate > filters.StartDateFrom && t.result.minDate < filters.StartDateTo).ToList();
                return results.Count != 0 ? new JsonResult(results) : new JsonResult("Файлы с таким диапазоном времени запуска первой операции не найдены");
            }
            if (filters.AverageValueFrom != null && filters.AverageValueTo != null)
            {
                if (filters.AverageValueFrom > filters.AverageValueTo) return new JsonResult("Неверно указан диапазон среднего показателя");
                var results = _databaseContext.results.Where(t => t.result.averageValue > filters.AverageValueFrom && t.result.averageValue < filters.AverageValueTo).ToList();
                return results != null ? new JsonResult(results) : new JsonResult("Файлы с таким диапазоном среднего показателя не найдены");
            }
            if (filters.AverageExecutionTimeFrom != null && filters.AverageExecutionTimeTo != null)
            {
                if (filters.AverageExecutionTimeFrom > filters.AverageExecutionTimeTo) return new JsonResult("Неверно указан диапазон времени выполнения");
                var results = _databaseContext.results.Where(t => t.result.averageExecutionTime > filters.AverageExecutionTimeFrom && t.result.averageExecutionTime < filters.AverageExecutionTimeTo).ToList();
                return results != null ? new JsonResult(results) : new JsonResult("Файлы с таким диапазоном времени выполнения не найдены");
            }
            return new JsonResult("Тело метода не соответствует формату");
        }

        public IActionResult GetLastTenValuesByFileName(string fileName)
        {
            var values = _databaseContext.values.First(t => t.fileName == fileName);
            if (values == null) return new JsonResult("Файл с таким именем не найден");
            return new JsonResult(values.value.OrderByDescending(r => r.date).Take(10).ToList());
        }
    }
}