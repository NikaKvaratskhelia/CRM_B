using System.Threading;
using System.Threading.Tasks;
using CRM_B.Api.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM_B.Api.Presentation.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
        {
            var result = await mediator.Send(req.ToCommand(), ct);
            return Ok(result);
        }
    }
}