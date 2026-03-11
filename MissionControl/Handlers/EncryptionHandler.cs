using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Base;
using CommandCore.Models;

namespace MissionControl.Handlers
{
    class EncryptionHandler
    {
		public class EncryptionHandler : BaseCommandHandler
		{
			private const byte EncryptionKey = 0x5A; 

			protected override void ProcessCommand(CommandContext context)
			{
				context.AddLog("[ENCRYPT] Шифрование данных...");

				if (context.TransformedData == null)
				{
					context.AddLog("[ENCRYPT] Нет данных для шифрования, пропуск");
					return;
				}
				var encrypted = new byte[context.TransformedData.Length];
				for (int i = 0; i < context.TransformedData.Length; i++)
				{
					encrypted[i] = (byte)(context.TransformedData[i] ^ EncryptionKey);
				}

				context.TransformedData = encrypted;

				var hexString = BitConverter.ToString(encrypted).Replace("-", " ");
				context.AddLog($"[ENCRYPT] Данные зашифрованы: [{hexString}]");
			}
}
