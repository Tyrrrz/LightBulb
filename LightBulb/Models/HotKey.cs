using System.Text;
using Avalonia.Input;

namespace LightBulb.Models;

public readonly record struct HotKey(
    PhysicalKey Key,
    KeyModifiers Modifiers = KeyModifiers.None
)
{
    public static HotKey None { get; } = new();
    
    public override string ToString()
    {
        if (Key == PhysicalKey.None && Modifiers == KeyModifiers.None)
            return "< None >";

        var buffer = new StringBuilder();

        if (Modifiers.HasFlag(KeyModifiers.Control))
            buffer.Append("Ctrl + ");
        if (Modifiers.HasFlag(KeyModifiers.Shift))
            buffer.Append("Shift + ");
        if (Modifiers.HasFlag(KeyModifiers.Alt))
            buffer.Append("Alt + ");
        if (Modifiers.HasFlag(KeyModifiers.Meta))
            buffer.Append("Win + ");

        buffer.Append(Key);

        return buffer.ToString();
    }
}
