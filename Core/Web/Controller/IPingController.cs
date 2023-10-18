using Microsoft.AspNetCore.Mvc;

namespace ModularSystem.Web;

public interface IPingController
{
    [HttpGet("ping")]
    Task<IActionResult> Ping();
}
