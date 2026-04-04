using LobbyService.Model;
using LobbyService.Services.LobbyCode;
using System.Collections.Concurrent;
using LobbyService.Data.Enumerations;

namespace LobbyService;

public class LobbyManager : ILobbyService
{
    private readonly LobbyCodeService _codeService;
    
    private int _lobbyIdCounter = 0;
    private int _playerIdCounter = 0;

    private readonly ConcurrentDictionary<string, Lobby> _lobbies = new();

    public LobbyManager(LobbyCodeService codeService)
    {
        _codeService = codeService;
    }

    public Task<Lobby> CreateLobbyAsync(int hostUserId, string hostUsername)
    {
        int newId = Interlocked.Increment(ref _lobbyIdCounter);
        string accessCode = _codeService.EncodeLobbyId(newId);

        var newLobby = new Lobby
        {
            Id = newId,
            HostUserId = hostUserId,
            MaxPlayCout = 50,
            AccessCode = accessCode,
            Players = new List<LobbyPlayer>()
        };

        var hostPlayer = new LobbyPlayer
        {
            Id = Interlocked.Increment(ref _playerIdCounter),
            UserId = hostUserId,
            Username = hostUsername,
            LobbyId = newId,
            Color = Color.Red, 
            IsReady = false,
            RolledNumber = 0
        };

        newLobby.Players.Add(hostPlayer);

        _lobbies.TryAdd(accessCode, newLobby);
        return Task.FromResult(newLobby);
    }

    public Task<Lobby?> GetLobbyAsync(string accessCode)
    {
        _lobbies.TryGetValue(accessCode, out var lobby);
        return Task.FromResult(lobby);
    }

    public Task<Lobby?> GetLobbyByIdAsync(int lobbyId)
    {
        var accessCode = _codeService.EncodeLobbyId(lobbyId);
        _lobbies.TryGetValue(accessCode, out var lobby);
        return Task.FromResult(lobby);
    }

    public Task<IEnumerable<Lobby>> GetAllLobbiesAsync()
    {
        return Task.FromResult(_lobbies.Values.AsEnumerable());
    }

    public Task<bool> DeleteLobbyAsync(string accessCode)
    {
        return Task.FromResult(_lobbies.TryRemove(accessCode, out _));
    }

    public async Task<LobbyPlayer> AddPlayerAsync(string accessCode, int userId, string username)
    {
        if (!_lobbies.TryGetValue(accessCode, out var lobby))
            throw new KeyNotFoundException("Lobi nije pronađen.");

        await lobby.Semaphore.WaitAsync();
        try
        {
            if (lobby.Players.Count >= 4)
                throw new InvalidOperationException("Lobi je pun.");

            if (lobby.Players.Any(p => p.UserId == userId))
                throw new InvalidOperationException("Igrač je već u lobiju.");

            var takenColors = lobby.Players.Select(p => p.Color).ToList();
            var assignedColor = GetFirstAvailableColor(takenColors);

            var newPlayer = new LobbyPlayer
            {
                Id = Interlocked.Increment(ref _playerIdCounter),
                UserId = userId,
                Username = username,
                LobbyId = lobby.Id,
                Color = assignedColor,
                IsReady = false,
                RolledNumber = 0
            };

            lobby.Players.Add(newPlayer);
            return newPlayer;
        }
        finally
        {
            lobby.Semaphore.Release();
        }
    }

    public async Task RemovePlayerAsync(string accessCode, int userId)
    {
        if (!_lobbies.TryGetValue(accessCode, out var lobby)) 
            return;

        await lobby.Semaphore.WaitAsync();
        try
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == userId);
            if (player == null) 
                return;

            lobby.Players.Remove(player);

            if (lobby.Players.Count == 0)
            {
                _lobbies.TryRemove(accessCode, out _);
            }
            else if (lobby.HostUserId == userId)
            {
                lobby.HostUserId = lobby.Players.First().UserId;
            }   
        }
        finally
        {
            lobby.Semaphore.Release();
        }
    }

    public Task<LobbyPlayer?> GetPlayerByIdAsync(int playerId)
    {
        var player = _lobbies.Values
            .SelectMany(l => l.Players)
            .FirstOrDefault(p => p.Id == playerId);
            
        return Task.FromResult(player);
    }

    private Color GetFirstAvailableColor(List<Color> takenColors)
    {
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            if (!takenColors.Contains(color)) return color;
        }
        return Color.White;
    }
}