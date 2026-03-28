namespace LobbyService.Data.Records;

public record JoinRequest(string AccessCode, int UserId, string Username);