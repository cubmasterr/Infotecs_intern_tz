using Infotecs_intern_tz.Migrations;
using Infotecs_intern_tz.Models;
using Infotecs_intern_tz.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs_intern_tz.Services
{
    public class DataImportService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DateTime _lowestPossibleDate = new DateTime(2000, 1, 1);
        public DataImportService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public IActionResult ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)return new JsonResult("Файл пустой");
            var valueEntry = new ValueEntry();
            var resultEntry = new ResultEntry();
            ParseCsv(file, valueEntry, resultEntry);
            return SaveChangesToDb(resultEntry, valueEntry);
        }
        private IActionResult ParseCsv(IFormFile file, ValueEntry valueEntry, ResultEntry resultEntry)
        {
            List<float> values = new();
            List<ValueSchema> valueSchemas = new List<ValueSchema>();
            int lineCount = 0;
            var maxDate = _lowestPossibleDate;
            resultEntry.result.minDate = DateTime.Now;
            double averageExecutionTime = 0;
            using var stream = file.OpenReadStream();
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;

                    if (lineCount > 10000)
                    {
                        return new JsonResult("Количество строк не может быть меньше 1 и больше 10 000");
                    }
                    if (lineCount == 1) continue;

                    var parseResult = ParseLine(line, lineCount, ref maxDate, resultEntry, valueSchemas, values,ref averageExecutionTime);
                    if (parseResult != null) return parseResult;
                }
            }

            ProcessResults(values, maxDate, resultEntry, file, valueEntry, valueSchemas,averageExecutionTime);
            return new OkResult();
        }

        private IActionResult? ParseLine(string line, int lineCount, ref DateTime maxDate,
            ResultEntry resultEntry, List<ValueSchema> valueSchemas, List<float> values,ref double averageExecutionTime)
        {
            try
            {
                var schema = ParseCsvLine(line);
                if (schema == null)
                { 
                    return new JsonResult("Значения должны соответствовать своим типам, отсутствие одного из значений в записи недопустимо"); 
                }
                var validationResult = ValidateSchema(schema);
                if (validationResult != null) return validationResult;

                UpdateResultsWithSchema(schema, lineCount, ref maxDate, resultEntry, valueSchemas, values,ref averageExecutionTime);
            }
            catch 
            {
                return new JsonResult("Значения должны соответствовать своим типам, отсутствие одного из значений в записи недопустимо");
            }

            return null;
        }

        private ValueSchema? ParseCsvLine(string line)
        {
            var data = line.Split(';');
            if (data.Length != 3)
            {
                return null;
            }
            DateTimeOffset dto = DateTimeOffset.Parse(data[0]);
            return new ValueSchema(
                dto.UtcDateTime,
                Convert.ToInt32(data[1]),
                Convert.ToSingle(data[2].Replace('.', ',')));
        }

        private IActionResult? ValidateSchema(ValueSchema schema)
        {
            if (schema.date > DateTime.Now || schema.date < _lowestPossibleDate)
                return new JsonResult("Дата не может быть позже текущей и раньше 01.01.2000");
            if (schema.executionTime < 0)
                return new JsonResult("Время выполнения не может быть меньше 0");
            if (schema.value < 0)
                return new JsonResult("Значение показателя не может быть меньше 0");

            return null;
        }

        private void UpdateResultsWithSchema(ValueSchema schema, int lineCount, ref DateTime maxDate,
            ResultEntry resultEntry, List<ValueSchema> valueSchemas, List<float> values,ref double averageExecutionTime)
        {
            valueSchemas.Add(schema);

            if (schema.date > maxDate) maxDate = schema.date;
            if (schema.date < resultEntry.result.minDate)
            {
                resultEntry.result.minDate = schema.date;
            }
            averageExecutionTime += (schema.executionTime - averageExecutionTime) / (lineCount - 1);
            resultEntry.result.averageValue += (schema.value - resultEntry.result.averageValue) / (lineCount - 1);

            values.Add(schema.value);
        }

        private void ProcessResults(List<float> values, DateTime maxDate, ResultEntry resultEntry,
            IFormFile file, ValueEntry valueEntry, List<ValueSchema> valueSchemas,double averageExecutionTime)
        {
            values.Sort();
            resultEntry.result.averageExecutionTime =(long) averageExecutionTime;
            resultEntry.result.maxValue = values[values.Count - 1];
            resultEntry.result.minValue = values[0];
            resultEntry.result.medianValue = values.Count % 2 == 1
                ? values[values.Count / 2]
                : (values[values.Count / 2 - 1] + values[values.Count / 2]) / 2.0f;
            resultEntry.result.minDate = DateTime.SpecifyKind(resultEntry.result.minDate, DateTimeKind.Utc);
            TimeSpan delta = maxDate - resultEntry.result.minDate;
            resultEntry.result.deltaTime = (long)delta.TotalSeconds;
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            resultEntry.fileName = fileName;
            valueEntry.fileName = fileName;
            valueEntry.value = valueSchemas;
        }
        private IActionResult SaveChangesToDb(ResultEntry resultEntry, ValueEntry valueEntry)
        {
            using var transition = _databaseContext.Database.BeginTransaction();
            {
                try
                {
                    var resultDb = _databaseContext.results.Where(content => content.fileName == resultEntry.fileName).FirstOrDefault();
                    if (resultDb == null)
                    {
                        _databaseContext.results.Add(resultEntry);
                    }
                    else
                    {
                        resultDb.result = resultEntry.result;// mau be need to made it by lines
                    }
                    var valueDb = _databaseContext.values.Where(content => content.fileName == valueEntry.fileName).FirstOrDefault();
                    if (valueDb == null)
                    {
                        _databaseContext.values.Add(valueEntry);
                    }
                    else
                    {
                        valueDb.value = valueEntry.value;// mau be need to made it by lines
                    }
                    _databaseContext.SaveChanges();
                    transition.Commit();
                }
                catch (Exception ex)
                {
                    transition.Rollback();
                    return new JsonResult("Значения должны соответствовать своим типам, отсутствие одного из значений в записи недопустимо");
                }
            }
            return new OkResult();
        }
    }
}
