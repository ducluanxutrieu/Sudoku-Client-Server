using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server
{
    public partial class Server1Form : Form
    {
        //firstGame dùng để phân biệt để tránh new mảng nhiều lần ở server 2
        bool firstGame = true;

        Random random = new Random();


        public Server1Form()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void ServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseConect();
        }


        //Random các hàng và cột để thực hiện swap các hàng và cột đó để cho ra một mảng hoàn toàn mới
        private void RandomRowAndColumn(int map, int roomID)
        {
            for (int i = 0; i < map; i++)
            {
                int local = random.Next(1, map);
                SwapRow(local, map, roomID);
                local = random.Next(1, map);
                SwapColumn(local, map, roomID);
            }
        }

        //Đổi vị trí các dòng với nhau
        private void SwapRow(int rowA, int map, int roomID)
        {
            int rowB = CheckDistance(rowA, map);
            if (rowA != rowB) /*Nếu như giá trị A và B khác nhau mới SWap*/
            {
                for (int i = 0; i < map; i++)
                {
                    int tam;
                    tam = roomList[roomID].twoD[rowA, i];
                    roomList[roomID].twoD[rowA, i] = roomList[roomID].twoD[rowB, i];
                    roomList[roomID].twoD[rowB, i] = tam;
                }
            }

        }

        //Đổi vị trí các cột với nhau. 
        private void SwapColumn(int columnA, int map, int roomID)
        {
            int columnB = CheckDistance(columnA, map);
            for (int i = 0; i < map; i++)
            {
                int tam;
                tam = roomList[roomID].twoD[i, columnA];
                roomList[roomID].twoD[i, columnA] = roomList[roomID].twoD[i, columnB];
                roomList[roomID].twoD[i, columnB] = tam;
            }
        }

        //Kiểm tra nó thuộc ô nào và trả về một giá trị random nằm trong ô đó để hàm Swap hoạt động
        private int CheckDistance(int value, int map)
        {
            int temp = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(map)));
            int min = value / temp;
            min *= temp;
            int B = random.Next(min, min + temp); /*Tìm một hàng hay một cột nào đó để swap*/
            return B;
        }

        //Tạo giá trị chuẩn cho mảng ban đầu
        private void CreateMatrix(int map, int roomID)
        {
            int itemValue = 0;
            int temp = 1;
            for (int i = 0; i < map; i++)
            {
                for (int j = 0; j < map; j++)
                {
                    itemValue = temp + j;
                    if (itemValue > map) itemValue -= map;
                    roomList[roomID].twoD[i, j] = itemValue;
                    //firstMap[i , j ] = 5;
                }
                temp += Convert.ToInt32(Math.Sqrt(Convert.ToDouble(map)));
                if (temp > map) temp -= (map - 1);
            }
        }

        //Chuyển mảng hai chiều thành chuỗi để gửi đi
        private string ChuyenMangSangChuoi(int map, int roomID)
        {
            string temp = "";

            for (int r = 0; r < map; r++)
                for (int c = 0; c < map; c++)
                    temp += roomList[roomID].twoD[r, c].ToString();
            return temp;
        }

        //Bôi ô ngẫu nhiên
        private void BoiONgauNhien(int valueRandom, int map, int roomID)
        {
            int temp;
            for (int i = 0; i < map; i++)
            {
                for (int j = 0; j < map; j++)
                {
                    temp = random.Next(0, 10);
                    if (valueRandom > temp)
                        roomList[roomID].twoD[i, j] = 0;
                }
            }
        }

        private void checkGeneral(int map, int roomID)
        {
            for (int i = 0; i < map; i++)
            {
                checkHiddenRow(i, map, roomID);
            }
            for (int i = 0; i < map; i++)
            {
                checkHiddenColumn(i, map, roomID);
            }
            for (int i = 0; i < map; i++)
            {
                if (i % Math.Sqrt(map) == 0)
                {
                    if (map == 9)
                    {
                        checkHiddenBigCell(i, 0, map, roomID);
                        checkHiddenBigCell(i, 3, map, roomID);
                        checkHiddenBigCell(i, 6, map, roomID);
                    }
                    else
                    {
                        checkHiddenBigCell(i, 0, map, roomID);
                        checkHiddenBigCell(i, 2, map, roomID);
                    }
                }
            }
        }

        //Kiểm tra xem có trường hợp nguyên một hàng hay một cột cũng như một ô bự nào
        //đang có trường hộp điền toàn bộ không
        private void checkHiddenRow(int row, int map, int roomID)
        {
            int numberOfCellHidden = 0;
            for (int i = 0; i < map; i++)
            {
                if (roomList[roomID].twoD[row, i] != 0)
                {
                    numberOfCellHidden++;
                }
            }

            //Nếu giá trị của numberOfCellHidden vượt quá 8/9 hoặc 3/4 sẽ phải ramdom lại
            if (numberOfCellHidden >= ((map / 2) * 2)) //Nếu là 9 thì sẽ là 8 còn nếu là 4 thì sẽ đối chiếu với 4
            {
                roomList[roomID].twoD[row, random.Next(0, map)] = 0; //random một vị trí để Hidden again
            }
        }

        private void checkHiddenColumn(int column, int map, int roomID)
        {
            int numberOfCellHidden = 0;
            for (int i = 0; i < map; i++)
            {
                if (roomList[roomID].twoD[i, column] != 0)
                {
                    numberOfCellHidden++;
                }
            }

            //Nếu giá trị của numberOfCellHidden vượt quá 8/9 hoặc 3/4 sẽ phải ramdom lại
            if (numberOfCellHidden >= ((map / 2) * 2)) //Nếu là 9 thì sẽ là 8 còn nếu là 4 thì sẽ đối chiếu với 4
            {
                roomList[roomID].twoD[random.Next(0, map), column] = 0; //random một vị trí để Hidden again
            }
        }

        private void checkHiddenBigCell(int row, int column, int map, int roomID)
        {
            int sqrtMap = Convert.ToInt32(Math.Sqrt(map));
            int numberOfCellHidden = 0;
            for (int i = row; i < row + sqrtMap; i++)
            {
                for (int j = column; j < column + sqrtMap; j++)
                {
                    if (roomList[roomID].twoD[i, j] != 0)
                    {
                        numberOfCellHidden++;
                    }
                }
            }

            //Nếu giá trị của numberOfCellHidden vượt quá 8/9 hoặc 3/4 sẽ phải ramdom lại
            if (numberOfCellHidden >= ((map / 2) * 2)) //Nếu là 9 thì sẽ là 8 còn nếu là 4 thì sẽ đối chiếu với 4
            {
                roomList[roomID].twoD[random.Next(row, row + sqrtMap), random.Next(column, column + sqrtMap)] = 0; //random một vị trí để Hidden again
            }
        }

        //Gửi dữ liệu cho client
        private string NewGame(int level, int maps, int roomID)
        {
            //roomList[roomID] = new Room(maps);
            roomList[roomID].twoD = new int[maps, maps];
            roomList[roomID].twoDCheckFull = new int[maps, maps];
            roomList[roomID].twoDFrom2Client = new int[maps, maps];
            roomList[roomID].twoDCheck = new int[maps, maps];

            CreateMatrix(maps, roomID);
            RandomRowAndColumn(maps, roomID);
            BoiONgauNhien(level, maps, roomID);
            checkGeneral(maps, roomID);
            roomList[roomID].twoDCheck = roomList[roomID].twoD.Clone() as int[,];
            roomList[roomID].twoDCheckFull = roomList[roomID].twoD.Clone() as int[,];
            //twoDCheck = twoD;
            string s = ChuyenMangSangChuoi(maps, roomID);
            return s;
        }

        /// <summary>
        /// gửi cho tất cả client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        IPEndPoint IP;
        Socket server;
        Socket server1toServer2;

        List<Room1> roomList;
        int roomID = 0;

        void ConnectToServer2()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10999);
            server1toServer2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server1toServer2.Connect(IP);

            server1toServer2.Send(Serialize("10"));
        }



        //bool server2isAlive = true;

        //Comment Test

        //void SyncServer2(string s)
        //{
        //    try
        //    {
        //        if (server2isAlive)
        //            server1toServer2.Send(Serialize(s));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error inSyncServer2" + ex.ToString());
        //        server2isAlive = false;
        //    }
        //}


        List<Socket> roomSocketList = new List<Socket>();
        /// <summary>
        /// Kết nối tới server
        /// </summary>
        void Connect()
        {
            roomList = new List<Room1>();
            
            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            server.Bind(IP);

            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket socket;
                        //Player player = new Player();
                        socket = server.Accept();

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(socket);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error listen Connect" + ex.ToString());
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }


            });
            Listen.IsBackground = true;
            Listen.Start();
        }

        /// <summary>
        /// đóng kết nối hiện thời
        /// </summary>
        void CloseConect()
        {
            server.Close();
        }

        /// <summary>
        /// nhận tin nhắn
        /// </summary>
        void Receive(object obj)
        {
            Socket socket = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] dataRcv = new byte[1024 * 5000];
                    socket.Receive(dataRcv);

                    string message = (string)Deserialize(dataRcv);

                    string code = message.Substring(0, 2);
                    string data = message.Substring(2);

                    switch (code)
                    {
                        case "0x": //New Game
                            newGamePlayer(data);
                            break;
                        case "1x": //Playing Game
                            playGame(data);
                            break;
                        case "2x": //Nhận ID người chơi và thêm người chơi vào đúng phòng
                            receiveAndAddPlayer(data, socket);
                            break;
                        case "7x": //Tạo 1 phòng trống
                            createEmptyRoom();
                            break;
                        case "8x": //Lấy thông tin người chơi khi vào phòng
                            getInfoPlayer(data);
                            break;
                        case "9x": //Có thêm thành viên mới
                            roomSocketList.Add(socket);
                            break;

                        case "10": //Tin nhắn gửi trả lại thông tin từ Server2
                            int player1ID = int.Parse(data.Substring(0, 2));
                            int player1CountTime = int.Parse(data.Substring(2, 1));
                            int player2ID = int.Parse(data.Substring(3, 2));
                            int player2CountTime = int.Parse(data.Substring(5));

                            PlayerServer1 player1 = new PlayerServer1(player1ID, player1CountTime, null);
                            PlayerServer1 player2 = new PlayerServer1(player2ID, player2CountTime, null);

                            roomList.Add(new Room1(this.roomID, player1, player2, "4x4"));

                            this.roomID++;
                            break;
                        case "12":
                            int roomID = int.Parse(data.Substring(0, 1));
                            int playerID = int.Parse(data.Substring(1));

                            PlayerServer1 player;

                            if (roomList[roomID].player1.iD == playerID)
                            {
                                int iD = roomList[roomID].player1.iD;
                                int countTime = roomList[roomID].player1.countTime;

                                player = new PlayerServer1(iD, countTime, socket);

                                roomList[roomID].player1 = player;

                                roomList[roomID].player1.socket.Send(Serialize("5x"));

                                //SyncServer2("5x2x" + roomID + playerID);
                            }
                            else
                            {
                                int iD = roomList[roomID].player2.iD;
                                int countTime = roomList[roomID].player2.countTime;

                                player = new PlayerServer1(iD, countTime, socket);

                                roomList[roomID].player2 = player;

                                roomList[roomID].player2.socket.Send(Serialize("5x"));

                                //SyncServer2("5x2x" + roomID + playerID);
                            }
                            break;
                    }

                    lsvMessage.Items.Add(new ListViewItem() { Text = "Connection accepted from " + socket.RemoteEndPoint });
                    AddMessage(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Receive Server" + ex.ToString());
                //Báo cho server 2 biết là có thằng out
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].player1.socket == socket)
                    {
                        string tinnhanSync = "6x" + i + socket;
                        //SyncServer2(tinnhanSync);
                        AddMessage(tinnhanSync);

                        roomList[i].player1.iD = 0;
                        roomList[i].player1.countTime = 0;
                        roomList[i].player1.socket = null;
                    }

                    else if (roomList[i].player2.socket == socket)
                    {
                        string tinnhanSync = "6x" + i + socket;
                        //SyncServer2(tinnhanSync);
                        AddMessage(tinnhanSync);

                        roomList[i].player2.iD = 0;
                        roomList[i].player2.countTime = 0;
                        roomList[i].player2.socket = null;
                    }
                }
            }
        }

        private void newGamePlayer(String data)
        {
            int roomID = int.Parse(data.Substring(0, 1));
            string maps = data.Substring(1, 1);
            string data2 = data.Substring(2);

            int map = int.Parse(maps);
            int level = int.Parse(data2);

            //Kiểm tra xem có đủ 2 người chơi hay không
            if (roomList[roomID].player1.iD != 0 && roomList[roomID].player2.iD != 0)
            {
                //Quyết định người nào chơi trước
                string tinnhan1 = "2x1";
                string tinnhan2 = "2x2";

                //MessageBox.Show(roomList[roomID].player1.socket + "Socket player 1");
                //MessageBox.Show(roomList[roomID].player2.socket + "Socket player 2");

                if (roomList[roomID].player1.iD >= roomList[roomID].player2.iD)
                {
                    roomList[roomID].player1.socket.Send(Serialize(tinnhan1));
                    roomList[roomID].player2.socket.Send(Serialize(tinnhan2));
                }
                else
                {
                    roomList[roomID].player2.socket.Send(Serialize(tinnhan1));
                    roomList[roomID].player1.socket.Send(Serialize(tinnhan2));
                }

                //Gửi game cho 2 người chơi
                string tinnhan = "0x" + NewGame(level, map, roomID);
                roomList[roomID].player1.socket.Send(Serialize(tinnhan));
                roomList[roomID].player2.socket.Send(Serialize(tinnhan));

                //Gửi luôn màn chơi cho Server2 để lưu lại trạng thái
                string tinnhanSync = "";
                if (firstGame == true)
                {
                    tinnhanSync = "5x0x1" + roomID + map + ChuyenMangSangChuoi(map, roomID);
                    firstGame = false;
                }
                else
                    tinnhanSync = "5x0x2" + roomID + map + ChuyenMangSangChuoi(map, roomID);
                //SyncServer2(tinnhanSync);
            }
            //Thông báo cho client biết là chỉ có một người và chờ người thứ 2
            else
            {
                string tinnhan3 = "3x";

                roomList[roomID].player1.socket.Send(Serialize(tinnhan3));
            }
        }
        private void playGame(String data)
        {
            int roomID1x = int.Parse(data.Substring(0, 1));
            int playerID = int.Parse(data.Substring(1, 2));
            int mapdangchoi = int.Parse(data.Substring(3, 1));
            string manchoi = data.Substring(4);
            string tinnhan4 = "1x" + manchoi;

            //Đồng bộ màn chơi qua server 2
            string tinnhanSync2 = "5x1x" + roomID1x + playerID + mapdangchoi + manchoi;
            //SyncServer2(tinnhanSync2);

            if (mapdangchoi == 4)
                ChuyenChuoiSangMang4(manchoi, roomID1x);
            else
                ChuyenChuoiSangMang9(manchoi, roomID1x);

            if (mapdangchoi == 4) //Nếu như đang chơi map 4x4
            {
                for (int r = 0; r < 4; r++)
                    for (int c = 0; c < 4; c++)
                    {
                        if (roomList[roomID1x].twoDFrom2Client[r, c] != 0 && roomList[roomID1x].twoD[r, c] != roomList[roomID1x].twoDFrom2Client[r, c])
                        {
                            roomList[roomID1x].twoD[r, c] = roomList[roomID1x].twoDFrom2Client[r, c];
                            roomList[roomID1x].twoDCheckFull[r, c] = roomList[roomID1x].twoDFrom2Client[r, c];
                        }

                    }

                for (int r = 0; r < 4; r++)
                    for (int c = 0; c < 4; c++)
                    {
                        if (roomList[roomID1x].twoD[r, c] != roomList[roomID1x].twoDCheck[r, c])
                        {
                            if (CheckValid4(r, c, roomList[roomID1x].twoD[r, c], roomID1x))
                            {
                                if (roomList[roomID1x].player1.iD == playerID)
                                    roomList[roomID1x].player1.countTime++;
                                else
                                    roomList[roomID1x].player2.countTime++;
                                roomList[roomID1x].twoDCheck = roomList[roomID1x].twoD.Clone() as int[,];
                            }
                            else
                                roomList[roomID1x].twoD[r, c] = 0;
                        }
                    }

                if (checkFinish4(roomID1x)) //Nếu chơi xong rồi thì thông báo kết quả
                {
                    if (roomList[roomID1x].player1.iD == playerID)
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan4));
                    else
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan4));

                    int max = roomList[roomID1x].player1.countTime;
                    if (roomList[roomID1x].player2.countTime > max)
                        max = roomList[roomID1x].player2.countTime;

                    if (roomList[roomID1x].player1.countTime == roomList[roomID1x].player2.countTime)
                    {
                        string tinnhan7 = "4x3";
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan7));
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan7));
                    }
                    else
                    {
                        string tinnhan5 = "4x1";
                        string tinnhan6 = "4x2";

                        if (roomList[roomID1x].player1.countTime == max)
                        {
                            roomList[roomID1x].player1.socket.Send(Serialize(tinnhan5));
                            roomList[roomID1x].player2.socket.Send(Serialize(tinnhan6));
                        }

                        else
                        {
                            roomList[roomID1x].player2.socket.Send(Serialize(tinnhan5));
                            roomList[roomID1x].player1.socket.Send(Serialize(tinnhan6));
                        }

                    }
                }
                else //Nếu không thì tiếp tục
                {
                    if (roomList[roomID1x].player1.iD == playerID)
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan4));
                    else
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan4));
                }
            }
            else //Nếu như đang chơi map 9x9
            {
                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                    {
                        if (roomList[roomID1x].twoDFrom2Client[r, c] != 0 && roomList[roomID1x].twoD[r, c] != roomList[roomID1x].twoDFrom2Client[r, c])
                        {
                            roomList[roomID1x].twoD[r, c] = roomList[roomID1x].twoDFrom2Client[r, c];
                            roomList[roomID1x].twoDCheckFull[r, c] = roomList[roomID1x].twoDFrom2Client[r, c];
                        }

                    }

                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                    {
                        if (roomList[roomID1x].twoD[r, c] != roomList[roomID1x].twoDCheck[r, c])
                        {
                            if (CheckValid9(r, c, roomList[roomID1x].twoD[r, c], roomID1x))
                            {
                                //player.countTime++;
                                roomList[roomID1x].twoDCheck = roomList[roomID1x].twoD.Clone() as int[,];
                            }
                            else
                                roomList[roomID1x].twoD[r, c] = 0;
                        }
                    }

                if (checkFinish9(roomID1x)) //Nếu chơi xong rồi thì thông báo kết quả
                {
                    if (roomList[roomID1x].player1.iD == playerID)
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan4));
                    else
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan4));

                    int max = roomList[roomID1x].player1.countTime;
                    if (roomList[roomID1x].player2.countTime > max)
                        max = roomList[roomID1x].player2.countTime;

                    if (roomList[roomID1x].player1.countTime == roomList[roomID1x].player2.countTime)
                    {
                        string tinnhan7 = "4x3";
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan7));
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan7));
                    }
                    else
                    {
                        string tinnhan5 = "4x1";
                        string tinnhan6 = "4x2";

                        if (roomList[roomID1x].player1.countTime == max)
                        {
                            roomList[roomID1x].player1.socket.Send(Serialize(tinnhan5));
                            roomList[roomID1x].player2.socket.Send(Serialize(tinnhan6));
                        }

                        else
                        {
                            roomList[roomID1x].player2.socket.Send(Serialize(tinnhan5));
                            roomList[roomID1x].player1.socket.Send(Serialize(tinnhan6));
                        }

                    }
                }
                else //Nếu không thì tiếp tục
                {
                    if (roomList[roomID1x].player1.iD == playerID)
                        roomList[roomID1x].player2.socket.Send(Serialize(tinnhan4));
                    else
                        roomList[roomID1x].player1.socket.Send(Serialize(tinnhan4));
                }
            }
        }
        private void receiveAndAddPlayer(String data, Socket socket)
        {
            int roomID = int.Parse(data.Substring(0, 1));
            int playerID = int.Parse(data.Substring(1));
            //MessageBox.Show(socket + ": Player Socket");
            PlayerServer1 player = new PlayerServer1(playerID, 0, socket);
            if (roomList[roomID].player1.socket == null)
            {
                roomList[roomID].player1 = player;
            }
            else
            {
                roomList[roomID].player2 = player;
            }

            //MessageBox.Show(roomList[roomID].player1.socket + "Socket Player 1 Receive");
            //MessageBox.Show(roomList[roomID].player2.socket + "Socket Player 2 Receive");

            //SyncServer2("5x2x" + roomID + playerID);
        }
        private void createEmptyRoom()
        {
            PlayerServer1 player1 = new PlayerServer1(0, 0, null);
            PlayerServer1 player2 = new PlayerServer1(0, 0, null);
            Room1 room = new Room1(roomID, player1, player2, "4x4");
            string tinnhanTaoPhong = "7x" + roomID;
            foreach (Socket socket in roomSocketList)
                socket.Send(Serialize(tinnhanTaoPhong));
            roomList.Add(room);
            roomID++;

            //SyncServer2("5x7x");
        }

        private void getInfoPlayer(String data)
        {
            int roomIDofPlayer = int.Parse(data.Substring(0, 1));
            int PlayerID = int.Parse(data.Substring(1));
            if (roomList[roomIDofPlayer].player1.iD == 0)
            {

                roomList[roomIDofPlayer].player1.iD = PlayerID;
                string tinnhanNguoiChoiVaoPhong = "8x" + data;
                foreach (Socket item in roomSocketList)
                    item.Send(Serialize(tinnhanNguoiChoiVaoPhong));
            }
            else
            {
                roomList[roomIDofPlayer].player2.iD = PlayerID;
                string tinnhanNguoiChoiVaoPhong = "8x" + data;
                foreach (Socket item in roomSocketList)
                    item.Send(Serialize(tinnhanNguoiChoiVaoPhong));
            }
        }

        //Chuyển chuỗi sang mảng
        void ChuyenChuoiSangMang4(string s, int roomID)
        {
            int count = 0;
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    roomList[roomID].twoDFrom2Client[r, c] = Convert.ToInt32((char.GetNumericValue(s[count])));
                    count++;
                }
        }
        void ChuyenChuoiSangMang9(string s, int roomID)
        {
            int count = 0;
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    roomList[roomID].twoDFrom2Client[r, c] = Convert.ToInt32((char.GetNumericValue(s[count]))); ;
                    count++;
                }
        }
        //Kiểm tra xem đã điền hết ô chưa
        bool checkFinish4(int roomID)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (roomList[roomID].twoDCheckFull[r, c] == 0)
                        return false;
            return true;
        }
        bool checkFinish9(int roomID)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (roomList[roomID].twoDCheckFull[r, c] == 0)
                        return false;
            return true;
        }

        //Check xem người chơi có đúng không, nếu đúng thì lưu vào twoD[,] và cộng thêm 1 nước đúng cho player
        bool CheckValid4(int x, int y, int k, int roomID)
        {
            for (int i = 0; i < 4; i++)
            {

                if (roomList[roomID].twoD[x, i] == k && i != y) return false;

            }
            for (int i = 0; i < 4; i++)
            {
                if (roomList[roomID].twoD[i, y] == k && (i != x)) return false;
            }
            int a = x / 2, b = y / 2;
            for (int i = 2 * a; i < 2 * a + 2; i++)
            {
                for (int j = 2 * b; j < 2 * b + 2; j++)
                {
                    if (roomList[roomID].twoD[i, j] == k && (i != x) && (j != y)) return false;
                }
            }
            return true;
        }

        bool CheckValid9(int x, int y, int k, int roomID)
        {
            for (int i = 0; i < 9; i++)
            {

                if (roomList[roomID].twoD[x, i] == k && i != y) return false;

            }
            for (int i = 0; i < 9; i++)
            {
                if (roomList[roomID].twoD[i, y] == k && (i != x)) return false;
            }
            int a = x / 3, b = y / 3;
            for (int i = 3 * a; i < 3 * a + 3; i++)
            {
                for (int j = 3 * b; j < 3 * b + 3; j++)
                {
                    if (roomList[roomID].twoD[i, j] == k && (i != x) && (j != y)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// thêm tin nhắn vào listView
        /// </summary>
        /// <param name="obj"></param>
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
        }

        /// <summary>
        /// phân mảnh
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        /// <summary>
        /// gom mảnh
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }


        private void connectServer2Btn_Click(object sender, EventArgs e)
        {
            ConnectToServer2();
        }
    }
}
