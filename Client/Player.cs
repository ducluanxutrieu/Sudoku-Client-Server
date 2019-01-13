using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Client
{
    class Player
    {
        public int iD { get; set; }
        public Socket socket { get; set; }

        public Player(int iD, Socket socket)
        {
            this.iD = iD;
            this.socket = socket;
        }
    }
}
