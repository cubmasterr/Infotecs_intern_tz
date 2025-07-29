using Infotecs_intern_tz.Schemas;
using System.ComponentModel.DataAnnotations;

namespace Infotecs_intern_tz.Models
{
    public class ValueEntry
    {
        public ValueEntry()
        {
            value = new();
        }
        [Key]
        public string fileName { get; set; }
        public List<ValueSchema>  value { get; set; }
    }
}
