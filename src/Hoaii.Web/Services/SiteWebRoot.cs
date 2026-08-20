namespace Hoaii.Web.Services;

/// <summary>
/// Where wwwroot actually is. WebRootPath comes back null when the host can't find a wwwroot next
/// to the entry assembly — launching the exe straight out of bin/ does it — so both the code that
/// writes uploads and the code that serves them have to fall back the same way. They used to fall
/// back differently, which meant an upload could land in one folder while /uploads was served from
/// another (blank thumbnails), or the app could die at startup on Path.Combine(null).
/// </summary>
public static class SiteWebRoot
{
    public static string For(IWebHostEnvironment env)
    {
        var root = env.WebRootPath;
        if (string.IsNullOrEmpty(root))
        {
            root = Path.Combine(env.ContentRootPath, "wwwroot");
        }
        Directory.CreateDirectory(root);
        return root;
    }

    /// <summary>Folder that backs the /uploads URL prefix.</summary>
    public static string Uploads(IWebHostEnvironment env)
    {
        var dir = Path.Combine(For(env), "uploads");
        Directory.CreateDirectory(dir);
        return dir;
    }
}
