namespace Infotecs_intern_tz.Dtos
{
    public class ResultsFilterDto
    {
        public string? FileName { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public double? AverageValueFrom { get; set; }
        public double? AverageValueTo { get; set; }
        public double? AverageExecutionTimeFrom { get; set; }
        public double? AverageExecutionTimeTo { get; set; }
    }

}
