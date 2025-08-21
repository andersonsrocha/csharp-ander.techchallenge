using Microsoft.AspNetCore.Mvc;
using TechChallenge.Application.Auth.Commands;
using TechChallenge.Application.Auth.Interfaces;

namespace TechChallenge.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController(IAuthService service) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        => await Send(service.Login(request));
}