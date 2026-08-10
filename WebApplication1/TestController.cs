using Microsoft.AspNetCore.Mvc;
using sge.framework.middleman;

namespace WebApplication1;

[ApiController]
[Route("[controller]")]
public class TestController(IMiddlemanSender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var command = new ServiceCommand();
        
        var send = await sender.Send(command);
        
        return Ok();
    }
}