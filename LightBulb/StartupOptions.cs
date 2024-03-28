using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBulb;

public partial class StartupOptions
{
    public bool IsInitiallyHidden { get; init; }
}

public partial class StartupOptions
{
    public static string IsInitiallyHiddenArgument { get; } = "--start-hidden";

    public static StartupOptions Current { get; } =
        Parse(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public static StartupOptions Parse(IReadOnlyList<string> commandLineArgs) =>
        new()
        {
            IsInitiallyHidden = commandLineArgs.Contains(
                IsInitiallyHiddenArgument,
                StringComparer.OrdinalIgnoreCase
            )
        };
}
