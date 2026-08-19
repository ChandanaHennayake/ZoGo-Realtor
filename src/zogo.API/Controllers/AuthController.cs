using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Authentication;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [ProducesResponseType(
        typeof(RegisterResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authenticationService.RegisterAsync(
                request,
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
     [FromBody] LoginRequest request,
     CancellationToken cancellationToken)
    {
        var response =
            await _authenticationService.LoginAsync(
                request,
                cancellationToken);

        return Ok(response);
    }


    [HttpPost("google")]
    public async Task<ActionResult<AuthenticationResponse>> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await _authenticationService.GoogleLoginAsync(
                request,
                cancellationToken);

        return Ok(response);
    }
}