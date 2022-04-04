using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Timers;









namespace WindowsFormsApp1
{

    



    public partial class Form2 : Form
    {
        
        public int sendbutton = 0;
        public int checkclient = 0;
        public int connectint = 0;
        public int scanconnect = 0;
        public int connectsucess = 0;
        public int bantime = 1;
        public int clientnumber = 0;
        public string[] clientmsg = new string[21];



        public int sendrefresh = 0;
        public string selectid = "";
        public string selectname = "";

        Thread threadWatch = null; //负责监听客户端的线程
        Socket socketWatch = null; //负责监听客户端的套接字










        public Form2()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            insertComboItem1();
            insertComboItem2();
            insertComboItem3();
            insertComboItem4();
            insertComboItem5();
            
            insertComboItem7();
            textBox1.KeyUp += new KeyEventHandler(textBox1_KeyUp);
            textBox3.KeyUp += new KeyEventHandler(textBox3_KeyUp);
            textBox4.KeyUp += new KeyEventHandler(textBox4_KeyUp);
            textBox6.KeyUp += new KeyEventHandler(textBox6_KeyUp);
            textBox7.KeyUp += new KeyEventHandler(textBox7_KeyUp);



            /*
            using (client)
            {
                //连接完服务器后便在客户端和服务端之间产生一个流的通道
                NetworkStream ns = client.GetStream();
                byte[] m = System.Text.Encoding.UTF8.GetBytes(rconpassword);
                ByteBuffer buffer = new ByteBuffer();
                buffer.PopUInt((uint)m.Length + 10);
                buffer.PushInt(100);
                buffer.PushInt(3);
                buffer.PushByteArray(m);
                buffer.PushByte((byte)0);
                buffer.PushByte((byte)0);
                byte[] mm = buffer.ToByteArray();
                ns.Write(mm,0,buffer.Length);
                
                byte[] buffer2 = new byte[client.ReceiveBufferSize];
                //读取进来的流
                int bytesRead = ns.Read(buffer2, 0, client.ReceiveBufferSize);


                //接收的数据转换成字符串
                string dataReceived = Encoding.ASCII.GetString(buffer2, 0, bytesRead);
                textBox1.AppendText(System.Environment.NewLine);
                textBox1.AppendText("Received :" + dataReceived);
                
            }

            */












