using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class ExecutionHandler : BaseCommandHandler
    {
        protected override void ProcessCommand(CommandContext context)
        {
            context.AddLog("[EXEC] Выполнение команды...");

            var cmd = context.ParsedCommand!;
            var state = context.ProbeState;
            var result = new StringBuilder();

            switch (cmd.Type)
            {
                case "SET_SPEED":
                    double speed = double.Parse(cmd.Parameter!);
                    state.CurrentSpeed = speed;
                    result.AppendLine($" Скорость установлена: {speed} м/с");
                    result.AppendLine($" Расход энергии: {speed * 0.01:F1}%");
                    state.BatteryLevel -= (int)(speed * 0.01);
                    break;

                case "CAPTURE_IMAGE":
                    result.AppendLine($" Снимок выполнен в режиме {cmd.Parameter}");
                    result.AppendLine($" Размер изображения: ~{GetImageSize(cmd.Parameter!)} МБ");
                    state.BatteryLevel -= 5;
                    break;

                case "ADJUST_TRAJECTORY":
                    double angle = double.Parse(cmd.Parameter!);
                    result.AppendLine($" Траектория скорректирована на {angle}°");
                    result.AppendLine($" Новый курс: {GetNewCourse(angle)}");
                    state.BatteryLevel -= 3;
                    break;

                case "DEPLOY_INSTRUMENT":
                    state.InstrumentsDeployed = true;
                    result.AppendLine($" Инструмент {cmd.Parameter} развёрнут");
                    result.AppendLine($" Все системы работают в штатном режиме");
                    state.BatteryLevel -= 10;
                    break;

                case "TRANSMIT_DATA":
                    result.AppendLine($" Данные переданы с приоритетом {cmd.Parameter}");
                    result.AppendLine($" Объём переданных данных: {GetDataSize(cmd.Parameter!)} МБ");
                    state.BatteryLevel -= GetTransmitCost(cmd.Parameter!);
                    break;

                case "ENTER_SAFE_MODE":
                    state.IsInSafeMode = true;
                    result.AppendLine($" Зонд переведён в безопасный режим");
                    result.AppendLine($" Все некритичные системы отключены");
                    break;

                case "EXIT_SAFE_MODE":
                    state.IsInSafeMode = false;
                    result.AppendLine($" Зонд выведен из безопасного режима");
                    result.AppendLine($" Все системы активированы");
                    state.BatteryLevel -= 5;
                    break;
            }

            if (state.BatteryLevel < 10 && !state.IsInSafeMode)
            {
                result.AppendLine("\n ВНИМАНИЕ: Критически низкий заряд батареи!");
                result.AppendLine("  Рекомендуется войти в безопасный режим");
            }

            context.ExecutionResult = result.ToString();
            context.AddLog($"[EXEC] Команда выполнена успешно");
            context.AddLog($"[EXEC] └─ Остаток заряда: {state.BatteryLevel}%");
        }
        private double GetImageSize(string mode)
        {
            return mode.ToUpper() switch
            {
                "WIDE" => 2.5,
                "ZOOM" => 4.2,
                "THERMAL" => 1.8,
                _ => 2.0
            };
        }

        private string GetNewCourse(double angle)
        {
            Random rnd = new Random();
            return $"{rnd.Next(0, 360)}° (корректировка: {angle:+0.0;-0.0}°)";

        }

        private int GetDataSize(string priority)
        {
            return priority.ToUpper() switch
            {
                "LOW" => 50,
                "NORMAL" => 150,
                "HIGH" => 300,
                _ => 100
            };
        }

        private int GetTransmitCost(string priority)
        {
            return priority.ToUpper() switch
            {
                "LOW" => 2,
                "NORMAL" => 5,
                "HIGH" => 10,
                _ => 5
            };
        }
    }
}