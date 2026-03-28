using Microsoft.AspNetCore.Mvc;
using LobbyService.Model;
using LobbyService;
using LobbyService.Data.Records;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class LobbyPlayerController : ControllerBase
{
    private readonly ILobbyService _lobbyService;

    public LobbyPlayerController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    [HttpPost("Join")]
    public async Task<ActionResult<LobbyPlayer>> Join([FromBody] JoinRequest request)
    {
        try
        {
            var player = await _lobbyService.AddPlayerAsync(
                request.AccessCode, 
                request.UserId, 
                request.Username ?? "Nepoznat igrač"
            );
            
            return Ok(player);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Lobi sa tim kodom ne postoji.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Leave/{accessCode}/{userId}")]
    public async Task<ActionResult> Leave(string accessCode, int userId)
    {
        try
        {
            await _lobbyService.RemovePlayerAsync(accessCode, userId);
            return Ok("Igrač je napustio lobi.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<LobbyPlayer>> GetById(int id)
    {
        var player = await _lobbyService.GetPlayerByIdAsync(id);
        
        if (player == null)
            return NotFound($"Igrač sa ID {id} nije pronađen u sistemu.");

        return Ok(player);
    }
}