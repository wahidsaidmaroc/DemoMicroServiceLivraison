using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceNotification.Application.Commands;
using ServiceNotification.Application.Queries;

namespace ServiceNotification.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] SendNotificationCommand command, CancellationToken cancellationToken)
    {
        var notificationId = await sender.Send(command, cancellationToken);

        return AcceptedAtAction(
            nameof(GetById),
            new { id = notificationId },
            new { id = notificationId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetNotificationQuery(id), cancellationToken);

        return result is not null ? Ok(result) : NotFound();
    }
}
