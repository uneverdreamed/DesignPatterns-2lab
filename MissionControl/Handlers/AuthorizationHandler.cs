using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class AuthorizationHandler : BaseCommandHandler
    {
        protected override void ProcessCommand(CommandContext context)
        {
            context.AddLog("[AUTH] Проверка полномочий...");

            var cmd = context.ParsedCommand!;
            var state = context.ProbeState;

            if (cmd.Type == "CAPTURE_IMAGE" && state.BatteryLevel < 20)
            {
                context.Stop("[AUTH] Ошибка: Недостаточный заряд батареи для съёмки (требуется ≥20%)");
                return;
            }

            if (cmd.Type == "SET_SPEED" && state.IsInSafeMode)
            {
                context.Stop("[AUTH] Ошибка: Изменение скорости запрещено в безопасном режиме");
                return;
            }

            if (cmd.Type == "ADJUST_TRAJECTORY" && state.IsInSafeMode)
            {
                context.Stop("[AUTH] Ошибка: Корректировка траектории запрещена в безопасном режиме");
                return;
            }

            if (cmd.Type == "DEPLOY_INSTRUMENT" && state.InstrumentsDeployed)
            {
                context.Stop("[AUTH] Ошибка: Инструменты уже развёрнуты");
                return;
            }

            if (cmd.Type == "DEPLOY_INSTRUMENT" && state.BatteryLevel < 30)
            {
                context.Stop("[AUTH] Ошибка: Недостаточно энергии для развёртывания инструментов (требуется ≥30%)");
                return;
            }

            if (cmd.Type == "EXIT_SAFE_MODE" && state.BatteryLevel < 50)
            {
                context.Stop("[AUTH] Ошибка: Недостаточно энергии для выхода из безопасного режима (требуется ≥50%)");
                return;
            }

            if (cmd.Type == "CAPTURE_IMAGE" && !state.InstrumentsDeployed)
            {
                context.Stop("[AUTH] Ошибка: Камера не развёрнута. Сначала выполните DEPLOY_INSTRUMENT");
                return;
            }
            context.AddLog("[AUTH] Команда разрешена");
        }
    }
}
