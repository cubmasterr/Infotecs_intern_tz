using Infotecs_intern_tz.Schemas;
using System.ComponentModel.DataAnnotations;

namespace Infotecs_intern_tz.Models
{
    public class ResultEntry
    {
        public ResultEntry()
        {
            result = new Resultschema();
        }
        [Key]
        public string fileName { get; set; }
        public Resultschema result { get; set; }
    }
}
