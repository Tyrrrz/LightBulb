using System.Runtime.InteropServices;

// ReSharper disable ConvertToAutoPropertyWhenPossible (marshaling)

namespace LightBulb.Models
{
    /// <summary>
    /// Rectangle object
    /// <remarks>WinAPI struct</remarks>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        private readonly int _left;
        private readonly int _top;
        private readonly int _right;
        private readonly int _bottom;

        /// <summary>
        /// Left coordinate
        /// </summary>
        public int Left => _left;

        /// <summary>
        /// Top coordinate
        /// </summary>
        public int Top => _top;

        /// <summary>
        /// Right coordinate
        /// </summary>
        public int Right => _right;

        /// <summary>
        /// Bottom coordinate
        /// </summary>
        public int Bottom => _bottom;

        /// <summary>
        /// Total height
        /// </summary>
        public int Height => _bottom - _top;

        /// <summary>
        /// Total width
        /// </summary>
        public int Width => _right - _left;

        public Rect(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public override string ToString()
        {
            return $"L:{_left} T:{_top} R:{_right} B:{_bottom}";
        }
    }
}