             threadWatch = new Thread(WatchConnecting);            //将窗体线程设置为与后台同步
             threadWatch.IsBackground = true;             //启动线程
             threadWatch.Start();











        }
        private readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        public void Connectrcon(IPEndPoint remoteEndPoint, int timeoutMSec)
        {
            TimeoutObject.Reset();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(remoteEndPoint, CallBackMethod, socket);
            //阻塞当前线程           
            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                //MessageBox.Show("网络正常");

            }
            else
            {
                //MessageBox.Show("连接超时");



            }
        }


        private void CallBackMethod(IAsyncResult asyncresult)
        {
            //使阻塞的线程继续        
            TimeoutObject.Set();
        }


        private void WatchConnecting()
        {



            Font font1 = new Font("宋体", 12);
            Font font2 = new Font("微软雅黑", 12);
            

            TimeoutObject.Reset();
            IPAddress ip = IPAddress.Parse(Form1.ipstr);
            TcpClient client = new TcpClient();
            //client.Connect(new IPEndPoint(ip, Form1.portnum));

            var result = client.BeginConnect(ip, Form1.portnum, CallBackMethod, client);
            int timeoutMSec = 1;
            if (result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutMSec)))
            {
                //MessageBox.Show("网络正常");
            }
            else
            {
                //MessageBox.Show("连接超时");
                connectsucess = 0;
                MessageBox.Show("Time Out");
                this.Close();
                threadWatch.Abort();


            }




            string rconpassword = Form1.passwordstr;
            // richTextBox1.AppendText("ip :" + ip + " port:" + Form1.portnum + " password:" + rconpassword , Color.Green ,font2);
            using (client)
            {
                //连接完服务器后便在客户端和服务端之间产生一个流的通道
                NetworkStream ns = client.GetStream();
                byte[] m = System.Text.Encoding.UTF8.GetBytes(rconpassword);
                PacketWriter pw = new PacketWriter();
                pw.Write(m.Length + 10);
                pw.Write(100);
                pw.Write(3);
                pw.Write(m);
                pw.Write((byte)0);
                pw.Write((byte)0);
                byte[] mm = pw.GetBytes();
                ns.Write(mm, 0, mm.Length);


                PacketReader pr = new PacketReader(ns);



                while (true)
                {
                    if (sendbutton == 1)
                    {
                        ns.Write(sendmessage(textBox2.Text), 0, sendmessage(textBox2.Text).Length);
                        richTextBox1.AppendText(DateTime.Now.ToString("T") + " ", Color.Black, font2);
                        richTextBox1.AppendText(textBox2.Text, Color.Orange, font2);
                        textBox2.Text = "";
                        richTextBox1.AppendText(System.Environment.NewLine);

                        sendbutton = 0;
                    }

                    if (checkclient == 1)
                    {
                        string displaycommand = "display_all_clients";
                        ns.Write(sendmessage(displaycommand), 0, sendmessage(displaycommand).Length);
                        checkclient = 0;
                    }

                    if (client.Available > 0)
                    {
                        /* string recmsgg = pr.ReadMessage();

                        textBox1.AppendText(System.Environment.NewLine);
                        textBox1.AppendText("Received:" + recmsgg);
                        
                        if(String.Compare(recmsgg, "d") == 0)
                        { 
                                textBox1.AppendText(System.Environment.NewLine);
                                textBox1.AppendText("Connection successful!");
                                connectsucess = 1;
                        }

                        if (recmsgg.IndexOf("*", StringComparison.OrdinalIgnoreCase) >= 0)                           
                        {
                            textBox1.AppendText(System.Environment.NewLine);
                            textBox1.AppendText("Received successful");         // 接受返回的信息
                            
                            
                            

                        }

                        
                        
                        */
                        //richTextBox1.AppendText(System.Environment.NewLine);
                        byte[] ccc = pr.ReadMe();
                        /*
                        for (int i = 0; i < ccc.Length; i++)
                        {
                            textBox1.AppendText(ccc[i].ToString("X2"));                            
                        }
                        */
                        if (ccc[0].ToString("X2") == "64" && connectsucess == 0)
                        {
                            richTextBox1.AppendText(DateTime.Now.ToString("T") + " ", Color.Black, font2);
                            richTextBox1.AppendText("Connection successful!", Color.Red, font2);
                            connectsucess = 1;
                            richTextBox1.AppendText(System.Environment.NewLine);

                        }
                        else if (ccc[0].ToString("X2") == "64" && connectsucess == 1)
                        {

                        }
                        else if (ccc[0].ToString("X2") == "2A")
                        {



                            int cccmsglong = ccc.Length - 10;
                            byte[] cccmsg = new byte[cccmsglong];

                            for (int j = 0; j < ccc.Length - 10; j++)
                            {
                                cccmsg[j] = ccc[j + 8];
                            }


                            string dataReceived = Encoding.UTF8.GetString(cccmsg);
                            /*
                            for (int i = 0; i < cccmsg.Length; i++)
                            {
                                textBox1.AppendText(cccmsg[i].ToString("X2"));
                            }
                            */
                            string cccmsgt = System.Text.Encoding.UTF8.GetString(cccmsg);
                            if (cccmsgt.Contains("Client List :") == false)
                            {
                                richTextBox1.AppendText(DateTime.Now.ToString("T") + " ", Color.Black, font2);
                                richTextBox1.AppendText(cccmsgt, Color.Green, font2);
                                richTextBox1.AppendText(System.Environment.NewLine);
                            }

                            else
                            {
                                string[] lines = cccmsgt.Split(
                                new[] { "\r\n", "\r", "\n" },
                                StringSplitOptions.None
                                );
                                //MessageBox.Show("lineLength" + lines.Length);

                                clientnumber = lines.Length - 2;




                                for (int i = 1; i <= clientnumber; ++i)
                                {

                                    clientmsg[i] = lines[i];

                                }
                                for (int j = clientnumber + 1; j <= 20; ++j)
                                {

                                    clientmsg[j] = "";

                                }



                                insertitemupdate();




                            }


                        }
                        else
                        {
                            richTextBox1.AppendText(System.Environment.NewLine);
                            richTextBox1.AppendText(DateTime.Now.ToString("T") + " ", Color.Black, font2);
                            richTextBox1.AppendText("未知错误", Color.Red, font2);
                        }

                        scanconnect++;





                        Thread.Sleep(100);


                    }
                    else
                    {

                        scanconnect++;
                        Thread.Sleep(100);
                    }

                    if (scanconnect >= 10 && connectsucess == 0)
                    {
                        connectsucess = 0;
                        MessageBox.Show("Password Error");
                        ns.Close();
                        this.Close();
                        threadWatch.Abort();

                        break;
                    }













                }



                /*
                byte[] buffer2 = new byte[client.ReceiveBufferSize];
                //读取进来的流
                int bytesRead = ns.Read(buffer2, 0, client.ReceiveBufferSize);


                //接收的数据转换成字符串
                string dataReceived = Encoding.ASCII.GetString(buffer2, 0, bytesRead);
                textBox1.AppendText(System.Environment.NewLine);
                textBox1.AppendText("Received :" + dataReceived);
                */
            }

        }











        public byte[] sendmessage(string cmd)
        {

            byte[] m = System.Text.Encoding.UTF8.GetBytes(cmd);
            PacketWriter msg = new PacketWriter();
            msg.Write(m.Length + 10);
            msg.Write(42);
            msg.Write(2);
            msg.Write(m);
            msg.Write((byte)0);
            msg.Write((byte)0);
            byte[] mmm = msg.GetBytes();
            return mmm;

        }









        private void Form2_Load(object sender, EventArgs e)
        {

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            //获取当前选中的值
            string itemText = cb.SelectedItem as string;

            switch (comboBox1.SelectedIndex) //获取选择的内容
            {

                case 0:
                    break;

                case 1:

                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar VictoryCond 2";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar CombatRule 2";
                        sendbutton = 1;

                    }

                    );



                    break;

                case 2:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar VictoryCond 3";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar CombatRule 2";
                        sendbutton = 1;

                    }

                    );

                    break;

                case 3:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar VictoryCond 5";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar CombatRule 2";
                        sendbutton = 1;

                    }

                    );
                    break;

                case 4:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar VictoryCond 2";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar CombatRule 1";
                        sendbutton = 1;

                    }

                    );
                    break;

                case 5:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar VictoryCond 3";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar CombatRule 1";
                        sendbutton = 1;

                    }

                    );
                    break;

            }




        }
        private void insertComboItem1()
        {
            comboBox1.Items.Add("");
            comboBox1.Items.Add("夺旗模式遭遇战");
            comboBox1.Items.Add("夺旗模式近战");
            comboBox1.Items.Add("夺旗模式突破");
            comboBox1.Items.Add("摧毁模式遭遇战");
            comboBox1.Items.Add("摧毁模式近战");
            comboBox1.SelectedIndex = 0;
        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            //获取当前选中的值
            string itemText = cb.SelectedItem as string;

            switch (comboBox2.SelectedIndex) //获取选择的内容
            {

                case 0:
                    break;

                case 1:

                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar GameType 0";
                        sendbutton = 1;

                    }

                    );


                    break;

                case 2:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar GameType 1";
                        sendbutton = 1;

                    }

                    );



                    break;

                case 3:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar GameType 2";
                        sendbutton = 1;

                    }

                    );


                    break;


            }
        }



        private void insertComboItem2()
        {
            comboBox2.Items.Add("");
            comboBox2.Items.Add("盟军vs轴心国");
            comboBox2.Items.Add("盟军vs盟军");
            comboBox2.Items.Add("盟军vs轴心国");

            comboBox2.SelectedIndex = 0;
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            //获取当前选中的值
            string itemText = cb.SelectedItem as string;
            switch (comboBox3.SelectedIndex) //获取选择的内容
            {

                case 0:
                    break;

                case 1:

                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 2";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 2";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 1";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 1";
                        sendbutton = 1;

                    }

                    );


                    break;

                case 2:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 4";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 4";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 2";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 2";
                        sendbutton = 1;

                    }

                    );


                    break;

                case 3:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 6";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 6";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 3";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 3";
                        sendbutton = 1;

                    }

                    );


                    break;

                case 4:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 8";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 8";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 4";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 4";
                        sendbutton = 1;

                    }

                    );


                    break;



                case 5:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 10";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 10";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 5";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 5";
                        sendbutton = 1;

                    }

                    );


                    break;

                case 6:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 12";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 12";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 6";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 6";
                        sendbutton = 1;

                    }

                    );


                    break;
                case 7:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 14";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 14";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 7";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 7";
                        sendbutton = 1;

                    }

                    );


                    break;
                case 8:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 16";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 16";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 8";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 8";
                        sendbutton = 1;

                    }

                    );


                    break;
                case 9:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 18";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 18";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 9";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 9";
                        sendbutton = 1;

                    }

                    );


                    break;
                case 10:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMaxPlayer 20";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(210).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar NbMinPlayer 20";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(410).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar DeltaMaxTeamSize 10";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(660).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar MaxTeamSize 10";
                        sendbutton = 1;

                    }

                    );


                    break;


            }



        }






        private void insertComboItem3()
        {
            comboBox3.Items.Add("");
            comboBox3.Items.Add("1v1");
            comboBox3.Items.Add("2v2");
            comboBox3.Items.Add("3v3");
            comboBox3.Items.Add("4v4");
            comboBox3.Items.Add("5v5");
            comboBox3.Items.Add("6v6");
            comboBox3.Items.Add("7v7");
            comboBox3.Items.Add("8v8");
            comboBox3.Items.Add("9v9");
            comboBox3.Items.Add("10v10");

            comboBox3.SelectedIndex = 0;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            //获取当前选中的值
            string itemText = cb.SelectedItem as string;

            switch (comboBox4.SelectedIndex) //获取选择的内容
            {

                case 0:
                    break;

                case 1:

                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar InitMoney 750";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar IncomeRate 3";
                        sendbutton = 1;

                    }

                    );



                    break;

                case 2:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar InitMoney 500";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar IncomeRate 2";
                        sendbutton = 1;

                    }

                    );

                    break;

                case 3:
                    Task.Delay(10).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar InitMoney 250";
                        sendbutton = 1;

                    }

                    );

                    Task.Delay(220).ContinueWith(_ =>
                    {

                        textBox2.Text = "setsvar IncomeRate 1";
                        sendbutton = 1;

                    }

                    );
                    break;



            }




        }












        private void insertComboItem4()
        {
            comboBox4.Items.Add("");
            comboBox4.Items.Add("1v1-4v4 standard rule");
            comboBox4.Items.Add("10v10");
            comboBox4.Items.Add("10v10 Tactical");


            comboBox4.SelectedIndex = 0;
        }

        private void insertComboItem5()
        {
            comboBox5.Items.Add("永久");
            comboBox5.Items.Add("1小时");
            comboBox5.Items.Add("6小时");
            comboBox5.Items.Add("1天");
            comboBox5.Items.Add("3天");
            comboBox5.Items.Add("1周");
            comboBox5.Items.Add("1月");
            comboBox5.SelectedIndex = 1;
        }
        
        private void insertComboItem7()
        {
            comboBox7.Items.Add("English");
            comboBox7.Items.Add("中文");
            comboBox7.SelectedIndex = 0;
        }

        private void Send_Click(object sender, EventArgs e)
        {
            sendbutton = 1;

        }

        private void Send_Click2(object sender, EventArgs e)
        {
            Task.Delay(10).ContinueWith(_ =>
            {

                textBox2.Text = "launch";
                sendbutton = 1;

            }
            );
        }

        private void Send_Click3(object sender, EventArgs e)
        {
            Task.Delay(10).ContinueWith(_ =>
            {

                textBox2.Text = "cancel_launch";
                sendbutton = 1;

            }
            );
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string nametext = "setsvar ServerName " + textBox1.Text;
                        textBox2.Text = nametext;
                        sendbutton = 1;

                    }

                    );
                }
            }
        }
        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                if (textBox3.Text != "")
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string passwdtext = "setsvar Password " + textBox3.Text;
                        textBox2.Text = passwdtext;
                        sendbutton = 1;

                    }

                    );
                }
                else
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string passwdtext = "setsvar Password";
                        textBox2.Text = passwdtext;
                        sendbutton = 1;
                    }

                    );

                }
            }
        }
        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                if (textBox5.Text != "")
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string passwdtext = "setpvar " + textBox5.Text + " PlayerName " + textBox4.Text;
                        textBox2.Text = passwdtext;
                        sendbutton = 1;
                    }

                    );
                }

            }
        }

        private void textBox6_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                if (textBox6.Text != "" && textBox5.Text != "")
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string passwdtext = "setpvar " + textBox5.Text + " PlayerDeckContent " + textBox6.Text;
                        textBox2.Text = passwdtext;
                        sendbutton = 1;

                    }

                    );
                }
                else
                {
                    MessageBox.Show("请先选择玩家且输入卡组代码");

                }
            }
        }

        private void textBox7_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                if (textBox7.Text != "")
                {
                    Task.Delay(10).ContinueWith(_ =>
                    {
                        string passwdtext = "unban " + textBox7.Text;
                        textBox2.Text = passwdtext;
                        sendbutton = 1;

                    }

                    );
                }

            }
        }

        private void insertitemupdate()
        {




            this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度


            listView1.Items.Clear();

            for (int i = 1; i <= clientnumber; ++i)
            {

                int splixindex = clientmsg[i].IndexOf(' ');
                string indexresult = clientmsg[i].Substring(0, splixindex);

                string nameresult = clientmsg[i].Substring(splixindex + 1, clientmsg[i].Length - splixindex - 1);



                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Clear();
                lvi.SubItems[0].Text = indexresult;
                lvi.SubItems.Add(nameresult);

                this.listView1.Items.Add(lvi);

            }
            /*
                for (int j = clientnumber + 1; j <= 20; ++j)
                {
                    ListViewItem item = listView1.SelectedItems[j];
                    listView1.Items.Remove(item);
                }
            */




            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。



        }





        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void Form2_Shown(int timeout, string password)
        {


        }




        private void label1_Click(object sender, EventArgs e)
        {

        }

        System.Timers.Timer timer = new System.Timers.Timer();



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer.Enabled = true;
            timer.Interval = 200;


            if (checkBox1.Checked)
            {
                timer.Start();
                timer.Elapsed += new System.Timers.ElapsedEventHandler(SendScan);

                button3.Enabled = false;






            }
            else
            {

                timer.Stop();

                button3.Enabled = true;


            }

        }

        private void SendScan(object sender, EventArgs e)
        {
            checkclient = 1;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string Eugnetid = listView1.FocusedItem.SubItems[0].Text;
            string Playername = listView1.FocusedItem.SubItems[1].Text;
            textBox5.Text = Eugnetid;
            textBox4.Text = Playername;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                Task.Delay(10).ContinueWith(_ =>
                {

                    textBox2.Text = "setpvar " + textBox5.Text + " PlayerAlliance 1";
                    sendbutton = 1;

                }

                );
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                Task.Delay(10).ContinueWith(_ =>
                {

                    textBox2.Text = "setpvar " + textBox5.Text + " PlayerAlliance 0";
                    sendbutton = 1;

                }

                );
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                Task.Delay(10).ContinueWith(_ =>
                {

                    textBox2.Text = "kick " + textBox5.Text;
                    sendbutton = 1;

                }

                );
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox7.Text != "")
            {
                Task.Delay(10).ContinueWith(_ =>
                {

                    textBox2.Text = "unban " + textBox7.Text;
                    sendbutton = 1;

                }

                );
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox5.SelectedIndex) //获取选择的内容
            {

                case 0:
                    bantime = 0;
                    break;
                case 1:
                    bantime = 1;
                    break;

                case 2:
                    bantime = 6;
                    break;
                case 3:
                    bantime = 24;
                    break;
                case 4:
                    bantime = 72;
                    break;
                case 5:
                    bantime = 168;
                    break;
                case 6:
                    bantime = 720;
                    break;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                Task.Delay(10).ContinueWith(_ =>
                {

                    textBox2.Text = "ban " + textBox5.Text + " " + bantime;
                    sendbutton = 1;

                }

                );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            checkclient = 1;




        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

            
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            //   MessageBox.Show("jin zhege fnagfa ");
            //switch (comboBox5.SelectedIndex)
            //{
            //    case 0:
            //        languageEng();
            //        Global.language = Language.en_US;
            //        break;
            //    case 1:
            //        languageChi();
            //        Global.language = Language.zh_CN;
            //        break;
            //}

            if (((System.Windows.Forms.ComboBox)sender).SelectedItem.ToString() == "English")
            {
                Global.language = Language.en_US;
            }
            else
            {
                Global.language = Language.zh_CN;
            }
            LoadInfo();
        }


        public void LoadInfo()
        {
            Font font3 = new Font("微软雅黑", (float)21.75);
            Font font4 = new Font("微软雅黑", 12);
            //this.label12.Text=
            switch (Global.language)
            {
                case Language.zh_CN:
                    /*      title        */
                    this.Text = "钢铁之师2服务器管理工具";
                    /*      Page 1        */
                    //this.label13.Text = "语言";
                    this.tabPage1.Text = "常用设置";
                    this.button5.Text = "发送";
                    this.button1.Text = "启动游戏";
                    this.button2.Text = "停止启动";
                    this.label6.Text = "服务器名";
                    this.label6.Font = font3;
                    this.label5.Text = "进服密码";
                    this.label5.Font = font3;
                    this.label1.Text = "部署模式";
                    this.label1.Font = font3;
                    this.label2.Text = "阵营对手";
                    this.label2.Font = font3;
                    this.label3.Text = "对战人数";
                    this.label3.Font = font3;
                    this.label4.Text = "资源设定";
                    this.label4.Font = font3;

                    this.comboBox1.Items[1] = "夺旗模式遭遇战";
                    this.comboBox1.Items[2] = "夺旗模式近战";
                    this.comboBox1.Items[3] = "夺旗模式突破";
                    this.comboBox1.Items[4] = "摧毁模式遭遇战";
                    this.comboBox1.Items[5] = "摧毁模式近战";

                    this.comboBox2.Items[1] = "盟军vs轴心国";
                    this.comboBox2.Items[2] = "盟军vs盟军";
                    this.comboBox2.Items[3] = "盟军vs轴心国";



                    /*      Page 2        */
                    this.tabPage2.Text = "详细设置";
                    /*      Page 3        */
                    this.tabPage3.Text = "地图设置";
                    /*      Page 4        */
                    this.tabPage4.Text = "人员管理";
                    this.button3.Text = "刷 新";
                    this.button4.Text = "移至蓝队";
                    this.button6.Text = "移至红队";
                    this.button7.Text = "踢出玩家";
                    this.button8.Text = "封禁玩家";
                    this.button9.Text = "解封玩家";
                    this.label8.Text = "玩家索引";
                    this.label8.Font = font3;
                    this.label7.Text = "玩家名字";
                    this.label7.Font = font3;
                    this.label9.Text = "卡组代码";
                    this.label9.Font = font3;
                    this.label10.Text = "封禁时间";
                    this.label10.Font = font3;
                    this.label11.Text = "解封索引";
                    this.label11.Font = font3;
                    this.checkBox1.Text = "每0.2秒一次刷新";
                    this.checkBox1.Font = new Font("微软雅黑", 15);

                    this.comboBox5.Items[0] = "永久";
                    this.comboBox5.Items[1] = "1小时";
                    this.comboBox5.Items[2] = "6小时";
                    this.comboBox5.Items[3] = "1天";
                    this.comboBox5.Items[4] = "3天";
                    this.comboBox5.Items[5] = "1周";
                    this.comboBox5.Items[6] = "1月";

                    /*      Page 5        */
                    this.tabPage5.Text = "关于";
                    break;
                case Language.en_US:
                    /*      title        */
                    this.Text = "Steel Division 2 RCON Tool";
                    /*      Page 1        */
                    //this.label13.Text = "Language";
                    this.tabPage1.Text = "common";
                    this.button5.Text = "Send";
                    this.button1.Text = "Launch Game";
                    this.button2.Text = "Stop Launch";
                    this.label6.Text = "Server Name";
                    this.label6.Font = font4;
                    this.label5.Text = "Server Password";
                    this.label5.Font = font4;
                    this.label1.Text = "Battle Mode";
                    this.label1.Font = font4;
                    this.label2.Text = "Faction Setting";
                    this.label2.Font = font4;
                    this.label3.Text = "Team Presets";
                    this.label3.Font = font4;
                    this.label4.Text = "Resource Setting";
                    this.label4.Font = font4;

                    this.comboBox1.Items[1] = "Conquest";
                    this.comboBox1.Items[2] = "Breakthrough";
                    this.comboBox1.Items[3] = "Close Quarter";
                    this.comboBox1.Items[4] = "Destruction";
                    this.comboBox1.Items[5] = "Dest - Close Quarter";

                    this.comboBox2.Items[1] = "ALLIES vs AXIS";
                    this.comboBox2.Items[2] = "ALLIES vs ALLIES";
                    this.comboBox2.Items[3] = "AXIS vs AXIS";

                    /*      Page 2        */
                    this.tabPage2.Text = "detailed";
                    /*      Page 3        */
                    this.tabPage3.Text = " map";
                    /*      Page 4        */
                    this.tabPage4.Text = "player";
                    this.button3.Text = "Refresh";
                    this.button4.Text = "Move to Blue";
                    this.button6.Text = "Move to Red";
                    this.button7.Text = "Kick";
                    this.button8.Text = "Ban";
                    this.button9.Text = "Unban";
                    this.label8.Text = "Player Index";
                    this.label8.Font = font4;
                    this.label7.Text = "Player Name";
                    this.label7.Font = font4;
                    this.label9.Text = "Card Code";
                    this.label9.Font = font4;
                    this.label10.Text = "Ban Time";
                    this.label10.Font = font4;
                    this.label11.Text = "Unban Index";
                    this.label11.Font = font4;
                    this.checkBox1.Text = "0.2 seconds at a time refresh";
                    this.checkBox1.Font = new Font("微软雅黑", 9 );

                    this.comboBox5.Items[0] = "Permanently";
                    this.comboBox5.Items[1] = "1 hour";
                    this.comboBox5.Items[2] = "6 hours";
                    this.comboBox5.Items[3] = "1 day";
                    this.comboBox5.Items[4] = "3 days";
                    this.comboBox5.Items[5] = "1 week";
                    this.comboBox5.Items[6] = "1 month";

                    /*      Page 5        */
                    this.tabPage5.Text = "about";

                    break;
                default:
                    break;
            }

        }

        private void languageEng()
        {
            tabControl1.TabPages[0].Text = "Common Setting";
            tabControl1.Refresh();
        }

        private void languageChi()
        {
            tabControl1.TabPages[0].Text = "常用设置";
            tabControl1.Refresh();


        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            threadWatch.Abort();
            timer.Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Checked = false;
                timer.Stop();
                button3.Enabled = true;
            }

        }
    }

    public static class RichTextBoxColorExtensions
    {
        public static void AppendText(this RichTextBox rtb, string text, Color color, Font font, bool isNewLine = false)
        {
            rtb.SuspendLayout();
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            rtb.SelectionColor = color;
            rtb.SelectionFont = font;
            rtb.AppendText(isNewLine ? $"{text}{ Environment.NewLine}" : text);
            rtb.SelectionColor = rtb.ForeColor;
            rtb.ScrollToCaret();
            rtb.ResumeLayout();
        }
    }



    class ByteBuffer
    {
        //数组的最大长度
        private const int MAX_LENGTH = 1024;

        //固定长度的中间数组
        private byte[] TEMP_BYTE_ARRAY = new byte[MAX_LENGTH];

        //当前数组长度
        private int CURRENT_LENGTH = 0;

        //当前Pop指针位置
        private int CURRENT_POSITION = 0;

        //最后返回数组
        private byte[] RETURN_ARRAY;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ByteBuffer()
        {
            this.Initialize();
        }

        /// <summary>
        /// 重载的构造函数,用一个Byte数组来构造
        /// </summary>
        /// <param name="bytes">用于构造ByteBuffer的数组</param>
        public ByteBuffer(byte[] bytes)
        {
            this.Initialize();
            this.PushByteArray(bytes);
        }


        /// <summary>
        /// 获取当前ByteBuffer的长度
        /// </summary>
        public int Length
        {
            get
            {
                return CURRENT_LENGTH;
            }
        }

        /// <summary>
        /// 获取/设置当前出栈指针位置
        /// </summary>
        public int Position
        {
            get
            {
                return CURRENT_POSITION;
            }
            set
            {
                CURRENT_POSITION = value;
            }
        }

        /// <summary>
        /// 获取ByteBuffer所生成的数组
        /// 长度必须小于 [MAXSIZE]
        /// </summary>
        /// <returns>Byte[]</returns>
        public byte[] ToByteArray()
        {
            //分配大小
            RETURN_ARRAY = new byte[CURRENT_LENGTH];
            //调整指针
            Array.Copy(TEMP_BYTE_ARRAY, 0, RETURN_ARRAY, 0, CURRENT_LENGTH);
            return RETURN_ARRAY;
        }

        /// <summary>
        /// 初始化ByteBuffer的每一个元素,并把当前指针指向头一位
        /// </summary>
        public void Initialize()
        {
            TEMP_BYTE_ARRAY.Initialize();
            CURRENT_LENGTH = 0;
            CURRENT_POSITION = 0;
        }

        /// <summary>
        /// 向ByteBuffer压入一个字节
        /// </summary>
        /// <param name="by">一位字节</param>
        public void PushByte(byte by)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = by;
        }

        /// <summary>
        /// 向ByteBuffer压入数组
        /// </summary>
        /// <param name="ByteArray">数组</param>
        public void PushByteArray(byte[] ByteArray)
        {
            //把自己CopyTo目标数组
            ByteArray.CopyTo(TEMP_BYTE_ARRAY, CURRENT_LENGTH);
            //调整长度
            CURRENT_LENGTH += ByteArray.Length;
        }

        /// <summary>
        /// 向ByteBuffer压入两字节的Short
        /// </summary>
        /// <param name="Num">2字节Short</param>
        public void PushUInt16(UInt16 Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x00ff) & 0xff);
        }

        /// <summary>
        /// 向ByteBuffer压入一个无符Int值
        /// </summary>
        /// <param name="Num">4字节UInt32</param>
        public void PushInt(UInt32 Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff000000) >> 24) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x00ff0000) >> 16) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x0000ff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x000000ff) & 0xff);
        }

        /// <summary>
        /// 向ByteBuffer压入一个Long值
        /// </summary>
        /// <param name="Num">4字节Long</param>
        public void PushLong(long Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff000000) >> 24) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x00ff0000) >> 16) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x0000ff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x000000ff) & 0xff);
        }

        /// <summary>
        /// 从ByteBuffer的当前位置弹出一个Byte,并提升一位
        /// </summary>
        /// <returns>1字节Byte</returns>
        public byte PopByte()
        {
            byte ret = TEMP_BYTE_ARRAY[CURRENT_POSITION++];
            return ret;
        }

        /// <summary>
        /// 从ByteBuffer的当前位置弹出一个Short,并提升两位
        /// </summary>
        /// <returns>2字节Short</returns>
        public UInt16 PopUInt16()
        {
            //溢出
            if (CURRENT_POSITION + 1 >= CURRENT_LENGTH)
            {
                return 0;
            }
            UInt16 ret = (UInt16)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1]);
            CURRENT_POSITION += 2;
            return ret;
        }

        /// <summary>
        /// 从ByteBuffer的当前位置弹出一个uint,并提升4位
        /// </summary>
        /// <returns>4字节UInt</returns>
        public uint PopUInt()
        {
            if (CURRENT_POSITION + 3 >= CURRENT_LENGTH)
                return 0;
            uint ret = (uint)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 24 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 16 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3]);
            CURRENT_POSITION += 4;
            return ret;
        }

        /// <summary>
        /// 从ByteBuffer的当前位置弹出一个long,并提升4位
        /// </summary>
        /// <returns>4字节Long</returns>
        public long PopLong()
        {
            if (CURRENT_POSITION + 3 >= CURRENT_LENGTH)
                return 0;
            long ret = (long)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 24 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 16 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3]);
            CURRENT_POSITION += 4;
            return ret;
        }

        /// <summary>
        /// 从ByteBuffer的当前位置弹出长度为Length的Byte数组,提升Length位
        /// </summary>
        /// <param name="Length">数组长度</param>
        /// <returns>Length长度的byte数组</returns>
        public byte[] PopByteArray(int Length)
        {
            //溢出
            if (CURRENT_POSITION + Length >= CURRENT_LENGTH)
            {
                return new byte[0];
            }
            byte[] ret = new byte[Length];
            Array.Copy(TEMP_BYTE_ARRAY, CURRENT_POSITION, ret, 0, Length);
            //提升位置
            CURRENT_POSITION += Length;
            return ret;
        }

    }
    internal class PacketWriter : BinaryWriter
    {
        private MemoryStream m_ms;
        public PacketWriter()
        {
            this.m_ms = new MemoryStream();
            this.OutStream = this.m_ms;
        }
        public byte[] GetBytes()
        {
            return this.m_ms.ToArray();
        }


    }


    internal class PacketReader : BinaryReader
    {
        private NetworkStream m_ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            m_ns = ns;
        }
        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            m_ns.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
        public byte[] ReadCanvasStrokes()
        {
            byte[] strokesBuffer;
            var length = ReadInt32();
            strokesBuffer = new byte[length];
            m_ns.Read(strokesBuffer, 0, length);

            return strokesBuffer;
        }
        public byte[] ReadMe()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            m_ns.Read(msgBuffer, 0, length);

            return msgBuffer;



        }

    }




}
