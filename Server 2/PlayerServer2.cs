using System.Net.Sockets;


namespace Server_2
{
    class PlayerServer2
    {
        public int iD { get; set; }
        public int countTime { get; set; }
        public Socket socket { get; set; }

        public PlayerServer2(int iD, int countTime, Socket socket)
        {
            this.iD = iD;
            this.countTime = countTime;
            this.socket = socket;
        }
    }
}
