using HashidsNet;

namespace LobbyService.Services.LobbyCode;

public class LobbyCodeService
{
    private readonly Hashids _hashids;

    public LobbyCodeService(IConfiguration configuration)
    {
        var salt = configuration["GameSettings:HashIdSalt"];

        if (string.IsNullOrEmpty(salt) || salt == "PLACEHOLDER_SALT")
        {
            throw new InvalidOperationException("HashIdSalt nije ispravno konfigurisan u User Secrets ili appsettings.json!");
        }

        _hashids = new Hashids(salt, 6, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
    }

    public string EncodeLobbyId(int lobbyId)
    {
        return _hashids.Encode(lobbyId);
    }
    public int? DecodeLobbyCode(string lobbyCode)
    {
        var decoded = _hashids.Decode(lobbyCode);
        if (decoded.Length > 0)
        {
            return decoded[0];
        }
        return null;
    }
}