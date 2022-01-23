namespace AudioBoos.Server.Helpers;

public static class Constants {
    public const string SessionCookie = "_abs";
    public const string AccessTokenCookie = "_aba";
    public const string UsernameCookie = "_abu";
    public const string RefreshTokenCookie = "_abr";

    // public const bool DebugMode = _isDebug(System.Reflection.Assembly.GetExecutingAssembly());
    // public const bool ReleaseMode = _isRelease(System.Reflection.Assembly.GetExecutingAssembly());
#if DEBUG
    public const bool DebugMode = true;
#else
    public const bool DebugMode = false;
#endif
}
