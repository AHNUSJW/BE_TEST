using BEM4W;
using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Windows.Forms;

namespace BEM4
{
    public partial class XET
    {
        public XET()
        {
            //
            userName = "admin";
            userPassword = "";
            userCFG = Application.StartupPath + @"\cfg";
            userDAT = Application.StartupPath + @"\dat";
            userLOG = Application.StartupPath + @"\log";
            userPIC = Application.StartupPath + @"\pic";
            userOut = Application.StartupPath + @"\out";
            userDATA = Application.StartupPath + @"\data";
            userDEV = Application.StartupPath + @"\dev";

            if (!Directory.Exists(userCFG))
            {
                Directory.CreateDirectory(userCFG);
            }
            if (!Directory.Exists(userDAT))
            {
                Directory.CreateDirectory(userDAT);
            }
            if (!Directory.Exists(userLOG))
            {
                Directory.CreateDirectory(userLOG);
            }
            if (!Directory.Exists(userPIC))
            {
                Directory.CreateDirectory(userPIC);
            }
            if (!Directory.Exists(userOut))
            {
                Directory.CreateDirectory(userOut);
            }
            if (!Directory.Exists(userDATA))
            {
                Directory.CreateDirectory(userDATA);
            }
            if (!Directory.Exists(userDEV))
            {
                Directory.CreateDirectory(userDEV);
            }

            //
            mePort.PortName = "COM1";
            mePort.BaudRate = 115200; //波特率固定
            mePort.DataBits = 8; //数据位固定
            mePort.StopBits = StopBits.One; //停止位固定
            mePort.Parity = Parity.None; //校验位固定
            mePort.ReceivedBytesThreshold = 1; //接收即通知
            mePort.DataReceived += new SerialDataReceivedEventHandler(mePort_DataReceived);

            //
            Array.Clear(meTXD, 0, meTXD.Length);
            Array.Clear(meRXD, 0, meRXD.Length);

            //
            snDate = System.DateTime.Now.ToString("yyMMdd");

            //
            myPC = 0;
        }

        //保存帐号
        public bool SaveToDat()
        {
            //空
            if (userDAT == null)
            {
                return false;
            }
            //创建新路径
            else if (!Directory.Exists(userDAT))
            {
                Directory.CreateDirectory(userDAT);
            }

            //写入
            try
            {
                String mePath = userDAT + @"\user." + userName + ".dat";
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                BinaryWriter meWrite = new BinaryWriter(meFS);
                //
                meWrite.Write(userName);
                meWrite.Write(userPassword);
                meWrite.Write(userDAT);
                meWrite.Write(userCFG);
                meWrite.Write(userLOG);
                //
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //
        public bool SaveToBat()
        {
            //空
            if (userCFG == null)
            {
                return false;
            }
            //创建新路径
            else if (!Directory.Exists(userCFG))
            {
                Directory.CreateDirectory(userCFG);
            }

            //写入
            try
            {
                String mePath = userCFG + @"\bat." + userName + ".cfg";
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                BinaryWriter meWrite = new BinaryWriter(meFS);
                //
                meWrite.Write(MyDefine.myXET.snDate);
                meWrite.Write(MyDefine.myXET.snBat);
                //
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool openTxtLogFile()
        {
            //空
            if (meTxtPath == null)
            {
                return false;
            }

            //写入
            try
            {
                if (File.Exists(meTxtPath))
                {
                    System.IO.File.SetAttributes(meTxtPath, FileAttributes.Normal);
                }
                meTxtFS = new FileStream(meTxtPath, FileMode.Create, FileAccess.Write);
                meTxtWriter = new StreamWriter(meTxtFS);
                //
                meTxtWriter.WriteLine(";" + System.DateTime.Now.ToString());
                meTxtWriter.WriteLine(";---------------------------------------------------------------");
                meTxtWriter.WriteLine("序号,  时间戳,    流水号,     扭矩,   角度,  模式,  扭矩峰值, 角度峰值, 扭矩单位, 角度挡位,  报警上下限");
                //
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool closeTxtLogFile()
        {
            try
            {
                meTxtWriter.Close();
                meTxtFS.Close();
                System.IO.File.SetAttributes(meTxtPath, FileAttributes.ReadOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool saveTxtLogFile(String str)
        {
            if (str.Length > 0)
            {
                try
                {
                    meTxtWriter.WriteLine(str);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void openCsvLogFile()
        {
            meCsvlines.Clear();
            if (languageNum == 0)
            {
                meCsvlines.Add("序号,时间戳,流水号,扭矩,角度,模式,扭矩峰值,角度峰值,扭矩单位,角度挡位,报警设置,报警目标上下限");
            }
            else
            {
                meCsvlines.Add("No.,time point,record No.,torque,angle,model,torque peak,angle peak,torque unit,angle unit,alarm set,alarm limits");
            }
        }

        public void saveCsvLogFile(String str)
        {
            if (str.Length > 0)
            {
                meCsvlines.Add(str);
            }
        }

        public bool closeCsvLogFile()
        {
            //空
            if (meCsvPath == null)
            {
                return false;
            }

            //无数据
            if (meCsvlines.Count < 2)
            {
                return false;
            }

            //写入
            try
            {
                if (File.Exists(meCsvPath))
                {
                    System.IO.File.SetAttributes(meCsvPath, FileAttributes.Normal);
                }
                File.WriteAllLines(meCsvPath, meCsvlines, System.Text.Encoding.Default);
                System.IO.File.SetAttributes(meCsvPath, FileAttributes.ReadOnly);
                meCsvlines.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IPEndPoint CurrentClient
        {
            get { return _CurrentClient; }
            set { _CurrentClient = value; }
        }

        public static bool RevBool
        {
            get { return _RevBool; }
            set
            {
                if (_RevBool != value)
                {
                    _RevBool = value;
                    if (_RevBool)
                    {
                        RevBoolChanged?.Invoke(0, EventArgs.Empty);
                    }
                }
            }
        }

        public void form_Update()
        {
            //委托
            if (dataUpdate != null)
            {
                dataUpdate();
            }
        }
    }
}

//end