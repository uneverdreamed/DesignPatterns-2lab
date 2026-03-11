using CommandCore.Interfaces;
using CommandCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionControl.Pipeline
{
    // управляет цепочкой обработчиков и выполнением команд
    public class CommandPipeline
    {
        private readonly List<ICommandHandler> _handlers;
        private ICommandHandler? _firstHandler;

        public CommandPipeline()
        {
            _handlers = new List<ICommandHandler>();
        }

        // добавление обработчика в конец цепочки
        public CommandPipeline AddHandler(ICommandHandler handler)
        {
            if (_handlers.Count > 0)
            {
                _handlers[_handlers.Count - 1].SetNext(handler);
            }
            else
            {
                _firstHandler = handler;
            }
            _handlers.Add(handler);
            return this;
        }

        // выполнение обработки команды через цепочку
        public CommandContext Execute(string rawCommand, ProbeState probeState)
        {
            if (_firstHandler == null)
            {
                throw new InvalidOperationException("Пусто. Добавьте хотя бы один обработчик.");
            }

            // создание контекста выполнения
            var context = new CommandContext(rawCommand, probeState);
            Console.WriteLine($"=== ОБРАБОТКА КОМАНДЫ: {rawCommand} ===");

            var result = _firstHandler.Handle(context); // запуск цепочки 

            PrintResult(result);
            return result;
        }
        public void Clear() // очистка цепочки 
        {
            _handlers.Clear();
            _firstHandler = null;
        }

        public int GetHandlerCount()
        {
            return _handlers.Count;
        }

        // вывод информации о CommandPipeline
        public void PrintPipelineInfo()
        {
            Console.WriteLine("=== КОНФИГУРАЦИЯ КОНВЕЙЕРА ===");
            Console.WriteLine($"Количество обработчиков: {_handlers.Count}");
            for (int i = 0; i < _handlers.Count; i++)
            {
                var handlerName = _handlers[i].GetType().Name.Replace("Handler", "");
                Console.WriteLine($"║  {i + 1}. {handlerName} ║");
            }
        }
        // вывод результата обработки
        private void PrintResult(CommandContext context)
        {
            Console.WriteLine("== ЖУРНАЛ ОБРАБОТКИ: ==");
            foreach (var log in context.Logs)
            {
                Console.WriteLine(log);
            }

            if (context.IsStoped)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n КОМАНДА ОТКЛОНЕНА: {context.ErrorMessage}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n КОМАНДА УСПЕШНО ОБРАБОТАНА");
                Console.ResetColor();

                if (!string.IsNullOrEmpty(context.ExecutionResult))
                {
                    Console.WriteLine($"\nРезультат выполнения:\n{context.ExecutionResult}");
                }
            }
            Console.WriteLine();
        }

    }
}
