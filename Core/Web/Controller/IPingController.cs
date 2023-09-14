using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core.Security;

namespace ModularSystem.Web;

public interface IPingController
{
    [HttpGet("ping")]
    Task<IActionResult> Ping([BindNever] IResourcePolicy? resourcePolicy = null);
}
