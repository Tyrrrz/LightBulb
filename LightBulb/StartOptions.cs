using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBulb;

public partial class StartOptions
{
    public bool IsInitiallyHidden { get; init; }
}

public partial class StartOptions
{
    public static string IsInitiallyHiddenArgument { get; } = "--start-hidden";

    public static StartOptions Current { get; } =
        Parse(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public static StartOptions Parse(IReadOnlyList<string> commandLineArgs) =>
        new()
        {
            IsInitiallyHidden = commandLineArgs.Contains(
                IsInitiallyHiddenArgument,
                StringComparer.OrdinalIgnoreCase
            )
        };
}
