using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCore.Models
{
 // контекст выполнения команды по цепочке обработчиков
    public class CommandContext
    {
        public string RawCommand { get; set; } = string.Empty; // исходная команда в текстовом виде
        public Command? ParsedCommand { get; set; }
        public byte[]? TransformedData { get; set; } // преобразованные данные
        public List<string> Logs { get; set; } // журнал обработки, логи с каждого этапа
        public bool IsStoped { get; set; }
        public string? ErrorMessage { get; set; }
        public ProbeState ProbeState { get; set; }
        public string? ExecutionResult { get; set; }
        public CommandContext(string rawCommand, ProbeState probeState)
        {
            RawCommand = rawCommand;
            ProbeState = probeState;
            Logs = new List<string>();
            IsStoped = false;
        }

        public void Stop(string errorMessage)
        {
            IsStoped = true;
            ErrorMessage = errorMessage;
        }

        public void AddLog(string message)
        {
            Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

    }
}
