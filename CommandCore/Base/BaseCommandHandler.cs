using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Interfaces;
using CommandCore.Models;

namespace CommandCore.Base
{
    // базовый класс для обработчиков команд, реализующий цепочку ответственности
    public abstract class BaseCommandHandler : ICommandHandler
    {
        private ICommandHandler? _next;
        // установить следующий обработчик в цепочке
        public void SetNext(ICommandHandler next)
        {
            _next = next;
        }
        public virtual CommandContext Handle(CommandContext context)
        {
            // не обрабатываем, если команда прервана
            if (context.IsStoped)
            {
                return context;
            }

            ProcessCommand(context);

            if (!context.IsStoped && _next != null)
            {
                return _next.Handle(context);
            }

            return context;
        }

        // метод обработки команды, который должен быть реализован в конкретных обработчиках
        protected abstract void ProcessCommand(CommandContext context); 
    }
}
