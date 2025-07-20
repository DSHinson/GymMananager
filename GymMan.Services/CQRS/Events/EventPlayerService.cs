using GymMan.Services.CQRS.Commands;
using GymMan.Services.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.CQRS.Events
{
    public class EventPlayerService
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly List<IEvent> _eventLog = new();

        public EventPlayerService(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        }

        public async Task<TResult> EmitAsync<TResult>(IQuery<TResult> query)
        {
            _eventLog.Add(query);
            var method = typeof(ICommandDispatcher)
                    .GetMethod(nameof(ICommandDispatcher.DispatchAsync))!
                    .MakeGenericMethod(query.GetType());


            return await _queryDispatcher.DispatchAsync<IQuery<TResult>, TResult>(query);
        }

        public async Task EmitAsync(ICommand command)
        {
            _eventLog.Add(command);
            var method = typeof(ICommandDispatcher)
         .GetMethod(nameof(ICommandDispatcher.DispatchAsync))!
         .MakeGenericMethod(command.GetType());

            await (Task)method.Invoke(_commandDispatcher, [command])!;
        }

        public IEnumerable<IEvent> GetEventLog() => _eventLog.AsReadOnly();

        public async Task ReplayAsync()
        {
            foreach (var evt in _eventLog)
            {
                var attr = evt.GetType()
                    .GetCustomAttributes(typeof(EventReplayBehaviorAttribute), false)
                    .FirstOrDefault() as EventReplayBehaviorAttribute;

                if (attr is null || !attr.Options.HasFlag(EventReplayOptions.Replayable))
                    continue;

                if (evt is ICommand command)
                {
                    var method = typeof(ICommandDispatcher)
                        .GetMethod(nameof(ICommandDispatcher.DispatchAsync))!
                        .MakeGenericMethod(command.GetType());

                    await (Task)method.Invoke(_commandDispatcher, [command])!;
                }
            }
        }

    }
}
