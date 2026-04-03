using System.Collections.Concurrent;


namespace LobbyService.Hubs.ConnectionMapping;

public class ConnectionMappingService
{
    private readonly ConcurrentDictionary<string, (string AccessCode, int UserId)> _connections = new();

    public void Add(string connectionId, string accessCode, int userId) => 
        _connections[connectionId] = (accessCode, userId);

    public bool TryGet(string connectionId, out string accessCode, out int userId)
    {
        if (_connections.TryGetValue(connectionId, out var info))
        {
            accessCode = info.AccessCode;
            userId = info.UserId;
            return true;
        }
        accessCode = "";
        userId = 0;
        return false;
    }

    public void Remove(string connectionId) => _connections.TryRemove(connectionId, out _);
}