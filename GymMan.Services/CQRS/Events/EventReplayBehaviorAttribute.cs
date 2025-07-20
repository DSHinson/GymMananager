using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.CQRS.Events
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EventReplayBehaviorAttribute : Attribute
    {
        public EventReplayOptions Options { get; }

        public EventReplayBehaviorAttribute(EventReplayOptions options)
        {
            Options = options;
        }
    }

}
