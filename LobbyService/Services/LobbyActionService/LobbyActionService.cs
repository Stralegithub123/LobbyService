using LobbyService.Model;
using LobbyService.Data.Enumerations;

namespace LobbyService;

public class LobbyActionManager : ILobbyActionService
{
    private readonly ILobbyService _lobbyService;

    public LobbyActionManager(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    public async Task<Lobby> ChangeColorAsync(string accessCode, int userId, Color newColor)
    {
        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        if (lobby == null) 
            throw new Exception("Lobi ne postoji.");

        await lobby.Semaphore.WaitAsync();
        try
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
            if (player == null) 
                throw new Exception("Igrač nije u lobiju.");

            if (lobby.Players.Any(p => p.Color == newColor && p.UserId != userId))
                throw new Exception("Boja je već zauzeta.");

            player.Color = newColor;
            return lobby;
        }
        finally
        {
            lobby.Semaphore.Release();
        }
    }

    public async Task<Lobby> ToggleReadyAsync(string accessCode, int userId)
    {
        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        if (lobby == null) throw new Exception("Lobi ne postoji.");

        await lobby.Semaphore.WaitAsync();
        try
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
            if (player != null)
            {
                player.IsReady = !player.IsReady;
            }
            return lobby;
        }
        finally
        {
            lobby.Semaphore.Release();
        }
    }

    public async Task<Lobby> RollDiceAsync(string accessCode, int userId, int rolledNumber)
    {
        var lobby = await _lobbyService.GetLobbyAsync(accessCode);
        if (lobby == null) throw new Exception("Lobi ne postoji.");

        await lobby.Semaphore.WaitAsync();
        try
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
            if (player != null)
            {
                player.RolledNumber = rolledNumber;
            }
            return lobby;
        }
        finally
        {
            lobby.Semaphore.Release();
        }
    }
}