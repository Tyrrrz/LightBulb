using System;

namespace LightBulb.PlatformInterop;

public partial class Process : IDisposable
{
    private Process() { }

    public string? TryGetExecutableFilePath() => null;

    public void Dispose() { }
}

public partial class Process
{
    public static Process? TryGet(int processId) => null;
}
