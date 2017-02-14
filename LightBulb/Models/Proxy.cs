namespace LightBulb.Models
{
    /// <summary>
    /// Internet proxy settings
    /// </summary>
    public class Proxy
    {
        public string Host { get; }

        public ushort Port { get; }

        public string Username { get; }

        public string Password { get; }

        public Proxy(string host, ushort port, string username = null, string password = null)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }
    }
}