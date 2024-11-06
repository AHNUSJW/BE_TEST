using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace BEM4
{
    //用户配置使用
    public partial class XET : Wrench
    {
        //User function
        public String userName;
        public String userPassword;
        public String userCFG;//软件参数目录
        public String userDAT;//账户信息目录
        public String userLOG;//日志数据目录
        public String userPIC;//图片和说明书目录
        public String userOut;//pdf保存位置
        public String userDATA;//数据保存位置
        public String userDEV;//设备参数目录

        //
        public String meTxtPath;
        public String meCsvPath;//日志文件路径
        public String meDataCsvPath;//数据文件路径
        private FileStream meTxtFS;
        private TextWriter meTxtWriter;
        public List<string> meCsvlines = new List<string>();

        //User PC Copyright
        public Int64 myMac = 0;
        public Int64 myVar = 0;
        public Byte myPC = 0;
        public int num_clear = 1;
        public Int16 isType = 0;
        public Boolean isTcp = false;//判断tcp的连接状态

        //蓝牙接收器
        public Boolean IsUnbind;//接收器能否解绑

        //串口
        public SerialPort mePort = new SerialPort();
        public Byte[] meTXD = new Byte[CMD.TxSize]; //发送缓冲区
        public Byte[] meRXD = new Byte[CMD.RxSize]; //接收缓冲区
        public Byte[] meCRC = new Byte[20];
        public Int16 rxRead = 0; //接收缓冲区读指针
        public Int16 rxWrite = 0; //接收缓冲区写指针
        public Int16 rxCount = 0; //接收计数
        public RTCOM rtCOM = RTCOM.COM_NULL; //通讯状态机
        public UIT myUIT = new UIT();//类型数据使用
        public CMD myCMD = new CMD();//参数数据使用

        //网口
        public Socket socket;
        public Byte[] neTXD = new Byte[CMD.TxSize]; //发送缓冲区
        public static Byte[] neRXD = new Byte[CMD.RxSize]; //接收缓冲区
        public static int bytesRead = 0; //接收计数
        public static Dictionary<string, Socket> clientConnectionItems = new Dictionary<string, Socket> { };// 存储客户端连接Socket
        // public static List<byte> RevBuf;// 接收的字节
        public static bool _RevBool = false;// 触发接收消息的委托
        public static event EventHandler RevBoolChanged = null;
        public static IPEndPoint _CurrentClient;// 当前发送数据的客户端

        //语言
        public Int16 languageNum = 0;
        //User Event 定义事件
        public event freshHandler myUpdate;
        public event freshHandler dataUpdate;
    }
}
