using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCore.Models
{
 // модель команды космического зонда  
    public class Command
    {
        public string Type { get; set; } = string.Empty; // тип команды
        public string Parameter { get; set; } = string.Empty; // параметр команды
        public DateTime Timestamp { get; set; } // время создания команды
        public Command(string type, string? parameter=null)
        {
            Type = type;
            Parameter = parameter;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return Parameter != null ? $"{Type} {Parameter}" : Type;
        }

    }
}
