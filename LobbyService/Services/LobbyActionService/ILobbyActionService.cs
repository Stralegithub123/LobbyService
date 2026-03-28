using LobbyService.Model;
using LobbyService.Data.Enumerations;

namespace LobbyService;

public interface ILobbyActionService
{
    Task<Lobby> ChangeColorAsync(string accessCode, int userId, Color newColor);
    Task<Lobby> ToggleReadyAsync(string accessCode, int userId);
    Task<Lobby> RollDiceAsync(string accessCode, int userId, int rolledNumber);
}