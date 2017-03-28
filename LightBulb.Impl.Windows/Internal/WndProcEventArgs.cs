using System;
using System.Windows.Forms;

namespace LightBulb.Internal
{
    /// <summary>
    /// Event arguments associated with a windows procedure
    /// </summary>
    internal class WndProcEventArgs : EventArgs
    {
        public WndProcEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}