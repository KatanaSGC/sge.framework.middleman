namespace sge.framework.middleman;

public class MiddlemanSender(IServiceProvider serviceProvider) : IMiddlemanSender
{
    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request)
    {
        var requestType = request!.GetType();
        
        var handlerType = typeof(IMiddlemanHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType) as dynamic;
        if (handler == null)
            throw new InvalidOperationException($"No handler found for request of type {typeof(TRequest).Name}");

        return await handler.Handle(request);
    }

    public async Task<TResponse> Send<TResponse>(IMiddlemanRequest<TResponse> request)
    {
        var requestType = request!.GetType();
        
        var handlerType = typeof(IMiddlemanHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType) as dynamic;
        if (handler == null)
            throw new InvalidOperationException($"No handler found for request of type {requestType.Name}");

        return await ((dynamic)handler).Handle((dynamic)request);
    }
}