using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.CQRS.Queries
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _provider;

        public QueryDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
            var handler = _provider.GetService(handlerType);

            if (handler is null)
                throw new InvalidOperationException($"Handler for {typeof(TQuery).Name} not found");

            var method = handlerType.GetMethod("HandleAsync");
            if (method is null)
                throw new InvalidOperationException($"HandleAsync method not found for {handlerType.Name}");

            var resultTask = (Task<TResult>)method.Invoke(handler, new object[] { query })!;
            return await resultTask;
        }
    }
}
