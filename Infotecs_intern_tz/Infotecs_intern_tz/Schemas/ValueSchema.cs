namespace Infotecs_intern_tz.Schemas
{
    public class ValueSchema
    {
        public ValueSchema(DateTime date, long executionTime, float value)
        {
            this.date = date; DateTime.SpecifyKind(date, DateTimeKind.Utc);
            this.executionTime = executionTime;  
            this.value = value;
        }
        public DateTime date { get; set; }
        public long executionTime { get; set; }
        public float value { get; set; }
    }
}
