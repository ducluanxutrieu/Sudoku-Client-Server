using System.Net.Sockets;


namespace Load_Balance
{
    class PlayerLoad
    {
        public int iD { get; set; }
        public Socket socket { get; set; }

        public PlayerLoad(int iD, Socket socket)
        {
            this.iD = iD;
            this.socket = socket;
        }
    }
}
