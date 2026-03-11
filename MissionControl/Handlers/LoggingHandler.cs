using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class LoggingHandler : BaseCommandHandler
    {
        protected override void ProcessCommand(CommandContext context)
        {
            var cmd = context.ParsedCommand!;
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                             $"Команда: {cmd} | " +
                             $"Батарея: {context.ProbeState.BatteryLevel}% | " +
                             $"Режим: {(context.ProbeState.IsInSafeMode ? "SAFE" : "NORMAL")}";

            context.AddLog($"[LOG] Записано в журнал миссии");
            context.AddLog($"[LOG]   └─ {logEntry}");
        }
    }
}
