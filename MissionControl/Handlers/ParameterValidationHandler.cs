using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class ParameterValidationHandler : BaseCommandHandler
    {
        protected override void ProcessCommand(CommandContext context)
        {
            context.AddLog("[PARAM] Валидация параметров...");

            var cmd = context.ParsedCommand!;

            switch (cmd.Type)
            {
                case "SET_SPEED":
                    ValidateSpeed(context, cmd.Parameter!);
                    break;

                case "CAPTURE_IMAGE":
                    ValidateImageMode(context, cmd.Parameter!);
                    break;

                case "ADJUST_TRAJECTORY":
                    ValidateTrajectory(context, cmd.Parameter!);
                    break;

                case "DEPLOY_INSTRUMENT":
                    ValidateInstrument(context, cmd.Parameter!);
                    break;

                case "TRANSMIT_DATA":
                    ValidatePriority(context, cmd.Parameter!);
                    break;

                default:
                    context.AddLog("[PARAM] Параметры не требуются");
                    break;
            }
        }
        private void ValidateSpeed(CommandContext context, string parameter)
        {
            if (!double.TryParse(parameter, out double speed))
            {
                context.Stop($"[PARAM] Ошибка: '{parameter}' не является числом");
                return;
            }

            if (speed < 0 || speed > 1000)
            {
                context.Stop($"[PARAM] Ошибка: Скорость {speed} вне диапазона (0-1000 м/с)");
                return;
            }

            context.AddLog($"[PARAM] Скорость валидна: {speed} м/с");
        }

        private void ValidateImageMode(CommandContext context, string parameter)
        {
            var validModes = new[] { "WIDE", "ZOOM", "THERMAL" };
            var mode = parameter.ToUpper();

            if (!Array.Exists(validModes, m => m == mode))
            {
                context.Stop($"[PARAM] Ошибка: Неизвестный режим съёмки '{parameter}' (допустимые: WIDE, ZOOM, THERMAL)");
                return;
            }
            context.AddLog($"[PARAM] Режим съёмки валиден: {mode}");
        }

        private void ValidateTrajectory(CommandContext context, string parameter)
        {
            if (!double.TryParse(parameter, out double angle))
            {
                context.Stop($"[PARAM] Ошибка: '{parameter}' не является числом");
                return;
            }

            if (angle < -180 || angle > 180)
            {
                context.Stop($"[PARAM] Ошибка: Угол {angle}° вне диапазона (-180° до 180°)");
                return;
            }

            context.AddLog($"[PARAM] Угол траектории валиден: {angle}°");
        }
        private void ValidateInstrument(CommandContext context, string parameter)
        {
            var validInstruments = new[] { "RADAR", "SPECTROMETER", "ANTENNA", "CAMERA" };
            var instrument = parameter.ToUpper();

            if (!Array.Exists(validInstruments, i => i == instrument))
            {
                context.Stop($"[PARAM] Ошибка: Неизвестный инструмент '{parameter}' (допустимые: RADAR, SPECTROMETER, ANTENNA, CAMERA)");
                return;
            }
            context.AddLog($"[PARAM] Инструмент валиден: {instrument}");
        }

        private void ValidatePriority(CommandContext context, string parameter)
        {
            var validPriorities = new[] { "LOW", "NORMAL", "HIGH" };
            var priority = parameter.ToUpper();

            if (!Array.Exists(validPriorities, p => p == priority))
            {
                context.Stop($"[PARAM] Ошибка: Неизвестный приоритет '{parameter}' (допустимые: LOW, NORMAL, HIGH)");
                return;
            }

            context.AddLog($"[PARAM] Приоритет валиден: {priority}");
        }

    }
}