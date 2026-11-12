namespace IceShell.Tests.Utilities;

using System.Runtime.CompilerServices;


[AttributeUsage(AttributeTargets.Method)]
public class WindowsFactAttribute : FactAttribute
{
    public WindowsFactAttribute([CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) : base(sourceFilePath, sourceLineNumber)
    {
        if (!OperatingSystem.IsWindows())
        {
            Skip = "This test will run on Windows only.";
        }
    }
}