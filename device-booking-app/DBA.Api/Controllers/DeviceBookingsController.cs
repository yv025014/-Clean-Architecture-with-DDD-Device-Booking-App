using DBA.Application.DeviceBookings.Commands;
using DBA.Application.DeviceBookings.Queries;
using DBA.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DBA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class DeviceBookingsController : ControllerBase
{
    private readonly ISender _sender;

    public DeviceBookingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DeviceBookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDeviceBookingsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DeviceBookingDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] GetDeviceBookingByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query with { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDeviceBookingCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] ApproveDeviceBookingCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectDeviceBookingCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/return")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Return([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ReturnDeviceBookingCommand(id), cancellationToken);
        return Ok(result);
    }
}
