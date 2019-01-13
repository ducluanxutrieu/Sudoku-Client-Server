using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room1
    {
        public int iD { get; set; }
        public PlayerServer1 player1 { get; set; }
        public PlayerServer1 player2 { get; set; }
        public String mapPlayerRoom { get; set; }

        public int[,] twoD { get; set; }
        public int[,] twoDCheckFull { get; set; }
        public int[,] twoDFrom2Client { get; set; }
        public int[,] twoDCheck { get; set; }


        public Room1(int iD, PlayerServer1 player1, PlayerServer1 player2, string mapPlayerRoom)
        {
            this.iD = iD;
            this.player1 = player1;
            this.player2 = player2;
            this.mapPlayerRoom = mapPlayerRoom;
        }
    }
}
