using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace sge.framework.middleman;

public class MiddlemanSender(IServiceProvider serviceProvider) : IMiddlemanSender
{
    private static readonly ConcurrentDictionary<(Type Request, Type Response), object> Executors = new();

    public Task<TResponse> Send<TResponse>(
        IMiddlemanRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var executor = (RequestExecutor<TResponse>)Executors.GetOrAdd(
            (requestType, typeof(TResponse)),
            static key => Activator.CreateInstance(
                typeof(RequestExecutor<,>).MakeGenericType(key.Request, key.Response))!);

        return executor.Execute(request, serviceProvider, cancellationToken);
    }

    private abstract class RequestExecutor<TResponse>
    {
        public abstract Task<TResponse> Execute(
            object request,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }

    private sealed class RequestExecutor<TRequest, TResponse> : RequestExecutor<TResponse>
    {
        public override Task<TResponse> Execute(
            object request,
            IServiceProvider provider,
            CancellationToken cancellationToken)
        {
            var handler = provider.GetService<IMiddlemanHandler<TRequest, TResponse>>()
                          ?? throw new InvalidOperationException(
                              $"No handler found for request of type {typeof(TRequest).Name}");

            var typedRequest = (TRequest)request;
            MiddlemanHandlerDelegate<TResponse> next = () => handler.Handle(typedRequest, cancellationToken);

            // The first registered behavior is the outermost one.
            var behaviors = provider.GetServices<IMiddlemanPipelineBehavior<TRequest, TResponse>>().ToArray();
            for (var i = behaviors.Length - 1; i >= 0; i--)
            {
                var behavior = behaviors[i];
                var inner = next;
                next = () => behavior.Handle(typedRequest, inner, cancellationToken);
            }

            return next();
        }
    }
}