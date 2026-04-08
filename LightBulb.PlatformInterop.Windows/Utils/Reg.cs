using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LightBulb.PlatformInterop.Utils;

internal static class Reg
{
    private static void Run(IReadOnlyList<string> arguments, bool isElevated = false)
    {
        using var process = new System.Diagnostics.Process();
        process.StartInfo = new ProcessStartInfo("reg");

        foreach (var arg in arguments)
            process.StartInfo.ArgumentList.Add(arg);

        if (isElevated)
        {
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
        }

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new IOException(
                $"""
                Failed to run 'reg.exe', received exit code {process.ExitCode}. Arguments:
                {string.Join(" ", arguments)}
                """
            );
        }
    }

    public static void SetValue<T>(string key, string entry, T value)
    {
        var entryType = value switch
        {
            null => "REG_NONE",
            int => "REG_DWORD",
            string => "REG_SZ",
            _ => throw new NotSupportedException(
                $"Unsupported registry value type '{value.GetType()}'."
            ),
        };

        Run(["add", key, "/v", entry, "/d", value?.ToString() ?? "", "/t", entryType, "/f"], true);
    }

    public static void DeleteValue(string key, string entry)
    {
        Run(["delete", key, "/v", entry, "/f"], true);
    }
}
