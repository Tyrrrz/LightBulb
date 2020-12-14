using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly partial struct Rect
    {
        public int Left { get; }

        public int Top { get; }

        public int Right { get; }

        public int Bottom { get; }

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString() => $"L:{Left} T:{Top} R:{Right} B:{Bottom}";
    }

    internal partial struct Rect
    {
        public static Rect Empty { get; } = new();
    }

    internal partial struct Rect : IEquatable<Rect>
    {
        public bool Equals(Rect other) =>
            Left == other.Left &&
            Top == other.Top &&
            Right == other.Right &&
            Bottom == other.Bottom;

        public override bool Equals(object? obj) => obj is Rect other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

        public static bool operator ==(Rect a, Rect b) => a.Equals(b);

        public static bool operator !=(Rect a, Rect b) => !(a == b);
    }
}