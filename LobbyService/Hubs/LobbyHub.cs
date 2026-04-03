using Microsoft.AspNetCore.SignalR;
using LobbyService;
using LobbyService.Model;
using LobbyService.Data.Enumerations;
using LobbyService.Hubs.ConnectionMapping;

namespace LobbyService.Hubs;

public class LobbyHub : Hub
{
    private readonly ILobbyService _lobbyService;
    private readonly ILobbyActionService _actionService;
    private readonly ConnectionMappingService _mapping;

    public LobbyHub(ILobbyService lobbyService, 
        ILobbyActionService actionService, 
        ConnectionMappingService mapping)
    {
        _lobbyService = lobbyService;
        _actionService = actionService;
        _mapping = mapping;
    }

    public async Task JoinLobbyGroup(string accessCode, int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, accessCode);
        _mapping.Add(Context.ConnectionId, accessCode, userId);

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

    public async Task StartGame(string accessCode, int userId)
    {
        try
        {
            var success = await _actionService.StartGameAsync(accessCode, userId);
            if (success)
            {
                await Clients.Group(accessCode).SendAsync("GameStarted");
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_mapping.TryGet(Context.ConnectionId, out var accessCode, out var userId))
        {
            await _lobbyService.RemovePlayerAsync(accessCode, userId);
            var lobby = await _lobbyService.GetLobbyAsync(accessCode);
            if (lobby != null)
            {
                await Clients.Group(accessCode).SendAsync("LobbyStateUpdated", lobby);
            }
            _mapping.Remove(Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}