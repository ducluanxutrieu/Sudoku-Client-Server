using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class RoomPlayer : Form
    {
        List<Button> buttons = new List<Button>();
        List<ComboBox> combos = new List<ComboBox>();
        List<Label> users1 = new List<Label>();
        List<Label> users2 = new List<Label>();
        List<Label> serials = new List<Label>();
        int heightTableLayout = 33;
        int numberOfRow = 0;
        public RoomPlayer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
            Send("9x");
        }

        int port = 10002;
        IPEndPoint IP;
        Socket roomPlayerSocket;
        public static int passingID;
        public static int passingRoomID;

        void KhoiTaoListBanDau()
        {
            Button button = new Button();
            Label label = new Label();
            ComboBox combo = new ComboBox();

            label.Text = "";

            for(int i = 0; i<10;i++)
            {
                buttons.Add(button);
                users1.Add(label);
                users2.Add(label);
                combos.Add(combo);
            }
        }

        void Reconnect()
        {
            try
            {
                port = 10999;
                IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                roomPlayerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                roomPlayerSocket.Connect(IP);

                Thread listen = new Thread(Receive);
                listen.IsBackground = true;
                listen.Start();
            }
            catch
            {
                MessageBox.Show("Tất cả Server đều bị sập!\nChờ sửa xong Server rồi chơi tiếp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Close();
            }
        }

        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            roomPlayerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                roomPlayerSocket.Connect(IP);
                roomPlayerSocket.Send(Serialize("Test Clinet"));
                MessageBox.Show("Connect to Load Balance Successful!");
            }
            catch
            {
                try
                {
                    Reconnect();
                }
                catch
                {
                    MessageBox.Show("Không thể kết nối server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseConect();
                }

            }

            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] dataRcv = new byte[1024 * 5000];
                    roomPlayerSocket.Receive(dataRcv);

                    string message = (string)Deserialize(dataRcv);

                    string code = message.Substring(0, 2);
                    string data = message.Substring(2);

                    switch (code)
                    {
                        case "7x":
                            updateRoomUI();
                            break;
                        case "8x":
                            int roomID = int.Parse(data.Substring(0, 1));
                            string playerID = data.Substring(1);

                            if (users1[roomID].Text.Equals(""))
                            {
                                createNewUser(roomID, 1, playerID);
                            }
                            else if (users2[roomID].Text.Equals(""))
                            {
                                createNewUser(roomID, 2, playerID);
                            }

                            break;
                        case "9x":
                            int roomID9x = int.Parse(data.Substring(0, 1));
                            string player1ID = data.Substring(1,2);
                            string player2ID = data.Substring(3);
                            int Iplayer1ID = int.Parse(player1ID);
                            int Iplayer2ID = int.Parse(player2ID);
                            updateRoomUI();


                            if (users1[roomID9x].Text.Equals("") && Iplayer1ID != 0)
                            {
                                createNewUser(roomID9x, 1, player1ID);
                            }
                            else if (users2[roomID9x].Text.Equals("") && Iplayer2ID != 0)
                            {
                                createNewUser(roomID9x, 2, player2ID);
                            }
                            break;


                    }
                }
            }
            catch
            {
                CloseConect();
            }
        }

        void CloseConect()
        {
            try
            {
                roomPlayerSocket.Shutdown(SocketShutdown.Both);
                roomPlayerSocket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void Send(string s)
        {
            try
            {
                roomPlayerSocket.Send(Serialize(s));
            }
            catch
            {
                MessageBox.Show("Server chính đã bị sập!\nTrạng thái của bạn đã được Backup!\nReconnect tới Server phụ để chơi tiếp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        private void updateRoomUI()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    numberOfRow++;
                    tableLayoutPanel1.RowCount = numberOfRow;
                    tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));

                    createNewButton();
                    createNewComboboxMap(numberOfRow);
                    createNewLables(numberOfRow);

                    heightTableLayout += 33;
                    this.tableLayoutPanel1.Size = new System.Drawing.Size(776, heightTableLayout);
                });
            }
            else
            {
                numberOfRow++;
                tableLayoutPanel1.RowCount = numberOfRow;
                tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));

                createNewButton();
                createNewComboboxMap(numberOfRow);
                createNewLables(numberOfRow);

                heightTableLayout += 33;
                this.tableLayoutPanel1.Size = new System.Drawing.Size(776, heightTableLayout);
            }
        }

        private void btnNewRoom_Click(object sender, EventArgs e)
        {
            string tinnhan = "7x";
            Send(tinnhan);
        }

        private void createNewButton()
        {
            var button = new Button();
            //button4.Location = new Point(623, 96);
            button.Name = "button" + (numberOfRow - 1); //Trả về lại bắt đầu từ 0
            button.Size = new System.Drawing.Size(150, 27);
            //button.TabIndex = numberOfRow;
            button.Text = "Chọn";
            button.UseVisualStyleBackColor = true;
            Controls.Add(button);
            button.Click += new EventHandler(addControlForAllButtonOfList);
            buttons.Add(button);
            tableLayoutPanel1.Controls.Add(button, 4, numberOfRow);
            button.BringToFront();

        }

        private void createNewComboboxMap(int serial)
        {
            var maps = new ComboBox();
            //maps.Name = "map" + (serial - 1);
            maps.Size = new System.Drawing.Size(150, 27);
            //maps.TabIndex = serial;
            maps.Items.Add("4x4");
            maps.Items.Add("9x9");
            maps.Text = "4x4";
            combos.Add(maps);
            tableLayoutPanel1.Controls.Add(maps, 3, serial);
            maps.BringToFront();

        }

        private void createNewLables(int serial)
        {
            var user1 = new Label();
            var user2 = new Label();
            var serialRoom = new Label();
            //user.Name = "user1" + (serial - 1);
            user1.Size = new System.Drawing.Size(150, 27);
            user2.Size = new System.Drawing.Size(150, 27);
            serialRoom.Size = new System.Drawing.Size(100, 27);
            //user.TabIndex = serial;
            user1.Text = "";
            user2.Text = "";
            serialRoom.Text = serial - 1 + "";

            users1.Add(user1);
            users2.Add(user2);
            serials.Add(serialRoom);
            tableLayoutPanel1.Controls.Add(serialRoom, 0, serial);
            tableLayoutPanel1.Controls.Add(user1, 1, serial);
            tableLayoutPanel1.Controls.Add(user2, 2, serial);
            user1.BringToFront();
            user2.BringToFront();
            serialRoom.BringToFront();
        }

        private void addControlForAllButtonOfList(object sender, EventArgs e)
        {
            var button = sender as Button;

            int serial = int.Parse(button.Name.Substring(6));

            string b = combos[serial].SelectedItem.ToString();

            string s = idUser.Text;

            if (s.Equals(""))
            {
                MessageBox.Show("Vui lòng nhập ID!");
            }
            else if (checkID(s))
            {
                if (users1[serial].Text.Equals(""))
                {
                    Send("8x" + serial + idUser.Text);
                    Thread.Sleep(1000);


                    if (b.Equals("4x4"))
                    {
                        passingID = int.Parse(idUser.Text);
                        passingRoomID = serial;
                        ClientForm4x4 clientForm4X4 = new ClientForm4x4();
                        clientForm4X4.Show();
                    }
                    else
                    {
                        passingID = int.Parse(idUser.Text);
                        passingRoomID = serial;
                        ClientForm9x9 clientForm9X9 = new ClientForm9x9();
                        clientForm9X9.Show();
                    }
                }
                else if (users2[serial].Text.Equals(""))
                {
                    Send("8x" + serial + idUser.Text);
                    Thread.Sleep(1000);

                    if (b.Equals("4x4"))
                    {
                        passingID = int.Parse(idUser.Text);
                        passingRoomID = serial;
                        ClientForm4x4 clientForm4X4 = new ClientForm4x4();
                        clientForm4X4.Show();
                    }
                    else
                    {
                        passingID = int.Parse(idUser.Text);
                        passingRoomID = serial;
                        ClientForm9x9 clientForm9X9 = new ClientForm9x9();
                        clientForm9X9.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Bàn này đã đầy!");
                }
            }
            else
            {
                MessageBox.Show("ID này đã tồn tại, vui lòng nhập ID khác!");
            }
        }

        private bool checkID(object s)
        {
            //var id = s as Label;
            //return users1.Contains(s);

            //NẾu như giá trị trả về là false thì chứng tỏ trong mảng đó đã có id đó rồi
            for (int i = 0; i < users1.Count(); i++)
            {
                if (users1[i].Text.Equals(s) || users2[i].Text.Equals(s))
                    return false;
            }
            return true;
        }


        private void createNewUser(int serial, int posision, string idInput)
        {
            if (posision == 1)
            {
                //MessageBox.Show("0");
                users1[serial].Text = idInput;
            }
            else
            {
                //MessageBox.Show("1");
                users2[serial].Text = idInput;
            }

        }
    }
}
