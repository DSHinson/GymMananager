using GymMan.Services.CQRS.Commands.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.CQRS.Commands.Handlers
{
    public class loginEventHandler : ICommandHandler<loginEvent>
    {
        public Task HandleAsync(loginEvent command)
        {
            throw new NotImplementedException();
        }
    }
}
