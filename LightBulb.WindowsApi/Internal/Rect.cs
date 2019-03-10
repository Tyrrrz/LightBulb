using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
    public struct Rect
    {
        private readonly int Left;
        private readonly int Top;
        private readonly int Right;
        private readonly int Bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString() => $"L:{Left} T:{Top} R:{Right} B:{Bottom}";
    }
}