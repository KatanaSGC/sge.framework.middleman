using sge.framework.middleman;

namespace WebApplication1;

public class ServiceHandler : IMiddlemanHandler<ServiceCommand, bool>
{
    public async Task<bool> Handle(ServiceCommand request)
    {
        throw new NotImplementedException();
    }
}