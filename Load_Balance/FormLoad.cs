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

namespace Load_Balance
{
    public partial class FormLoad : Form
    {
        IPEndPoint iP;
        Socket socketLoad, socketServer1, socketServer2;
        List<PlayerLoad> playerLoads;

        public FormLoad()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            connectListen();
            playerLoads = new List<PlayerLoad>();

            Thread connect1 = new Thread(connectServer1);
            connect1.IsBackground = true;
            connect1.Start();

            //Thread connect2 = new Thread(connectServer2);
            //connect2.IsBackground = true;
            //connect2.Start();
        }

        private void connectListen()
        {
            iP = new IPEndPoint(IPAddress.Any, 10002);
            socketLoad = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketLoad.Bind(iP);

            Thread listen = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            socketLoad.Listen(100);
                            Thread thread = new Thread(receive);
                            thread.IsBackground = true;
                            thread.Start(socketLoad.Accept());
                        }
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                });
            listen.IsBackground = true;
            listen.Start();
        }

        private void connectServer1()
        {
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            socketServer1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                socketServer1.Connect(iPEnd);
                socketServer1.Send(Serialize("test"));
                MessageBox.Show("Connect to Server 1 successful!");
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void connectServer2()
        {
            IPEndPoint iP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10999);
            socketServer2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                socketServer2.Connect(iP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void receive(object obj)
        {
            Socket socket = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] dataReceived = new byte[1024 * 5000];
                    socket.Receive(dataReceived);
                    socketServer1.Send(Serialize("test"));
                    socketServer1.Send(dataReceived);
                    string data = (string) Deserialize(dataReceived);
                    listView1.Items.Add(new ListViewItem() { Text = data });
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void closeConnect()
        {
            socketLoad.Close();
        }

        private void FormLoad_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeConnect();
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
    }
    
}
