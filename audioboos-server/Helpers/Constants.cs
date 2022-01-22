namespace AudioBoos.Server.Helpers;

public static class Constants {
    public const string SessionCookie = "_abs";
    public const string AccessTokenCookie = "_aba";
    public const string UsernameCookie = "_abu";
    public const string RefreshTokenCookie = "_abr";

#if DEBUG
    public const bool DebugMode = true;
#else
    public const bool DebugMode = true;
#endif
}
