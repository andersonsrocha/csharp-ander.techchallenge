using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Dto;
using TechChallenge.Domain.Interfaces.Services;
using TechChallenge.Domain.Requests.Games;

namespace TechChallenge.Api.Controllers;

[Route("api/[controller]")]
public class GamesController(IGameService service) : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("{id:Guid}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] Guid id)
        => Send(service.Find(id));
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
        => Send(service.Find());
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateGameRequest request)
        => await Send(service.CreateAsync(request));
    
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] UpdateGameRequest request)
        => await Send(service.UpdateAsync(request));
    
    [HttpDelete]
    [Authorize]
    [Route("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
        => await Send(service.DeleteAsync(id));
}