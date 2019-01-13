using System;

namespace Server_2
{
    class Room2
    {
        public int iD { get; set; }
        public PlayerServer2 player1 { get; set; }
        public PlayerServer2 player2 { get; set; }
        public String mapPlayerRoom { get; set; }

        public int[,] twoD { get; set; }
        public int[,] twoDCheckFull { get; set; }
        public int[,] twoDFrom2Client { get; set; }
        public int[,] twoDCheck { get; set; }


        public Room2(int iD, PlayerServer2 player1, PlayerServer2 player2, string mapPlayerRoom)
        {
            this.iD = iD;
            this.player1 = player1;
            this.player2 = player2;
            this.mapPlayerRoom = mapPlayerRoom;
        }
    }
}
