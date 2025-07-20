using GymMan.Services.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.CQRS.Commands.Events
{
    [EventReplayBehaviorAttribute(EventReplayOptions.Replayable)]
    public class loginEvent : ICommand
    {
    }
}
