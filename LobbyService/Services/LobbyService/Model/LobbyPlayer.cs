using LobbyService.Data.Enumerations;

namespace LobbyService.Model;

public class LobbyPlayer
{
    public int Id { get; set; }
    public int UserId {get; set;}
    public string Username { get; set; } = string.Empty;
    public int LobbyId {get; set;}
    public Color Color { get; set; }
    public int RolledNumber {get; set;}
    public bool IsReady { get; set; }
}