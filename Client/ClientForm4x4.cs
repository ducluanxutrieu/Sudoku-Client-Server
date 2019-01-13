using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Client
{
    public partial class ClientForm4x4 : Form
    {
        //Lưu mảng ban đầu nhận từ server để check valid
        //Nếu người chơi mà chơi đúng thì lưu luôn vào để check valid
        //Chưa xử lý - để làm sau
        int[,] twoD = new int[4, 4];

        //Lưu mảng của người chơi để tiện set màu
        int[,] twoDPlayer = new int[4, 4];

        //Lưu mảng của đối phương để tiện set màu
        int[,] twoDFromOpponent = new int[4, 4];

        //Lưu mảng ban đầu của server
        int[,] twoDFromServer = new int[4, 4];

        //Lưu mảng khi giá trị của cell thay đổi
        int[,] twoDPlayerTemp = new int[4, 4];

        //Các biến dùng cho tính giờ
        bool status = false;
        Thread timeThread;
        int count = 0;

        //Khởi tạo giao diện để nhìn khỏi tục
        private void KhoiTaoGiaoDien()
        {
            for (int r = 0; r < 4; r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);
                this.dataGridView1.Rows.Add(row);
            }
            btnSubmit.Enabled = false;
            SuaChieuCaoCuaRow();
            dataGridView1.Paint += DataGridView1_Paint;
        }

        public ClientForm4x4()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
            KhoiTaoGiaoDien();
        }

        //Sửa chiều cao của dòng để fill Datagridview
        private void SuaChieuCaoCuaRow()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = dataGridView1.ClientRectangle.Height / dataGridView1.Rows.Count;
            }
        }

        //Giá trị ban đầu và giá trị của 2 người chơi được lưu riêng vào 2 mảng khác nhau
        private void GanGiaTriVaoBang()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            for (int r = 0; r < 4; r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);

                for (int c = 0; c < 4; c++)
                {
                    //Nếu như đó là giá trị ban đầu của Server thì set màu đỏ
                    if (twoDFromServer[r, c] != 0)
                    {
                        row.Cells[c].Value = twoDFromServer[r, c];
                        row.Cells[c].ReadOnly = true;
                        row.Cells[c].Style.ForeColor = Color.Red;

                    }
                    //Nếu như đó là giá trị của hai người chơi
                    else
                    {
                        //Nếu như đó là giá trị của người chơi thì set màu vàng
                        if (twoDPlayer[r, c] != 0 && twoDFromOpponent[r, c] == 0)
                        {
                            row.Cells[c].Value = twoDPlayer[r, c];
                            row.Cells[c].ReadOnly = true;
                            row.Cells[c].Style.ForeColor = Color.ForestGreen;
                        }
                        //Nếu như đó là giá trị của đối phương thì set màu xanh lá
                        else if (twoDPlayer[r, c] == 0 && twoDFromOpponent[r, c] != 0)
                        {
                            row.Cells[c].Value = twoDFromOpponent[r, c];
                            row.Cells[c].ReadOnly = true;
                            row.Cells[c].Style.ForeColor = Color.DodgerBlue;
                        }

                    }
                }
                this.dataGridView1.Rows.Add(row);
            }

            SuaChieuCaoCuaRow();
            dataGridView1.Refresh();
        }

        //Giá trị ban đầu nhận được từ Server khi NewGame và lưu vào mảng tương ứng
        private void LayGiaTriBanDauTuServer(string s)
        {
            int count = 0;
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    twoDFromServer[r, c] = (int)Char.GetNumericValue(s[count]);
                    twoD[r, c] = (int)Char.GetNumericValue(s[count]);
                    count++;
                }
        }

        //Giá trị của đối phương nhận được tù Server và lưu vào mảng tương ứng
        private void LayGiaTriCuaDoiPhuongTuServer(string s)
        {
            int count = 0;
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    twoDFromOpponent[r, c] = (int)Char.GetNumericValue(s[count]);
                    twoD[r, c] = (int)Char.GetNumericValue(s[count]);
                    count++;
                }
        }

        /// <summary>
        /// kẻ ô 3x3 cho DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.Gold, 4), 157, 0, 157, 314);
            e.Graphics.DrawLine(new Pen(Color.Gold, 4), 0, 159, 319, 159);
        }

        //Kiểm tra giá trị điền vào có hợp lệ hay không
        bool CheckValid(int x, int y, int k)
        {
            for (int i = 0; i < 4; i++)
            {

                if (twoD[x, i] == k && i != y) return false;

            }
            for (int i = 0; i < 4; i++)
            {
                if (twoD[i, y] == k && (i != x)) return false;
            }
            int a = x / 2, b = y / 2;
            for (int i = 2 * a; i < 2 * a + 2; i++)
            {
                for (int j = 2 * b; j < 2 * b + 2; j++)
                {
                    if (twoD[i, j] == k && (i != x) && (j != y)) return false;
                }
            }
            return true;
        }

        private static int layIDTuTextbox()
        {
            int ID = RoomPlayer.passingID;
            return ID;
        }

        private static int layRoomIDTuRoomPlayer()
        {
            int ID = RoomPlayer.passingRoomID;
            return ID;
        }

        static int ID = layIDTuTextbox();
        static int roomID = layRoomIDTuRoomPlayer();

        IPEndPoint IP;
        //Socket client;

        Player player = new Player(-1, null);

        void ReconnectServer1()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            player.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                player.socket.Connect(IP);
                player.iD = ID;
                Send("12" + roomID + ID);
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

        void Reconnect()
        {
            try
            {
                IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10999);
                player.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                player.socket.Connect(IP);

                Thread listen = new Thread(Receive);
                listen.IsBackground = true;
                listen.Start();

                Send("Client from Server 1! ID: " + ID);
                Thread.Sleep(1000);
                Send("5x3x" + roomID + ID);
            }
            catch
            {
                MessageBox.Show("Tất cả Server đều bị sập!\nChờ sửa xong Server rồi chơi tiếp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Close();
            }
        }
        /// <summary>
        /// Kết nối tới server
        /// </summary>
        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            player.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                player.socket.Connect(IP);
                player.iD = ID;
                Send("2x" + roomID + ID);
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

        /// <summary>
        /// đóng kết nối hiện thời
        /// </summary>
        void CloseConect()
        {
            try
            {
                player.socket.Shutdown(SocketShutdown.Both);
                player.socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //bool server1isAlive = true;

        /// <summary>
        /// gửi tin nhắn
        /// </summary>
        void Send(string s)
        {
            try
            {
                player.socket.Send(Serialize(s));
            }
            catch
            {
                MessageBox.Show("Server chính đã bị sập!\nTrạng thái của bạn đã được Backup!\nReconnect tới Server phụ để chơi tiếp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //server1isAlive = false;
            }
        }

        //Đặt lại tất cả giá trị của các mảng 2 chiều khi NewGame
        //Tránh hiển thị lại những giá trị còn sót của mình và đối phương ở màn chơi trước
        private void ResetAlltwoD()
        {
            Array.Clear(twoD, 0, 16);
            Array.Clear(twoDPlayer, 0, 16);
            Array.Clear(twoDFromOpponent, 0, 16);
            Array.Clear(twoDFromServer, 0, 16);
        }


        /// <summary>
        /// nhận tin nhắn
        /// </summary>
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] dataRcv = new byte[1024 * 5000];
                    player.socket.Receive(dataRcv);

                    string message = (string)Deserialize(dataRcv);

                    string code = message.Substring(0, 2);
                    string data = message.Substring(2);

                    switch (code)
                    {
                        case "0x": //New Game
                            BatDauTinhGio();
                            BatDauDetect();
                            ResetAlltwoD();
                            LayGiaTriBanDauTuServer(data);
                            GanGiaTriVaoBang();
                            break;
                        case "1x": //Playing Game
                            LayGiaTriCuaDoiPhuongTuServer(data);
                            GanGiaTriVaoBang();
                            status = true;
                            BatDauTinhGio();
                            detectServerThread.Abort();
                            btnSubmit.Enabled = true;
                            break;
                        case "2x": //Kiểm tra có được chơi trước hay không
                            if (data.Equals("1")) //bằng 1 thì được chơi trước
                                btnSubmit.Enabled = true;
                            else
                                btnSubmit.Enabled = false;
                            break;
                        case "3x": //Kiểm tra đã đủ hai người chơi chưa
                            {
                                //timeThread.Abort();
                                MessageBox.Show("Chờ người chơi còn lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case "4x": //Server thông báo kết quả cho người chơi, 1 là win, 2 là lose, 3 là hòa
                            if (data.Equals("1"))
                            {
                                detectServerThread.Abort();
                                timeThread.Abort();
                                btnSubmit.Enabled = false;
                                MessageBox.Show("Win!", "Kết Quả", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else if (data.Equals("2"))
                            {
                                detectServerThread.Abort();
                                timeThread.Abort();
                                btnSubmit.Enabled = false;
                                MessageBox.Show("Lose!", "Kết Quả", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                detectServerThread.Abort();
                                timeThread.Abort();
                                btnSubmit.Enabled = false;
                                MessageBox.Show("Draw!", "Kết Quả", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            break;
                        case "5x":
                            MessageBox.Show("Đã đồng bộ!", "Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            break;
                        case "11":
                            ReconnectServer1();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                CloseConect();
            }
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

        //Đếm ngược
        private void DemNguoc()
        {
            int seconds = 15;
            do
            {
                Thread.Sleep(950);
                seconds--;
                textBox1.Text = "Time: 00:" + seconds;
            } while (seconds > 0 && status);
            btnSubmit.Enabled = true;
            btnSubmit.PerformClick();
        }
        

        //Nút Quit, đóng form và tiếp đó là ngắt kết nối
        private void btnQuit_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        //Chuyển mảng sang chuỗi
        private string ChuyenMangSangChuoi()
        {
            string temp = "";

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    temp += twoDPlayer[r, c].ToString();
            return temp;
        }

        Thread detectServerThread;
        int DetectCount = 0;

        //Kiểm tra Server có bị sập không
        private void DetectServer()
        {
            int seconds = 17;
            do
            {
                Thread.Sleep(950);
                seconds--;
            } while (seconds > 0 && status);

            btnSubmit.Enabled = true;
        }

        //Bắt đầu Detect
        private void BatDauDetect()
        {
            if (DetectCount == 0)
                DetectCount++;
            else
                detectServerThread.Abort();
            status = true;
            detectServerThread = new Thread(DetectServer);
            //detectServerThread.IsBackground = true;
            detectServerThread.Start();
        }


        //Nút submit, giá trị trong mảng của người chơi sẽ được gửi cho đối phương
        private void btnSubmit_Click_1(object sender, EventArgs e)
        {
            try
            {
                detectServerThread.Abort();
            }
            catch
            {
                MessageBox.Show("Server chính đã bị sập!\nTrạng thái của bạn đã được Backup!\nReconnect tới Server phụ để chơi tiếp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //detectServerThread.Abort();
            twoDPlayer = twoDPlayerTemp;
            string mapdangchoi = "4";
            string message = "1x" + roomID + player.iD + mapdangchoi + ChuyenMangSangChuoi();
            Send(message);
            btnSubmit.Enabled = false;
            timeThread.Abort();
            textBox1.Text = "Time: 00:0";
            BatDauDetect();
        }

        //Kiểm tra combobox độ khó và trả về độ khó tương ứng để xử lý tiếp
        private int CheckCombobox()
        {
            string level = comboBox1.SelectedItem.ToString();
            if (level.Equals("Easy"))
            {
                return 3;
            }
            else if (level.Equals("Medium"))
            {
                return 5;
            }
            else if (level.Equals("Hard"))
            {
                return 7;
            }
            return 0;
        }

       

        //Tính giờ và hiển thị trên giao diện người chơi nếu cần có thể dùng làm gì đó
        private void BatDauTinhGio()
        {
            textBox1.Text = "Time: 00:15";
            if (count == 0)
                count++;
            else
                timeThread.Abort();
            status = true;
            timeThread = new Thread(DemNguoc);
            //detectServerThread.IsBackground = true;
            timeThread.Start();
        }

        //Nút NewGame
        private void newGameBtn_Click(object sender, EventArgs e)
        {
            //BatDauTinhGio();
            string s = "0x" + RoomPlayer.passingRoomID + 4 + CheckCombobox();
            Send(s);
        }

        //Khi đóng Form thì đóng kết nối
        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseConect();
        }

        //Khi giá trị của người chơi nhập vào thay đổi 1 cách hợp lệ thì sẽ lưu mảng tương ứng
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int r = dataGridView1.CurrentCell.RowIndex;
                int c = dataGridView1.CurrentCell.ColumnIndex;

                try
                {
                    int temp = Convert.ToInt32(dataGridView1.CurrentCell.Value);
                    if ((temp < 1) || temp > 4)
                    {
                        MessageBox.Show("Số nhập vào phải từ 1 đến 4!\nGiá trị này sẽ không được submit!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //twoD[r, c] = temp;
                        twoDPlayerTemp[r, c] = temp;
                    }
                }
                catch
                {
                    MessageBox.Show("Nhập số nguyên từ 1 đến 4!\nGiá trị này sẽ không được submit!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                //Nếu không có cái này sẽ lỗi bởi vì chưa có màn chơi sẽ không có CurrentCell
            }
        }

        private void reconnectBtn_Click(object sender, EventArgs e)
        {
            Reconnect();
            btnSubmit.Enabled = true;
        }
    }
}

