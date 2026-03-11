using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandCore.Models;

namespace CommandCore.Interfaces
{
    // интерфейс для обработчика команды в цепочке ответственности
    public interface ICommandHandler
    {
        void SetNext(ICommandHandler next); // установка следующего обработчика в цепочке
        CommandContext Handle(CommandContext context); // обработка команды и передача дальше по цепочке
    }
}
