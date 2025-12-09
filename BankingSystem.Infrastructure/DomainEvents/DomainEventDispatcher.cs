using BankingSystem.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace BankingSystem.Infrastructure.DomainEvents
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task Dispatch(IEnumerable<IDomainEvent> events)
        {
            foreach (var domainEvent in events)
            {
                var handlerType = typeof(IDomainEventHandler<>)
                    .MakeGenericType(domainEvent.GetType());

                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var method = handlerType.GetMethod("Handle");
                    await (Task)method.Invoke(handler,new object[] { domainEvent,CancellationToken.None});
                }
            } 
        }
    }
}
