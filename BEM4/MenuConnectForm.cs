using System;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

//20240919 Lumi

namespace BEM4
{
    public partial class MenuConnectForm : Form
    {
        //
        private String myCOM = "COM1";
        private Boolean activeCom = false;

        private Boolean isClose = false;

        private ConnectType selectedConnectType = ConnectType.BLE;
        private string lastClickButton = "";//上一次点击的按键
        public MenuConnectForm()
        {
            InitializeComponent();
        }

        //启动加载
        private void MenuConnectForm_Load(object sender, EventArgs e)
        {
            form_update();
            if (MyDefine.myXET.mePort.IsOpen)
            {
                activeCom = true;
                comboBox1.Items.Add(MyDefine.myXET.mePort.PortName);
                if (comboBox1.SelectedIndex < 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "设备已通过串口连接" : "The device has been connected through the serial port.";
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else if (XET.clientConnectionItems.Count > 0)
            {
                activeCom = true;
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "设备已通过网络连接" : "The device is connected to the network.";
                button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                //获取本地ip
                getIP();
                button2.Enabled = true;
                button4.Enabled = true;
                button1_Click(null, null);
            }
            MyDefine.myXET.myUpdate += new freshHandler(receiveData);
        }

        //关闭窗口
        private void MenuConnectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isClose)
            {
                if (MyDefine.myXET.languageNum == 0)
                {
                    MessageBox.Show("设备正在连接中，请稍等...");
                }
                else
                {
                    MessageBox.Show("The e device is establishing the connection, please wait.");
                }
                e.Cancel = true;
            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                MyDefine.myXET.myUpdate -= new freshHandler(receiveData);

                if (activeCom)
                {
                    if (MyDefine.myXET.mePort.IsOpen == false)
                    {
                        try
                        {
                            MyDefine.myXET.mePort.Open();
                        }
                        catch
                        {
                            MyDefine.myXET.mePort.Close();
                        }
                    }
                    else if (XET.clientConnectionItems.Count == 0 && MyDefine.myXET.isTcp)
                    {
                        MyDefine.myXET.socket.Close();
                        button4.BackColor = SystemColors.Control;
                    }
                }
                if (XET.clientConnectionItems.Count == 0 && MyDefine.myXET.isTcp)
                {
                    MyDefine.myXET.socket.Close();
                    button4.BackColor = SystemColors.Control;
                    MyDefine.myXET.isTcp = false;//更改tcp的连接状态
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lastClickButton = "";
            //
            if (myCOM != comboBox1.Text)
            {
                MyDefine.myXET.mePort.Close();
                myCOM = comboBox1.Text;
            }
        }

        //串口刷新
        private void button1_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.mePort.IsOpen && lastClickButton != "button_unbind")
            {
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "设备已连接" : "The device is connected.";
            }
            else
            {
                lastClickButton = "";
                //刷串口
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(SerialPort.GetPortNames());

                //无串口
                if (comboBox1.Items.Count == 0)
                {
                    activeCom = false;
                    comboBox1.Text = null;
                    myCOM = null;
                }
                //有可用串口
                else
                {
                    comboBox1.Text = MyDefine.myXET.mePort.PortName;
                    //
                    if (comboBox1.SelectedIndex < 0)
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                    myCOM = comboBox1.Text;
                }
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.mePort.IsOpen && lastClickButton != "button_unbind")
            {
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "设备已连接" : "The device is connected.";
            }
            else
            {
                lastClickButton = "";
                //可以测试串口
                if (myCOM != null)
                {
                    //尝试打开串口
                    if (MyDefine.myXET.mePort.IsOpen == false)
                    {
                        try
                        {
                            MyDefine.myXET.mePort.PortName = this.myCOM;
                            MyDefine.myXET.mePort.BaudRate = 115200;            //波特率固定
                            MyDefine.myXET.mePort.DataBits = 8;                 //数据位固定
                            MyDefine.myXET.mePort.StopBits = StopBits.One;      //停止位固定
                            MyDefine.myXET.mePort.Parity = Parity.None;         //校验位固定
                            MyDefine.myXET.mePort.ReceivedBytesThreshold = 1;   //接收即通知
                            MyDefine.myXET.mePort.Open();
                            MyDefine.myXET.isType = 1;
                        }
                        catch
                        {
                            activeCom = false;
                            MyDefine.myXET.mePort.Close();
                            button1_Click(null, null);
                        }
                    }

                    //串口发送
                    if (MyDefine.myXET.mePort.IsOpen == true)
                    {
                        textBox1.Text = MyDefine.myXET.languageNum == 0 ? "适配器已打开\r\n搜索中 ." : "Adapter turned on. \r\nIn the search .";
                        button2.BackColor = Color.OrangeRed;
                        timer1.Enabled = true;
                        MyDefine.myXET.mePort_Read_A1M01DAT();
                    }
                    else
                    {
                        textBox1.Text = MyDefine.myXET.languageNum == 0 ? "适配器打开失败\r\n" : "Adapter opening failed. \r\n";
                        button2.BackColor = Color.Firebrick;
                    }
                }
                else
                {
                    //
                    textBox1.Text = MyDefine.myXET.languageNum == 0 ? "未找到适配,刷新中...\r\n" : "Adaptation not found, refreshing...\r\n";
                    button2.BackColor = Color.Firebrick;
                    //刷新
                    button1_Click(null, null);
                }
            }
        }

