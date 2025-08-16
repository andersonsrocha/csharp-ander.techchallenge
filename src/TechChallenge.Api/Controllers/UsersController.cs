using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces.Services;
using TechChallenge.Domain.Requests.Users;

namespace TechChallenge.Api.Controllers;

[Route("api/[controller]")]
public class UsersController(IUserService service) : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("{id:Guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(Guid id)
        => Send(service.Find(id));
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get()
        => Send(service.Find());
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        => await Send(service.CreateAsync(request));
}