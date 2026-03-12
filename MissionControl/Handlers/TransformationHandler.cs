using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    public class TransformationHandler : BaseCommandHandler
    {
        protected override void ProcessCommand(CommandContext context)
        {
            context.AddLog("[TRANSFORM] Преобразование команды в байт-код...");

            var cmd = context.ParsedCommand!;

            var byteCode = new byte[3]; // имитация преобразования в байт-код

            // 1-й байт код команды
            byteCode[0] = cmd.Type switch
            {
                "SET_SPEED" => 0x10,
                "CAPTURE_IMAGE" => 0x20,
                "ADJUST_TRAJECTORY" => 0x30,
                "DEPLOY_INSTRUMENT" => 0x40,
                "TRANSMIT_DATA" => 0x50,
                "ENTER_SAFE_MODE" => 0x60,
                "EXIT_SAFE_MODE" => 0x61,
                _ => 0x00
            };

            // 2-й байт - режим, тип
            byteCode[1] = cmd.Parameter?.ToUpper() switch
            {
                "WIDE" => 0x01,
                "ZOOM" => 0x02,
                "THERMAL" => 0x03,
                "RADAR" => 0x01,
                "SPECTROMETER" => 0x02,
                "ANTENNA" => 0x03,
                "CAMERA" => 0x04,
                "LOW" => 0x01,
                "NORMAL" => 0x02,
                "HIGH" => 0x03,
                _ => 0x00
            };

            // 3-й байт - значение параметра 
            if (cmd.Parameter != null && double.TryParse(cmd.Parameter, out double value))
            {
                byteCode[2] = (byte)(value % 256); // упрощённое преобразование
            }

            context.TransformedData = byteCode;

            var hexString = BitConverter.ToString(byteCode).Replace("-", " ");
            context.AddLog($"[TRANSFORM] Преобразовано в байт-код: [{hexString}]");
        }
    }
}
