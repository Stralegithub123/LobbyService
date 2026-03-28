namespace LobbyService.Model;
using System.Text.Json.Serialization;
using System.Threading;

public class Lobby
{
    public int Id { get; set; }
    public int HostUserId { get; set; }
    public string AccessCode { get; set; } = string.Empty;
    public List<LobbyPlayer> Players { get; set; } = new List<LobbyPlayer>();
    [JsonIgnore] 
    public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1);
}