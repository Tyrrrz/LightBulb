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

    public static StartupOptions Parse(IEnumerable<string> commandLineArguments) =>
        new()
        {
            IsInitiallyHidden = commandLineArguments.Contains(
                IsInitiallyHiddenArgument,
                StringComparer.OrdinalIgnoreCase
            )
        };
}

public partial class StartupOptions
{
    public static StartupOptions Current { get; } = Parse(Environment.GetCommandLineArgs().Skip(1));
}
