using LobbyService.Model;
using System.Text.Json.Serialization;
using System.Threading;

namespace LobbyService;

public interface ILobbyService
{
    Task<Lobby> CreateLobbyAsync(int hostUserId, string hostUsername);
    Task<Lobby?> GetLobbyAsync(string accessCode);
    Task<LobbyPlayer> AddPlayerAsync(string accessCode, int userId, string username);
    Task RemovePlayerAsync(string accessCode, int userId);
    Task<IEnumerable<Lobby>> GetAllLobbiesAsync();
    Task<bool> DeleteLobbyAsync(string accessCode);
    Task<Lobby?> GetLobbyByIdAsync(int id);
    Task<LobbyPlayer?> GetPlayerByIdAsync(int playerId);
}