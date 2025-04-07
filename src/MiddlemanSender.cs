namespace sge.framework.middleman;

public class MiddlemanSender(IServiceProvider serviceProvider) : IMiddlemanSender
{
    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request)
    {
        var handlerType = typeof(IMiddlemanHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType) as IMiddlemanHandler<TRequest, TResponse>;
        if (handler == null)
            throw new InvalidOperationException($"No handler found for request of type {typeof(TRequest).Name}");

        return await handler.Handle(request);
    }
}