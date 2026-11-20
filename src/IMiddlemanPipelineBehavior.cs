namespace sge.framework.middleman;

/// <summary>Invokes the next behavior in the pipeline, or the handler if there are none left.</summary>
public delegate Task<TResponse> MiddlemanHandlerDelegate<TResponse>();

public interface IMiddlemanPipelineBehavior<in TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        MiddlemanHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
