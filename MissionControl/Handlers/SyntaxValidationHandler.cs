using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class SyntaxValidationHandler : BaseCommandHandler
    {
        private static readonly string[] ValidCommands = new[]
        {
            "SET_SPEED",
            "CAPTURE_IMAGE",
            "ADJUST_TRAJECTORY",
            "DEPLOY_INSTRUMENT",
            "TRANSMIT_DATA",
            "ENTER_SAFE_MODE",
            "EXIT_SAFE_MODE"
        };
        protected override void ProcessCommand(CommandContext context)
        {
            context.AddLog("[SYNTAX] Проверка синтаксиса команды...");

            var parts = context.RawCommand.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                context.Stop("[SYNTAX] Ошибка: Пустая команда");
                return;
            }

            string commandType = parts[0].ToUpper();

            if (!ValidCommands.Contains(commandType))
            {
                context.Stop($"[SYNTAX] Ошибка: Неизвестная команда '{commandType}'");
                return;
            }

            bool requiresParameter = commandType switch
            {
                "SET_SPEED" => true,
                "CAPTURE_IMAGE" => true,
                "ADJUST_TRAJECTORY" => true,
                "DEPLOY_INSTRUMENT" => true,
                "TRANSMIT_DATA" => true,
                _ => false
            };

            if (requiresParameter && parts.Length < 2)
            {
                context.Stop($"[SYNTAX] Ошибка: Команда '{commandType}' требует параметр");
                return;
            }

            context.ParsedCommand = new Command(
                commandType,
                parts.Length > 1 ? parts[1] : null
            );

            context.AddLog($"[SYNTAX] Синтаксис корректен: {context.ParsedCommand}");
        }
    }
}
