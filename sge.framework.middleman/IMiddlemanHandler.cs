namespace sge.framework.middleman;

public interface IMiddlemanHandler<TRequest, TResponse> 
{
    Task<TResponse> Handle(TRequest request);
}