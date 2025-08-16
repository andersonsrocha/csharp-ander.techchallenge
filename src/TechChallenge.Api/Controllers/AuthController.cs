using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Interfaces.Services;
using TechChallenge.Domain.Requests.Auth;

namespace TechChallenge.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController(IAuthService service) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        => await Send(service.Login(request));
}