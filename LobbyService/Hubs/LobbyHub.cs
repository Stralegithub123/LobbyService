using Microsoft.AspNetCore.SignalR;
using LobbyService;
using LobbyService.Model;
using LobbyService.Data.Enumerations;

namespace LobbyService.Hubs;

public class LobbyHub : Hub
{
    private readonly ILobbyService _lobbyService;
    private readonly ILobbyActionService _actionService;

    public LobbyHub(ILobbyService lobbyService, ILobbyActionService actionService)
    {
        _lobbyService = lobbyService;
        _actionService = actionService;
    }

    public async Task JoinLobbyGroup(string accessCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, accessCode);

        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        
        if (lobby != null)
        {
            await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", lobby);
        }
    }

    public async Task LeaveLobbyGroup(string accessCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, accessCode);

        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        
        if (lobby != null)
        {
            await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", lobby);
        }
    }

    public async Task ChangeColor(string accessCode, int userId, int colorCode)
    {
        try
        {
            var updatedLobby = await _actionService.ChangeColorAsync(accessCode, userId, (Color)colorCode);
            await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", updatedLobby);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task ToggleReady(string accessCode, int userId)
    {
        var updatedLobby = await _actionService.ToggleReadyAsync(accessCode, userId);
        await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", updatedLobby);
    }

    public async Task RollDice(string accessCode, int userId, int rolledNumber)
    {
        var updatedLobby = await _actionService.RollDiceAsync(accessCode, userId, rolledNumber);
        await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", updatedLobby);
    }
}