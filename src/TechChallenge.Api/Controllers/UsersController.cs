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
    [Authorize("Admin")]
    [Route("{id:Guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
        => Send(await service.Find(id));
    
    [HttpGet]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
        => Send(await service.Find());
    
    [HttpPost]
    [Authorize("Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        => await Send(service.CreateAsync(request));
}