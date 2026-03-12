using CommandCore.Models;
using MissionControl.Handlers;
using MissionControl.Pipeline;
using System;

namespace MissionControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА ОБРАБОТКИ КОМАНД КОСМИЧЕСКОГО ЗОНДА ===");
            var probeState = new ProbeState(batteryLevel: 80, safeMode: false); // создание состояния
            
            var pipeline = new CommandPipeline()
                .AddHandler(new SyntaxValidationHandler())
                .AddHandler(new AuthorizationHandler())
                .AddHandler(new ParameterValidationHandler())
                .AddHandler(new TransformationHandler())
                .AddHandler(new LoggingHandler())
                .AddHandler(new EncryptionHandler())
                .AddHandler(new ExecutionHandler());

            pipeline.PrintPipelineInfo();

            bool running = true;
            while (running)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunUserCommand(pipeline, probeState);
                        break;
                    case "2":
                        ShowProbeState(probeState);
                        break;
                    case "3":
                        ShowAvailableCommands();
                        break;
                    case "4":
                        ResetProbeState(probeState);
                        break;
                    case "5":
                        RunAutomatedDemo(pipeline, probeState);
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Завершение работы системы");
                        break;
                    default:
                        Console.WriteLine("\n[ОШИБКА] Неверный выбор. Попробуйте снова.\n");
                        break;
                }
                if (running && choice != "5")
                {
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }
        static void ShowMenu()
        {
            Console.WriteLine("                      ГЛАВНОЕ МЕНЮ");;
            Console.WriteLine("1. Ввести команду вручную");
            Console.WriteLine("2. Показать состояние зонда");
            Console.WriteLine("3. Показать доступные команды");
            Console.WriteLine("4. Сбросить состояние зонда");
            Console.WriteLine("5. Запустить автоматическую демонстрацию");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }
        static void RunUserCommand(CommandPipeline pipeline, ProbeState probeState)
        {
            Console.WriteLine("                  ВВОД ПОЛЬЗОВАТЕЛЬСКОЙ КОМАНДЫ");
            Console.WriteLine("\nПримеры команд:");
            Console.WriteLine("  SET_SPEED 500");
            Console.WriteLine("  CAPTURE_IMAGE WIDE");
            Console.WriteLine("  DEPLOY_INSTRUMENT CAMERA");
            Console.WriteLine("  ENTER_SAFE_MODE");
            Console.WriteLine("\nВведите команду (или 'help' для списка команд):");
            Console.Write("> ");

            var command = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine("\n[ОШИБКА] Команда не может быть пустой");
                return;
            }
            if (command.ToLower() == "help")
            {
                ShowAvailableCommands();
                return;
            }

            Console.WriteLine();
            pipeline.Execute(command, probeState);
        }

        static void ShowProbeState(ProbeState probeState)
        {
            Console.WriteLine("                  ТЕКУЩЕЕ СОСТОЯНИЕ ЗОНДА");
            Console.WriteLine($"Заряд батареи:       {probeState.BatteryLevel}%");

            if (probeState.BatteryLevel < 20)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (probeState.BatteryLevel < 50)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"                     [{GetBatteryBar(probeState.BatteryLevel)}]");
            Console.ResetColor();

            Console.WriteLine($"Режим:               {(probeState.IsInSafeMode ? "БЕЗОПАСНЫЙ" : "НОРМАЛЬНЫЙ")}");
            Console.WriteLine($"Текущая скорость:    {probeState.CurrentSpeed} м/с");
            Console.WriteLine($"Инструменты:         {(probeState.InstrumentsDeployed ? "РАЗВЁРНУТЫ" : "СЛОЖЕНЫ")}");
        }
        static string GetBatteryBar(int level)
        {
            int bars = level / 10;
            return new string('█', bars) + new string('░', 10 - bars);
        }
        static void ShowAvailableCommands()
        {
            Console.WriteLine("                   ДОСТУПНЫЕ КОМАНДЫ");
            Console.WriteLine("\n1. SET_SPEED <значение>");
            Console.WriteLine("   Установить скорость зонда (0-1000 м/с)");
            Console.WriteLine("   Пример: SET_SPEED 500");
            Console.WriteLine();
            Console.WriteLine("2. CAPTURE_IMAGE <режим>");
            Console.WriteLine("   Сделать снимок (WIDE/ZOOM/THERMAL)");
            Console.WriteLine("   Пример: CAPTURE_IMAGE WIDE");
            Console.WriteLine();
            Console.WriteLine("3. ADJUST_TRAJECTORY <угол>");
            Console.WriteLine("   Корректировка траектории (-180 до 180 градусов)");
            Console.WriteLine("   Пример: ADJUST_TRAJECTORY 15.5");
            Console.WriteLine();
            Console.WriteLine("4. DEPLOY_INSTRUMENT <название>");
            Console.WriteLine("   Развернуть инструмент (RADAR/SPECTROMETER/ANTENNA/CAMERA)");
            Console.WriteLine("   Пример: DEPLOY_INSTRUMENT CAMERA");
            Console.WriteLine();
            Console.WriteLine("5. TRANSMIT_DATA <приоритет>");
            Console.WriteLine("   Передать данные на Землю (LOW/NORMAL/HIGH)");
            Console.WriteLine("   Пример: TRANSMIT_DATA HIGH");
            Console.WriteLine();
            Console.WriteLine("6. ENTER_SAFE_MODE");
            Console.WriteLine("   Перейти в безопасный режим");
            Console.WriteLine();
            Console.WriteLine("7. EXIT_SAFE_MODE");
            Console.WriteLine("   Выйти из безопасного режима");
        }
        static void ResetProbeState(ProbeState probeState)
        {
            Console.WriteLine("                  СБРОС СОСТОЯНИЯ ЗОНДА");
            probeState.BatteryLevel = 100;
            probeState.IsInSafeMode = false;
            probeState.CurrentSpeed = 0;
            probeState.InstrumentsDeployed = false;

            Console.WriteLine("\n✓ Состояние зонда сброшено к начальным значениям:");
            Console.WriteLine("  • Заряд батареи: 100%");
            Console.WriteLine("  • Режим: НОРМАЛЬНЫЙ");
            Console.WriteLine("  • Скорость: 0 м/с");
            Console.WriteLine("  • Инструменты: СЛОЖЕНЫ\n");
        }

        static void RunAutomatedDemo(CommandPipeline pipeline, ProbeState probeState)
        {
            Console.Clear();
            Console.WriteLine("            АВТОМАТИЧЕСКАЯ ДЕМОНСТРАЦИЯ                    ");
            
            Console.WriteLine("Сбросить состояние зонда перед демонстрацией? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                ResetProbeState(probeState);
                probeState.BatteryLevel = 80; 
            }

            Console.WriteLine("\nНажмите Enter для начала...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine(" СЦЕНАРИЙ 1: Успешная обработка команды");
            
            pipeline.Execute("SET_SPEED 500", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 2: Ошибка валидации параметров                  ");
            
            pipeline.Execute("SET_SPEED 9999", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 3: Ошибка авторизации (низкий заряд)           ");

            probeState.BatteryLevel = 15; // Снижаем заряд
            pipeline.Execute("CAPTURE_IMAGE WIDE", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 4: Развёртывание инструментов                   ");

            probeState.BatteryLevel = 60;
            pipeline.Execute("DEPLOY_INSTRUMENT CAMERA", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 5: Съёмка после развёртывания камеры            ");
            pipeline.Execute("CAPTURE_IMAGE THERMAL", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 6: Переход в безопасный режим                   ");
            pipeline.Execute("ENTER_SAFE_MODE", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 7: Блокировка команды в безопасном режиме       ");
            pipeline.Execute("SET_SPEED 300", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 8: Ошибка синтаксиса команды                    ");
            pipeline.Execute("UNKNOWN_COMMAND 123", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 9: Корректировка траектории                     ");
            probeState.IsInSafeMode = false;
            pipeline.Execute("ADJUST_TRAJECTORY 15.5", probeState);
            Console.WriteLine("\nНажмите Enter для следующего сценария...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("  СЦЕНАРИЙ 10: Передача данных на Землю                    ");
            pipeline.Execute("TRANSMIT_DATA HIGH", probeState);

            Console.WriteLine("              ИТОГОВОЕ СОСТОЯНИЕ ЗОНДА");
            Console.WriteLine($"Заряд батареи:       {probeState.BatteryLevel}%");
            Console.WriteLine($"Режим:               {(probeState.IsInSafeMode ? "БЕЗОПАСНЫЙ" : "НОРМАЛЬНЫЙ")}");
            Console.WriteLine($"Текущая скорость:    {probeState.CurrentSpeed} м/с");
            Console.WriteLine($"Инструменты:         {(probeState.InstrumentsDeployed ? "РАЗВЁРНУТЫ" : "СЛОЖЕНЫ")}");
            Console.WriteLine("               ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА                      ");
            Console.WriteLine("\nНажмите Enter для возврата в меню...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}



