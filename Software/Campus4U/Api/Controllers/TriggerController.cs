using Api.Workers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

//Luka Kanjir
[ApiController]
public sealed class TriggerController(ITriggerControl control) : ControllerBase
{
    [HttpPost(ApiEndpoints.Triggers.Start)]
    public IActionResult Start()
    {
        control.Start();
        return Ok(new { enabled = control.Enabled });
    }

    [HttpPost(ApiEndpoints.Triggers.Heartbeat)]
    public IActionResult Heartbeat()
    {
        control.Heartbeat();
        return Ok(new { enabled = control.Enabled });
    }

    [HttpPost("kick")]
    public IActionResult Kick()
    {
        control.Kick();
        return Ok(new { enabled = control.Enabled });
    }

    [HttpGet(ApiEndpoints.Triggers.Status)]
    public IActionResult Status() => Ok(new { enabled = control.Enabled });
}