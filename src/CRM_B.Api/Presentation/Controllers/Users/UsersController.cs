using Asp.Versioning;
using CRM_B.Api.Contracts.Users;
using CRM_B.Api.Extensions.Auth;
using CRM_B.Application.Features.Users.DeleteProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_B.Api.Presentation.Controllers.Users;

[Authorize, ApiController, ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPut("me")]
    public async Task<IActionResult> EditProfile(EditProfileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(this.GetUserId()), ct);
        return Ok(result);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(this.GetUserId()), ct);
        return Ok(result);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProfileCommand(this.GetUserId()), ct);
        return Ok(result);
    }
}