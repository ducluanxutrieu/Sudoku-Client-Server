using System.Net.Sockets;


namespace Server
{
    class PlayerServer1
    {
        public int iD { get; set; }
        public int countTime { get; set; }
        public Socket socket { get; set; }

        public PlayerServer1(int iD, int countTime, Socket socket)
        {
            this.iD = iD;
            this.countTime = countTime;
            this.socket = socket;
        }
    }
}
