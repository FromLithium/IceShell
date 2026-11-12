namespace IceShell.Tests.Utilities;

using System.Runtime.CompilerServices;


[AttributeUsage(AttributeTargets.Method)]
public class UnixFactAttribute : FactAttribute
{
    public UnixFactAttribute([CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) : base(sourceFilePath, sourceLineNumber)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsBrowser())
        {
            Skip = "This test will run on *nix systems only.";
        }
    }
}