        //wifi刷新IP
        private void button3_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            //获取本地的ip
            string str = tb_IP.Text;
            getIP();
            if (str != tb_IP.Text)
            {
                MyDefine.myXET.isTcp = false;
                closeConnect();
                button4.Enabled = true;
            }
        }

        //网口测试
        private void button4_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            //检测并关闭串口
            if (MyDefine.myXET.mePort.IsOpen)
            {
                try
                {
                    MyDefine.myXET.mePort.Close();
                }
                catch
                {
                    textBox1.Text = MyDefine.myXET.languageNum == 0 ? "未能正确关闭串口" : "Failed to shut down the serial port correctly.";
                }
            }
            if (!MyDefine.myXET.isTcp)
            {
                button4.BackColor = Color.Firebrick;
                MyDefine.myXET.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                MyDefine.myXET.socket.Bind(new IPEndPoint(IPAddress.Parse(tb_IP.Text), Int32.Parse("5678")));
                MyDefine.myXET.socket.Listen(10);//监听
                MyDefine.myXET.isTcp = true;//更改tcp的连接状态
            }
            if (XET.clientConnectionItems.Count == 0)
            {
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "网络通讯已打开\r\n搜索中 ." : "Network communication is open. \r\nIn the search .";
                button4.BackColor = Color.OrangeRed;
                timer2.Enabled = true;

                //一旦连接上后的回调函数为AcceptCallback。当系统调用这个函数时，自动赋予的输入参数为IAsyncResoult类型变量ar。
                MyDefine.myXET.socket.BeginAccept(new AsyncCallback(AcceptCallback), MyDefine.myXET.socket);
            }
        }

        //切换蓝牙连接
        private void panel_ble_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            selectedConnectType = ConnectType.BLE;
            form_update();
        }

        //切换蓝牙连接
        private void label_ble_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            selectedConnectType = ConnectType.BLE;
            form_update();
        }

        //切换wifi连接
        private void panel_wifi_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            selectedConnectType = ConnectType.WIFI;
            form_update();
        }

        //切换wifi连接
        private void label_wifi_Click(object sender, EventArgs e)
        {
            lastClickButton = "";
            selectedConnectType = ConnectType.WIFI;
            form_update();
        }

        //更新菜单界面
        private void form_update()
        {
            //依据选择的连接方式更新界面
            switch (selectedConnectType)
            {
                case ConnectType.BLE:
                    panel_ble.BorderStyle = BorderStyle.Fixed3D;
                    panel_wifi.BorderStyle = BorderStyle.None;
                    panel_ble.BackColor = SystemColors.ActiveCaption;
                    panel_wifi.BackColor = SystemColors.GradientInactiveCaption;

                    groupBox_ble.Visible = true;
                    groupBox_wifi.Visible = false;
                    groupBox_ble.Location = new Point(12, 12);
                    break;

                case ConnectType.WIFI:
                    panel_wifi.BorderStyle = BorderStyle.Fixed3D;
                    panel_ble.BorderStyle = BorderStyle.None;
                    panel_wifi.BackColor = SystemColors.ActiveCaption;
                    panel_ble.BackColor = SystemColors.GradientInactiveCaption;

                    groupBox_wifi.Visible = true;
                    groupBox_ble.Visible = false;
                    groupBox_wifi.Location = new Point(12, 12);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="ar"></param>
        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = ar.AsyncState as Socket;
                if (listener != null)
                {
                    //完成连接，返回此时的socket通道。
                    Socket handler = listener.EndAccept(ar);
                    XET.StateObject state = new XET.StateObject();
                    state.workSocket = handler;
                    IPEndPoint clientipe = (IPEndPoint)handler.RemoteEndPoint;
                    XET.clientConnectionItems.Add(clientipe.ToString(), handler);
                    //this.BeginInvoke(new Action(() =>
                    //{
                    //    //语言选择为中文
                    //    if(MyDefine.myXET.languageNum == 0)
                    //    {
                    //        textBox1.Text += "\r\n已和扭矩扳手建立通讯" + "\r\n";
                    //    }
                    //    else
                    //    {
                    //        textBox1.Text += "\r\nCommunication has been established with \r\n" + clientipe.ToString();
                    //    }
                    //}));
                    MyDefine.myXET.isType = 2;
                    MyDefine.myXET.nePort_Read_A1M01DAT();
                    //接收的字节，0，字节长度，0，接收时调用的回调函数，接收行为的容器。
                    handler.BeginReceive(state.buffer, 0, XET.StateObject.BufferSize, 0, new AsyncCallback(MyDefine.myXET.RevCallback), state);
                }
                if (listener != null)
                {
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text += ".";
            MyDefine.myXET.mePort_Read_A1M01DAT();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox1.Text += ".";
            MyDefine.myXET.nePort_Read_A1M01DAT();
        }

        private void receiveData()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(receiveData);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    //MessageBox.Show("MenuConnectForm receiveData err 1");
                }
            }
            //本线程的操作请求
            else
            {
                string str = MyDefine.myXET.languageNum == 0 ? "读取" : "Read ";
                switch (MyDefine.myXET.rtCOM)
                {
                    case RTCOM.COM_READ_A1M01DAT:
                        timer1.Enabled = false;
                        isClose = true;
                        textBox1.Text += "\r\n" + str + "A1M01";
                        MyDefine.myXET.mePort_Read_A1M23DAT();
                        break;
                    case RTCOM.NET_READ_A1M01DAT:
                        timer2.Enabled = false;
                        isClose = true;
                        textBox1.Text += "\r\n" + str + "A1M01";
                        MyDefine.myXET.nePort_Read_A1M23DAT();
                        break;
                    case RTCOM.COM_READ_A1M23DAT:
                        textBox1.Text += "23";
                        MyDefine.myXET.mePort_Read_A1M45DAT();
                        break;
                    case RTCOM.NET_READ_A1M23DAT:
                        textBox1.Text += "23";
                        MyDefine.myXET.nePort_Read_A1M45DAT();
                        break;
                    case RTCOM.COM_READ_A1M45DAT:
                        textBox1.Text += "45";
                        MyDefine.myXET.mePort_Read_A1M67DAT();
                        break;
                    case RTCOM.NET_READ_A1M45DAT:
                        textBox1.Text += "45";
                        MyDefine.myXET.nePort_Read_A1M67DAT();
                        break;
                    case RTCOM.COM_READ_A1M67DAT:
                        textBox1.Text += "67";
                        MyDefine.myXET.mePort_Read_A1M89DAT();
                        break;
                    case RTCOM.NET_READ_A1M67DAT:
                        textBox1.Text += "67";
                        MyDefine.myXET.nePort_Read_A1M89DAT();
                        break;
                    case RTCOM.COM_READ_A1M89DAT:
                        textBox1.Text += "89\r\n";
                        MyDefine.myXET.mePort_Read_A2M01DAT();
                        break;
                    case RTCOM.NET_READ_A1M89DAT:
                        textBox1.Text += "89\r\n";
                        MyDefine.myXET.nePort_Read_A2M01DAT();
                        break;
                    case RTCOM.COM_READ_A2M01DAT:
                        textBox1.Text += str + "A2M01";
                        MyDefine.myXET.mePort_Read_A2M23DAT();
                        break;
                    case RTCOM.NET_READ_A2M01DAT:
                        textBox1.Text += str + "A2M01";
                        MyDefine.myXET.nePort_Read_A2M23DAT();
                        break;
                    case RTCOM.COM_READ_A2M23DAT:
                        textBox1.Text += "23";
                        MyDefine.myXET.mePort_Read_A2M45DAT();
                        break;
                    case RTCOM.NET_READ_A2M23DAT:
                        textBox1.Text += "23";
                        MyDefine.myXET.nePort_Read_A2M45DAT();
                        break;
                    case RTCOM.COM_READ_A2M45DAT:
                        textBox1.Text += "45";
                        MyDefine.myXET.mePort_Read_A2M67DAT();
                        break;
                    case RTCOM.NET_READ_A2M45DAT:
                        textBox1.Text += "45";
                        MyDefine.myXET.nePort_Read_A2M67DAT();
                        break;
                    case RTCOM.COM_READ_A2M67DAT:
                        textBox1.Text += "67";
                        MyDefine.myXET.mePort_Read_A2M89DAT();
                        break;
                    case RTCOM.NET_READ_A2M67DAT:
                        textBox1.Text += "67";
                        MyDefine.myXET.nePort_Read_A2M89DAT();
                        break;
                    case RTCOM.COM_READ_A2M89DAT:
                        textBox1.Text += "89\r\n";
                        MyDefine.myXET.DEV.isActive = true;
                        isClose = false;
                        button2.BackColor = Color.Green;
                        textBox1.Text += MyDefine.myXET.languageNum == 0 ? "已连接设备" : "Connected Devices.";
                        MyDefine.myXET.mePort_Read_Heart();
                        break;
                    case RTCOM.NET_READ_A2M89DAT:
                        textBox1.Text += "89\r\n";
                        MyDefine.myXET.DEV.isActive = true;
                        isClose = false;
                        button4.BackColor = Color.Green;
                        textBox1.Text += MyDefine.myXET.languageNum == 0 ? "已连接设备" : "Connected Devices.";
                        MyDefine.myXET.nePort_Read_Heart();
                        break;
                    case RTCOM.COM_WRITE_UNBIND:
                        if (MyDefine.myXET.IsUnbind)
                        {
                            textBox1.Text += "解绑成功\r\n";
                        }
                        else
                        {
                            textBox1.Text += "无法解绑\r\n";
                        }
                        button2.BackColor = SystemColors.Control;
                        button2.Enabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        //获取本地的ip
        public void getIP()
        {
            //获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            tb_IP.Text = AddressIP;
        }

        /// <summary>
        /// 关闭网络连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeConnect()
        {
            try
            {
                foreach (Socket sc in XET.clientConnectionItems.Values)
                {
                    sc.Shutdown(SocketShutdown.Both);
                    sc.Close();
                }
                //AppendToMainBoardText("# The client is closed.");
            }
            catch { }
            try
            {
                if (MyDefine.myXET.socket != null)
                {
                    MyDefine.myXET.socket.Close();
                }
            }
            catch { }
        }

        //解绑
        private void button_unbind_Click(object sender, EventArgs e)
        {
            //可以测试串口
            if (myCOM != null)
            {
                lastClickButton = "button_unbind";

                //尝试打开串口
                if (MyDefine.myXET.mePort.IsOpen == false)
                {
                    try
                    {
                        MyDefine.myXET.mePort.PortName = this.myCOM;
                        MyDefine.myXET.mePort.BaudRate = 115200;            //波特率固定
                        MyDefine.myXET.mePort.DataBits = 8;                 //数据位固定
                        MyDefine.myXET.mePort.StopBits = StopBits.One;      //停止位固定
                        MyDefine.myXET.mePort.Parity = Parity.None;         //校验位固定
                        MyDefine.myXET.mePort.ReceivedBytesThreshold = 1;   //接收即通知
                        MyDefine.myXET.mePort.Open();
                        MyDefine.myXET.isType = 1;
                    }
                    catch
                    {
                        activeCom = false;
                        MyDefine.myXET.mePort.Close();
                        button1_Click(null, null);
                    }
                }

                //串口发送
                if (MyDefine.myXET.mePort.IsOpen == true)
                {
                    timer1.Enabled = false;
                    textBox1.Text = MyDefine.myXET.languageNum == 0 ? "解绑中\r\n" : "Waiting...";
                    MyDefine.myXET.mePort_Write_UNBIND();
                }
                else
                {
                    textBox1.Text = MyDefine.myXET.languageNum == 0 ? "适配器打开失败\r\n" : "Failed. \r\n";
                }
            }
            else
            {
                //
                textBox1.Text = MyDefine.myXET.languageNum == 0 ? "未找到适配\r\n" : "Adaptation not found\r\n";
                button1_Click(null, null);
            }
        }
    }
}

//end

