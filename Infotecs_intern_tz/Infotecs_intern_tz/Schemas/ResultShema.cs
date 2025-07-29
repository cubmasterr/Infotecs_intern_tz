namespace Infotecs_intern_tz.Schemas
{
    public class Resultschema
    {
        public long deltaTime { get; set; }
        public DateTime minDate { get; set; }
        public long averageExecutionTime { get; set; }
        public float averageValue { get; set; }
        public float medianValue { get; set; }
        public float maxValue { get; set; }
        public float minValue { get; set; }
    }
}
