using Microsoft.AspNetCore.Authentication;

namespace ThreeChapters.API.Auth;

public class TgAuthOptions: AuthenticationSchemeOptions
{
    public string? BotToken { get; set; }
}