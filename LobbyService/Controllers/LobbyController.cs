using Microsoft.AspNetCore.Mvc;
using LobbyService.Model;
using LobbyService;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class LobbyController : ControllerBase
{
    private readonly ILobbyService _lobbyService;

    public LobbyController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    [HttpPost("CreateLobby")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Lobby>> CreateLobby([FromBody] CreateLobbyRequest request)
    {
        try
        {
            var lobby = await _lobbyService.CreateLobbyAsync(request.HostUserId, request.HostUsername);
            return Ok(lobby);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Lobby>> GetById(int id)
    {
        var lobby = await _lobbyService.GetLobbyByIdAsync(id);
        if (lobby == null)
            return NotFound($"Lobi sa ID {id} nije pronađen.");

        return Ok(lobby);
    }

    [HttpGet("GetByAccessCode/{accessCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Lobby>> GetByAccessCode(string accessCode)
    {
        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        if (lobby == null)
            return NotFound($"Lobi sa kodom {accessCode} nije pronađen.");

        return Ok(lobby);
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Lobby>>> GetAll()
    {
        var lobbies = await _lobbyService.GetAllLobbiesAsync();
        return Ok(lobbies);
    }

    [HttpDelete("Delete/{accessCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string accessCode)
    {
        bool deleted = await _lobbyService.DeleteLobbyAsync(accessCode);
        if (!deleted)
            return NotFound($"Lobi sa kodom {accessCode} nije pronađen.");

        return Ok($"Lobi sa kodom {accessCode} je uspešno obrisan.");
    }
}