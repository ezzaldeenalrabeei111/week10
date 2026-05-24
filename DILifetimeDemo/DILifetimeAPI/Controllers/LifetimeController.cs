using DILifetimeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DILifetimeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LifetimeController : ControllerBase
{
    private readonly ITransientService _transient1;
    private readonly ITransientService _transient2;
    private readonly IScopedService _scoped1;
    private readonly IScopedService _scoped2;
    private readonly ISingletonService _singleton1;
    private readonly ISingletonService _singleton2;

    public LifetimeController(
        ITransientService transient1, ITransientService transient2,
        IScopedService scoped1, IScopedService scoped2,
        ISingletonService singleton1, ISingletonService singleton2)
    {
        _transient1 = transient1;
        _transient2 = transient2;
        _scoped1 = scoped1;
        _scoped2 = scoped2;
        _singleton1 = singleton1;
        _singleton2 = singleton2;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Transient = new { Service1 = _transient1.GetOperationId(), Service2 = _transient2.GetOperationId(), Note = "Different IDs every time" },
            Scoped = new { Service1 = _scoped1.GetOperationId(), Service2 = _scoped2.GetOperationId(), Note = "Same IDs within one request, different across requests" },
            Singleton = new { Service1 = _singleton1.GetOperationId(), Service2 = _singleton2.GetOperationId(), Note = "Same ID for the entire application lifetime" }
        });
    }
}
