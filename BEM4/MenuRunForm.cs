using BEM4W;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BEM4
{
    public partial class MenuRunForm : Form
    {
        private Boolean isEVEN = false;  //降低更新界面频率
        private Boolean isPARA = false;  //是否点击comboBox更新参数
        private Boolean isUNIT = false;  //是否切换单位,之后继续读出AxMx
        private Boolean isScroll = false;//是否拉到最后一行
        private Boolean askEPT = true;   //询问是否读出缓存
        private Int32 lines = 1;         //table表行数

        private Byte infoTick = 0;      //控制info显示时间
        private Int32 infoErr = 0;      //控制info显示时间
        private Int32 infoLevel = 0;    //控制info显示时间

        private Int32 bemPick = 0;      //picturebox中选中数据对应表格索引

        private float torqueMax = 0.0f; //记录扭矩峰值
        public Dictionary<string, float> dicTorque = new Dictionary<string, float>();//记录扭矩峰值
        private Dictionary<string, float> dicAngle = new Dictionary<string, float>();//记录角度峰值
        private Dictionary<string, string> dicIndex = new Dictionary<string, string>();//记录数据段（以流水号为界）

        private int report_start;//选取数据起始idx
        private int report_stop; //选取数据结束idx
        private int table_index = 0;//读取表格下标
        private bool isZero = false;//是否归零

        private Picture myPicture = new Picture();
        private List<DataTable> mTable = new List<DataTable>();

        private string reportFileName;
        private string reportCompany;
        private string reportLoad;
        private string reportCommodity;
        private string reportStandard;
        private string reportOpsn;
        private string reportDate;

        private string reportPeak;
        private string reportAngle;
        private string reportUnit;
        private string reportMode;
        private string reportAngleSpeed;
        private string reportStamp;

        public MenuRunForm()
        {
            //设置窗体的双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            //
            InitializeComponent();

            //利用反射设置DataGridView的双缓冲
            Type myType = this.dataGridView1.GetType();
            PropertyInfo pi = myType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        //更新表格曲线（导出为csv）
        private void updateTableFromHeart()
        {
            String myStr = "";
            String myData;
            Boolean nextLine = false;
            int k = mTable.Count;
            for (Byte i = 0; i < MyDefine.myXET.REC.Count; i++)
            {
                if (MyDefine.myXET.REC[i].opsn.Length > 0)
                {
                    //表格数据
                    if (MyDefine.myXET.REC[i].modePt == 0)
                    {
                        mTable.Add(new DataTable(
                            lines,
                            MyDefine.myXET.REC[i].stamp,
                            MyDefine.myXET.REC[i].opsn,
                            MyDefine.myXET.REC[i].torque,//track值
                            MyDefine.myXET.DEV.torqueUnit,
                            MyDefine.myXET.REC[i].angle,//track值
                            MyDefine.myXET.REC[i].info));
                        isScroll = true;
                    }
                    else
                    {
                        mTable.Add(new DataTable(
                            lines,
                            MyDefine.myXET.REC[i].stamp,
                            MyDefine.myXET.REC[i].opsn,
                            MyDefine.myXET.REC[i].torque,//track值
                            MyDefine.myXET.DEV.torqueUnit,
                            MyDefine.myXET.REC[i].angle,
                            MyDefine.myXET.REC[i].info));
                    }

                    //获取对应流水号的扭矩峰值、角度峰值
                    if (dicTorque.ContainsKey(mTable[k].opsn))
                    {
                        dicTorque[mTable[k].opsn] = dicTorque[mTable[k].opsn] > mTable[k].torque ? dicTorque[mTable[k].opsn] : mTable[k].torque;
                        dicAngle[mTable[k].opsn] = dicAngle[mTable[k].opsn] > mTable[k].angle ? dicAngle[mTable[k].opsn] : mTable[k].angle;
                    }
                    else
                    {
                        dicTorque.Add(mTable[k].opsn, mTable[k].torque);
                        dicAngle.Add(mTable[k].opsn, mTable[k].angle);

                        //记录数据段
                        dicIndex.Add(mTable[k].opsn, (k).ToString());
                        if (k - 1 >= 0 && dicIndex[mTable[k - 1].opsn].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                        {
                            dicIndex[mTable[k - 1].opsn] += ":" + (k - 1).ToString();
                        }

                    }
                    float torpeak = MyDefine.myXET.REC[i].torquePeak / 100.0f;
                    //数据调整，调整数据峰值
                    if (dicTorque[mTable[k].opsn] < torpeak)
                    {
                        dicTorque[mTable[k].opsn] = torpeak;
                        mTable[k].torque = torpeak;
                    }
                    k++;//更改mTable下标

                    //日志文件数据
                    if (nextLine)
                    {
                        myStr += "\r\n";
                    }
                    else
                    {
                        nextLine = true;
                    }
                    myStr += lines.ToString() + ",";                                    //序号
                    //myStr += $"{MyDefine.myXET.REC[i].stamp},";                         //时间戳
                    //myStr += $"{MyDefine.myXET.REC[i].opsn},";                          //流水号
                    myStr += $"=\"{MyDefine.myXET.REC[i].stamp}\",=\"{MyDefine.myXET.REC[i].opsn}\",";  //时间戳、流水号
                    myStr += (MyDefine.myXET.REC[i].torque / 100.0f).ToString() + ",";  //扭矩
                    myStr += (MyDefine.myXET.REC[i].angle / 10.0f).ToString() + ",";    //角度
                    if (MyDefine.myXET.REC[i].modePt == 0)
                    {
                        myStr += "TRACK,"; //模式
                    }
                    else
                    {
                        myStr += "PEAK,"; //模式
                    }
                    myStr += (MyDefine.myXET.REC[i].torquePeak / 100.0f).ToString() + ","; //扭矩峰值
                    myStr += (MyDefine.myXET.REC[i].anglePeak / 10.0f).ToString() + ",";   //角度峰值
                    switch (MyDefine.myXET.DEV.torqueUnit)
                    {
                        case UNIT.UNIT_nm: myStr += "N·m,"; break;       //扭矩单位
                        case UNIT.UNIT_lbfin: myStr += "lbf·in,"; break; //扭矩单位
                        case UNIT.UNIT_lbfft: myStr += "lbf·ft,"; break; //扭矩单位
                        case UNIT.UNIT_kgcm: myStr += "kgf·cm,"; break;  //扭矩单位
                    }
                    switch (MyDefine.myXET.DEV.angleSpeed)
                    {
                        case 0: myStr += "15°/sec,"; break;   //角度挡位
                        case 1: myStr += "30°/sec,"; break;   //角度挡位
                        case 2: myStr += "60°/sec,"; break;   //角度挡位
                        case 3: myStr += "120°/sec,"; break;  //角度挡位
                        case 4: myStr += "250°/sec,"; break;  //角度挡位
                        case 5: myStr += "500°/sec,"; break;  //角度挡位
                        case 6: myStr += "1000°/sec,"; break; //角度挡位
                        case 7: myStr += "2000°/sec,"; break; //角度挡位
                    }
                    myStr += MyDefine.myXET.strAXMX;

                    //数据文件
                    myData = "";
                    myData += lines.ToString() + ",";                                             //序号
                    myData += MyDataExporter.FormatDateTime(MyDefine.myXET.REC[i].opsn, MyDefine.myXET.REC[i].stamp) + ",";  //时间
                    myData += MyDefine.myXET.REC[i].opsn + ",";                                   //流水号
                    myData += BEM4W.Properties.Settings.Default.WrenchName + ",";                 //设备名称
                    string axmx = MyDataExporter.GetAXMX(MyDefine.myXET.strAXMX);
                    myData += axmx + ",";                                                         //报警设置
                    myData += MySettingsManager.GetAXMXSetting(axmx) + ",";                       //编号代码名称
                    myData += MyDataExporter.GetTorqueTargetPart1(MyDefine.myXET.strAXMX) + ",";  //目标扭矩范围
                    myData += MyDataExporter.GetTorqueTargetPart2(MyDefine.myXET.strAXMX) + ",";  //目标扭矩范围
                    myData += (MyDefine.myXET.REC[i].torque / 100.0f).ToString() + ",";           //扭矩
                    myData += (MyDefine.myXET.REC[i].angle / 10.0f).ToString() + ",";             //角度
                    if (MyDefine.myXET.REC[i].modePt == 0)
                    {
                        myData += "TRACK,"; //模式
                    }
                    else
                    {
                        myData += "PEAK,"; //模式
                    }
                    myData += (MyDefine.myXET.REC[i].torquePeak / 100.0f).ToString() + ",";       //扭矩峰值
                    myData += (MyDefine.myXET.REC[i].anglePeak / 10.0f).ToString() + ",";         //角度峰值

                    switch (MyDefine.myXET.DEV.torqueUnit)
                    {
                        case UNIT.UNIT_nm: myData += "N·m,"; break;       //扭矩单位
                        case UNIT.UNIT_lbfin: myData += "lbf·in,"; break; //扭矩单位
                        case UNIT.UNIT_lbfft: myData += "lbf·ft,"; break; //扭矩单位
                        case UNIT.UNIT_kgcm: myData += "kgf·cm,"; break;  //扭矩单位
                    }
                    switch (MyDefine.myXET.DEV.angleSpeed)
                    {
                        case 0: myData += "15°/sec,"; break;   //角度挡位
                        case 1: myData += "30°/sec,"; break;   //角度挡位
                        case 2: myData += "60°/sec,"; break;   //角度挡位
                        case 3: myData += "120°/sec,"; break;  //角度挡位
                        case 4: myData += "250°/sec,"; break;  //角度挡位
                        case 5: myData += "500°/sec,"; break;  //角度挡位
                        case 6: myData += "1000°/sec,"; break; //角度挡位
                        case 7: myData += "2000°/sec,"; break; //角度挡位
                    }
                    myData += ""; //合格判定

                    //保存数据文件
                    MyDataExporter.SaveCsvDataFile(myData);

                    //
                    lines++;
                }
            }

            //保存日志文件
            MyDefine.myXET.saveCsvLogFile(myStr);

            //更新曲线计算
            myPicture.getPoint_pictureBox(MyDefine.myXET.REC);

            //
            MyDefine.myXET.REC.Clear();
        }

        //更新表格曲线
        private void updateTableFromRecord()
        {
            String opsn;
            String myStr = "";
            String myData;
            int k = mTable.Count;//mTable 下标

            MyDefine.myXET.snDate = System.DateTime.Now.ToString("yyMMdd");
            MyDefine.myXET.snBat++;
            opsn = MyDefine.myXET.snDate + MyDefine.myXET.snBat.ToString("").PadLeft(4, '0');

            for (UInt16 i = MyDefine.myXET.DEV.queueSize; i > 0;)
            {
                //
                i--;

                //表格数据
                mTable.Add(new DataTable(
                    lines,
                    MyDefine.myXET.DEV.queueArray[i].stamp,
                    opsn,
                    MyDefine.myXET.DEV.queueArray[i].torque,
                    MyDefine.myXET.DEV.torqueUnit,
                    MyDefine.myXET.DEV.queueArray[i].angle,
                    ""));

                //获取对应流水号的扭矩峰值、角度峰值
                if (dicTorque.ContainsKey(mTable[k].opsn))
                {
                    dicTorque[mTable[k].opsn] = dicTorque[mTable[k].opsn] > mTable[k].torque ? dicTorque[mTable[k].opsn] : mTable[k].torque;
                    dicAngle[mTable[k].opsn] = dicAngle[mTable[k].opsn] > mTable[k].angle ? dicAngle[mTable[k].opsn] : mTable[k].angle;
                }
                else
                {
                    dicTorque.Add(mTable[k].opsn, mTable[k].torque);
                    dicAngle.Add(mTable[k].opsn, mTable[k].angle);

                    //记录数据段
                    dicIndex.Add(mTable[k].opsn, (k).ToString());
                    if (k - 1 >= 0 && dicIndex[mTable[k - 1].opsn].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                    {
                        dicIndex[mTable[k - 1].opsn] += ":" + (k - 1).ToString();
                    }

                }

                k++;//更改mTable下标

                isScroll = true;

                //日志文件数据
                myStr += lines.ToString() + ",";                        //序号
                //myStr += $"{MyDefine.myXET.DEV.queueArray[i].stamp},=\\";  //时间戳
                //myStr += $"{opsn},";                                       //流水号
                myStr += $"=\"{MyDefine.myXET.DEV.queueArray[i].stamp}\",=\"{opsn}\",";  //时间戳、流水号
                switch (MyDefine.myXET.DEV.modeRec)
                {
                    case 0:
                        myStr += "0,";      //扭矩
                        myStr += "0,";      //角度
                        myStr += "NULL,";   //模式
                        myStr += "0,";      //扭矩峰值
                        myStr += "0,";      //角度峰值
                        break;
                    case 1:
                        myStr += (MyDefine.myXET.DEV.queueArray[i].torque / 100.0f).ToString() + ",";  //扭矩
                        myStr += (MyDefine.myXET.DEV.queueArray[i].angle / 10.0f).ToString() + ",";    //角度
                        myStr += MyDefine.myXET.languageNum == 0 ? "TRACK(缓存)," : "TRACK(CACHE),";   //模式
                        myStr += "0,";              //扭矩峰值
                        myStr += "0,";              //角度峰值
                        break;
                    case 2:
                        myStr += "0,";              //扭矩
                        myStr += "0,";              //角度
                        myStr += MyDefine.myXET.languageNum == 0 ? "TRACK(缓存)," : "TRACK(CACHE),";   //模式
                        myStr += (MyDefine.myXET.DEV.queueArray[i].torque / 100.0f).ToString() + ",";  //扭矩峰值
                        myStr += (MyDefine.myXET.DEV.queueArray[i].angle / 10.0f).ToString() + ",";    //角度峰值
                        break;
                }
                switch (MyDefine.myXET.DEV.torqueUnit)
                {
                    case UNIT.UNIT_nm: myStr += "N·m,"; break;       //扭矩单位
                    case UNIT.UNIT_lbfin: myStr += "lbf·in,"; break; //扭矩单位
                    case UNIT.UNIT_lbfft: myStr += "lbf·ft,"; break; //扭矩单位
                    case UNIT.UNIT_kgcm: myStr += "kgf·cm,"; break;  //扭矩单位
                }
                if (i > 0) myStr += "\r\n";

                //数据文件数据
                myData = "";
                myData += lines.ToString() + ",";                                             //序号
                myData += MyDataExporter.FormatDateTime(opsn, MyDefine.myXET.DEV.queueArray[i].stamp) + ",";  //时间
                myData += opsn + ",";                                                         //流水号
                myData += BEM4W.Properties.Settings.Default.WrenchName + ",";                 //设备名称
                myData += ",";               //报警设置
                myData += ",";               //编号代码名称
                myData += ",";               //目标扭矩范围
                myData += ",";               //目标扭矩范围
                switch (MyDefine.myXET.DEV.modeRec)
                {
                    case 0:
                        myData += "0,";      //扭矩
                        myData += "0,";      //角度
                        myData += "NULL,";   //模式
                        myData += "0,";      //扭矩峰值
                        myData += "0,";      //角度峰值
                        break;
                    case 1:
                        myData += (MyDefine.myXET.DEV.queueArray[i].torque / 100.0f).ToString() + ",";  //扭矩
                        myData += (MyDefine.myXET.DEV.queueArray[i].angle / 10.0f).ToString() + ",";    //角度
                        myData += MyDefine.myXET.languageNum == 0 ? "TRACK(缓存)," : "TRACK(CACHE),";   //模式
                        myData += "0,";              //扭矩峰值
                        myData += "0,";              //角度峰值
                        break;
                    case 2:
                        myData += "0,";              //扭矩
                        myData += "0,";              //角度
                        myData += MyDefine.myXET.languageNum == 0 ? "TRACK(缓存)," : "TRACK(CACHE),";   //模式
                        myData += (MyDefine.myXET.DEV.queueArray[i].torque / 100.0f).ToString() + ",";  //扭矩峰值
                        myData += (MyDefine.myXET.DEV.queueArray[i].angle / 10.0f).ToString() + ",";    //角度峰值
                        break;
                }
                switch (MyDefine.myXET.DEV.torqueUnit)
                {
                    case UNIT.UNIT_nm: myData += "N·m,"; break;       //扭矩单位
                    case UNIT.UNIT_lbfin: myData += "lbf·in,"; break; //扭矩单位
                    case UNIT.UNIT_lbfft: myData += "lbf·ft,"; break; //扭矩单位
                    case UNIT.UNIT_kgcm: myData += "kgf·cm,"; break;  //扭矩单位
                }
                myData += ","; //角度挡位
                myData += "";  //合格判定

                //PEAK更新流水号
                if (MyDefine.myXET.DEV.modePt == 1)
                {
                    MyDefine.myXET.snBat++;
                    opsn = MyDefine.myXET.snDate + MyDefine.myXET.snBat.ToString("").PadLeft(4, '0');
                }

                //保存数据文件
                MyDataExporter.SaveCsvDataFile(myData);

                //
                lines++;
            }

            //保存日志文件
            MyDefine.myXET.saveCsvLogFile(myStr);

            //更新到曲线
            myPicture.getPoint_pictureBox(MyDefine.myXET.DEV.queueArray);

            //
            MyDefine.myXET.DEV.queueSize = 0;
            MyDefine.myXET.DEV.queueArray = null;
            MyDefine.myXET.DEV.queuePercent = 0;
        }

        //接收任务
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
                    //MessageBox.Show("MenuRunForm receiveData err 3");
                }
            }
            //本线程的操作请求
            else
            {
                if (MyDefine.myXET.DEV.isActive)
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        case RTCOM.COM_READ_HEART:
                            //
                            if (isPARA == false)
                            {
                                comboBox1.SelectedIndex = MyDefine.myXET.DEV.modePt;
                                comboBox2.SelectedIndex = MyDefine.myXET.DEV.modeAx;
                                comboBox3.SelectedIndex = MyDefine.myXET.DEV.modeMx;
                                comboBox4.SelectedIndex = (Byte)MyDefine.myXET.DEV.torqueUnit;
                                comboBox5.SelectedIndex = MyDefine.myXET.DEV.angleSpeed;
                            }
                            //读缓存
                            if ((askEPT) && (MyDefine.myXET.DEV.isEmpty == false))
                            {
                                askEPT = false;
                                if ((MyDefine.myXET.languageNum == 0 ?
                                    MessageBox.Show("扭矩扳手设备有缓存数据, [确定]读出设备缓存", "读出设备缓存?", MessageBoxButtons.YesNo) :
                                    MessageBoxEX.Show("Torque wrench device has cache data, [confirm] read device cache.", "Read device cache?", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" }))
                                     == DialogResult.Yes)
                                {
                                    if (MyDefine.myXET.isType == 2)
                                    {
                                        MyDefine.myXET.nePort_Read_RECSIZE();
                                    }
                                    else
                                    {
                                        MyDefine.myXET.mePort_Read_RECSIZE();
                                    }
                                }
                            }
                            //读AxMx表
                            else if (MyDefine.myXET.DEV.isUpdate)
                            {
                                MyDefine.myXET.mePort_Read_A1M01DAT();
                            }
                            //读AxMx表
                            else if (MyDefine.myXET.DEV.isUnit)
                            {
                                MyDefine.myXET.mePort_Read_A1M01DAT();
                            }
                            //读心跳
                            else if (MyDefine.myXET.count >= 65)
                            {
                                MyDefine.myXET.mePort_Read_Heart();
                            }
                            //更新表格和曲线
                            if (isEVEN)
                            {
                                isEVEN = false;
                                updateTableFromHeart();
                            }
                            else
                            {
                                isEVEN = true;
                            }
                            break;

                        case RTCOM.NET_READ_HEART:
                            //
                            if (isPARA == false)
                            {
                                comboBox1.SelectedIndex = MyDefine.myXET.DEV.modePt;
                                comboBox2.SelectedIndex = MyDefine.myXET.DEV.modeAx;
                                comboBox3.SelectedIndex = MyDefine.myXET.DEV.modeMx;
                                comboBox4.SelectedIndex = (Byte)MyDefine.myXET.DEV.torqueUnit;
                                comboBox5.SelectedIndex = MyDefine.myXET.DEV.angleSpeed;
                            }
                            //读缓存
                            if ((askEPT) && (MyDefine.myXET.DEV.isEmpty == false))
                            {
                                askEPT = false;
                                if ((MyDefine.myXET.languageNum == 0 ?
                                    MessageBox.Show("扭矩扳手设备有缓存数据, [确定]读出设备缓存", "读出设备缓存?", MessageBoxButtons.YesNo) :
                                    MessageBoxEX.Show("Torque wrench devices have cache data, [OK] reads out the device cache.", "Read device cache?", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" }))
                                     == DialogResult.Yes)
                                {
                                    MyDefine.myXET.nePort_Read_RECSIZE();
                                }
                            }
                            //读AxMx表
                            else if (MyDefine.myXET.DEV.isUpdate)
                            {
                                MyDefine.myXET.nePort_Read_A1M01DAT();
                            }
                            //读AxMx表
                            else if (MyDefine.myXET.DEV.isUnit)
                            {
                                MyDefine.myXET.nePort_Read_A1M01DAT();
                            }
                            //读心跳
                            else if (MyDefine.myXET.count >= 65)
                            {
                                MyDefine.myXET.nePort_Read_Heart();
                            }
                            //更新表格和曲线
                            if (isEVEN)
                            {
                                isEVEN = false;
                                updateTableFromHeart();
                            }
                            else
                            {
                                isEVEN = true;
                            }
                            break;

                        case RTCOM.COM_WRITE_PARA:
                            if (isPARA)
                            {
                                //需要更新参数
                                if ((comboBox1.SelectedIndex != MyDefine.myXET.DEV.modePt) ||
                                (comboBox2.SelectedIndex != MyDefine.myXET.DEV.modeAx) ||
                                (comboBox3.SelectedIndex != MyDefine.myXET.DEV.modeMx) ||
                                (comboBox4.SelectedIndex != (Byte)MyDefine.myXET.DEV.torqueUnit) ||
                                (comboBox5.SelectedIndex != MyDefine.myXET.DEV.angleSpeed))
                                {
                                    MyDefine.myXET.mePort_Write_PARA((Byte)comboBox1.SelectedIndex, (Byte)comboBox2.SelectedIndex, (Byte)comboBox3.SelectedIndex, (Byte)comboBox4.SelectedIndex, (Byte)comboBox5.SelectedIndex);
                                }
                                //需要更新AxMx表
                                else if (isUNIT)
                                {
                                    isUNIT = false;
                                    MyDefine.myXET.mePort_Read_A1M01DAT();
                                }
                                //不需要更新参数
                                else
                                {
                                    isPARA = false;
                                    MyDefine.myXET.mePort_Read_Heart();
                                }
                            }
                            break;

                        case RTCOM.NET_WRITE_PARA:
                            if (isPARA)
                            {
                                //需要更新参数
                                if ((comboBox1.SelectedIndex != MyDefine.myXET.DEV.modePt) ||
                                (comboBox2.SelectedIndex != MyDefine.myXET.DEV.modeAx) ||
                                (comboBox3.SelectedIndex != MyDefine.myXET.DEV.modeMx) ||
                                (comboBox4.SelectedIndex != (Byte)MyDefine.myXET.DEV.torqueUnit) ||
                                (comboBox5.SelectedIndex != MyDefine.myXET.DEV.angleSpeed))
                                {
                                    MyDefine.myXET.nePort_Write_PARA((Byte)comboBox1.SelectedIndex, (Byte)comboBox2.SelectedIndex, (Byte)comboBox3.SelectedIndex, (Byte)comboBox4.SelectedIndex, (Byte)comboBox5.SelectedIndex);
                                }
                                //需要更新AxMx表
                                else if (isUNIT)
                                {
                                    isUNIT = false;
                                    MyDefine.myXET.nePort_Read_A1M01DAT();
                                }
                                //不需要更新参数
                                else
                                {
                                    isPARA = false;
                                    MyDefine.myXET.nePort_Read_Heart();
                                }
                            }
                            break;
                        case RTCOM.COM_READ_A1M01DAT:
                            MyDefine.myXET.mePort_Read_A1M23DAT();
                            break;
                        case RTCOM.NET_READ_A1M01DAT:
                            MyDefine.myXET.nePort_Read_A1M23DAT();
                            break;
                        case RTCOM.COM_READ_A1M23DAT:
                            MyDefine.myXET.mePort_Read_A1M45DAT();
                            break;
                        case RTCOM.NET_READ_A1M23DAT:
                            MyDefine.myXET.nePort_Read_A1M45DAT();
                            break;
                        case RTCOM.COM_READ_A1M45DAT:
                            MyDefine.myXET.mePort_Read_A1M67DAT();
                            break;
                        case RTCOM.NET_READ_A1M45DAT:
                            MyDefine.myXET.nePort_Read_A1M67DAT();
                            break;
                        case RTCOM.COM_READ_A1M67DAT:
                            MyDefine.myXET.mePort_Read_A1M89DAT();
                            break;
                        case RTCOM.NET_READ_A1M67DAT:
                            MyDefine.myXET.nePort_Read_A1M89DAT();
                            break;
                        case RTCOM.COM_READ_A1M89DAT:
                            MyDefine.myXET.mePort_Read_A2M01DAT();
                            break;
                        case RTCOM.NET_READ_A1M89DAT:
                            MyDefine.myXET.nePort_Read_A2M01DAT();
                            break;
                        case RTCOM.COM_READ_A2M01DAT:
                            MyDefine.myXET.mePort_Read_A2M23DAT();
                            break;
                        case RTCOM.NET_READ_A2M01DAT:
                            MyDefine.myXET.nePort_Read_A2M23DAT();
                            break;
                        case RTCOM.COM_READ_A2M23DAT:
                            MyDefine.myXET.mePort_Read_A2M45DAT();
                            break;
                        case RTCOM.NET_READ_A2M23DAT:
                            MyDefine.myXET.nePort_Read_A2M45DAT();
                            break;
                        case RTCOM.COM_READ_A2M45DAT:
                            MyDefine.myXET.mePort_Read_A2M67DAT();
                            break;
                        case RTCOM.NET_READ_A2M45DAT:
                            MyDefine.myXET.nePort_Read_A2M67DAT();
                            break;
                        case RTCOM.COM_READ_A2M67DAT:
                            MyDefine.myXET.mePort_Read_A2M89DAT();
                            break;
                        case RTCOM.NET_READ_A2M67DAT:
                            MyDefine.myXET.nePort_Read_A2M89DAT();
                            break;
                        case RTCOM.COM_READ_A2M89DAT:
                            isPARA = false;
                            MyDefine.myXET.mePort_Read_Heart();
                            break;
                        case RTCOM.NET_READ_A2M89DAT:
                            isPARA = false;
                            MyDefine.myXET.nePort_Read_Heart();
                            break;

                        case RTCOM.COM_WRITE_RECSIZE:
                            //开始读
                            if (MyDefine.myXET.DEV.queueSize > 0)
                            {
                                MyDefine.myXET.mePort_Read_RECDAT();
                            }
                            //结束读
                            else
                            {
                                isPARA = false;
                                MyDefine.myXET.mePort_Read_Heart();
                            }
                            break;

                        case RTCOM.NET_WRITE_RECSIZE:
                            //开始读
                            if (MyDefine.myXET.DEV.queueSize > 0)
                            {
                                MyDefine.myXET.nePort_Read_RECDAT();
                            }
                            //结束读
                            else
                            {
                                isPARA = false;
                                MyDefine.myXET.nePort_Read_Heart();
                            }
                            break;

                        case RTCOM.COM_WRITE_RECDAT:
                            //检测是否读完
                            if (MyDefine.myXET.DEV.queueArray[0].index == 0)
                            {
                                //检测完整性
                                for (UInt16 i = 1; i < MyDefine.myXET.DEV.queueSize; i++)
                                {
                                    if (MyDefine.myXET.DEV.queueArray[i].index == 0)
                                    {
                                        MyDefine.myXET.mePort_Read_RECDAT(i);
                                        return;
                                    }
                                }
                                //完成
                                MyDefine.myXET.mePort_Clear_RECSIZE();

                                //更新表格和曲线
                                updateTableFromRecord();
                            }
                            break;

                        case RTCOM.NET_WRITE_RECDAT:
                            //检测是否读完
                            if (MyDefine.myXET.DEV.queueArray[0].index == 0)
                            {
                                //检测完整性
                                for (UInt16 i = 1; i < MyDefine.myXET.DEV.queueSize; i++)
                                {
                                    if (MyDefine.myXET.DEV.queueArray[i].index == 0)
                                    {
                                        MyDefine.myXET.nePort_Read_RECDAT(i);
                                        return;
                                    }
                                }
                                //完成
                                MyDefine.myXET.nePort_Clear_RECSIZE();

                                //更新表格和曲线
                                updateTableFromRecord();
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        //点击参数控件
        private void comboBox_Click(object sender, EventArgs e)
        {
            isPARA = true;
        }

        //改变参数
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxScope_axis();

            if (isPARA)
            {
                if (comboBox4.SelectedIndex != (Byte)MyDefine.myXET.DEV.torqueUnit)
                {
                    //有缓存
                    if (MyDefine.myXET.DEV.isEmpty == false)
                    {
                        //切换单位后,需要继续读AxMx
                        if ((MyDefine.myXET.languageNum == 0 ?
                            MessageBox.Show("扭矩扳手仍有缓存数据, [确定]切换单位, 建议[取消]读出缓存再切换单位", "是否切换单位?", MessageBoxButtons.OKCancel) :
                            MessageBoxEX.Show("Torque wrench still has cache data, [OK] switch unit, suggest [cancel] read cache switch unit again.", "Do you want to switch units?", MessageBoxButtons.OKCancel, new string[] { "OK", "Cancel" }))
                            == DialogResult.OK)
                        {
                            isUNIT = true;
                        }
                        //不切换单位
                        else
                        {
                            isPARA = false;
                            comboBox4.SelectedIndex = (Byte)MyDefine.myXET.DEV.torqueUnit;
                            return;
                        }
                    }
                    //无缓存,切换单位后,需要继续读AxMx
                    else
                    {
                        isUNIT = true;
                    }
                }
                if (comboBox1.SelectedIndex == 0)
                {
                    MyDefine.myXET.num_clear = 1;
                }
                //需要更新参数
                if ((comboBox1.SelectedIndex != MyDefine.myXET.DEV.modePt) ||
                (comboBox2.SelectedIndex != MyDefine.myXET.DEV.modeAx) ||
                (comboBox3.SelectedIndex != MyDefine.myXET.DEV.modeMx) ||
                (comboBox4.SelectedIndex != (Byte)MyDefine.myXET.DEV.torqueUnit) ||
                (comboBox5.SelectedIndex != MyDefine.myXET.DEV.angleSpeed))
                {
                    if (MyDefine.myXET.isType == 1)
                    {
                        MyDefine.myXET.mePort_Write_PARA((Byte)comboBox1.SelectedIndex, (Byte)comboBox2.SelectedIndex, (Byte)comboBox3.SelectedIndex, (Byte)comboBox4.SelectedIndex, (Byte)comboBox5.SelectedIndex);
                    }
                    else if (MyDefine.myXET.isType == 2)
                    {
                        MyDefine.myXET.nePort_Write_PARA((Byte)comboBox1.SelectedIndex, (Byte)comboBox2.SelectedIndex, (Byte)comboBox3.SelectedIndex, (Byte)comboBox4.SelectedIndex, (Byte)comboBox5.SelectedIndex);
                    }
                }
                //不需要更新参数
                else
                {
                    isPARA = false;
                }
            }
            updateAXMXName();
        }

        //归零
        private void button1_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.isType == 1)
            {
                MyDefine.myXET.mePort_Write_Zero();
            }
            else if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Write_Zero();
            }
        }

        //关机
        private void button2_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.DEV.isActive)
            {
                if ((MyDefine.myXET.languageNum == 0 ?
                    MessageBox.Show("扭矩扳手关机后,需要重新开机才能连接", "关闭扭矩扳手?", MessageBoxButtons.YesNo) :
                    MessageBoxEX.Show("After closing of torque wrench ,needs toe be restarted.", "Close the torque wrench?", MessageBoxButtons.OKCancel, new string[] { "Yes", "NO" }))
                    == DialogResult.Yes)
                {
                    MyDefine.myXET.DEV.isActive = false;
                    if (MyDefine.myXET.isType == 1)
                    {
                        MyDefine.myXET.mePort_Write_OFF();
                    }
                    else if (MyDefine.myXET.isType == 2)
                    {
                        MyDefine.myXET.nePort_Write_OFF();
                    }
                }
            }
        }

        //清除
        private void button3_Click(object sender, EventArgs e)
        {
            //标记线清除
            myPicture.xline_pick = 0;
            //表格下标初始化
            table_index = 0;
            //初始化下标
            myPicture.bemStart = 10000;
            myPicture.bemStop = -10000;

            mTable.Clear();
            dicTorque.Clear();
            dicIndex.Clear();
            dicAngle.Clear();

            dataGridView1.Rows.Clear();

            myPicture.Clear();
        }

        //心跳
        private void timer1_Tick(object sender, EventArgs e)
        {
            //提示信息10秒, torqueErr屏蔽angleLevel信息
            if (infoErr != MyDefine.myXET.DEV.torqueErr)
            {
                infoTick++;
                if (infoTick > 100)
                {
                    infoTick = 0;
                    infoErr = MyDefine.myXET.DEV.torqueErr;
                    infoLevel = MyDefine.myXET.DEV.angleLevel;
                }
            }
            //提示信息5秒
            else if (infoLevel != MyDefine.myXET.DEV.angleLevel)
            {
                infoTick++;
                if (infoTick > 50)
                {
                    infoTick = 0;
                    infoLevel = MyDefine.myXET.DEV.angleLevel;
                }
            }
            else
            {
                infoTick = 0;
            }

            //PEAK模式下写入表格
            if (MyDefine.myXET.num_clear % 2 == 0)
            {
                isZero = true;
                table_index--;
                if (table_index < 0)
                {
                    table_index = 0;
                }
            }

            dataGridView_update();
            //
            pictureBoxScope_draw();

            //通讯监控
            if (MyDefine.myXET.DEV.isActive)
            {
                switch (MyDefine.myXET.rtCOM)
                {
                    case RTCOM.COM_READ_HEART:
                    case RTCOM.COM_WRITE_ZERO:
                    case RTCOM.COM_WRITE_OFF:
                        if (MyDefine.myXET.elapse > 1)//0.1秒
                        {
                            MyDefine.myXET.mePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    case RTCOM.NET_READ_HEART:
                    case RTCOM.NET_WRITE_ZERO:
                    case RTCOM.NET_WRITE_OFF:
                        if (MyDefine.myXET.elapse > 1)//0.1秒
                        {
                            MyDefine.myXET.nePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    case RTCOM.COM_WRITE_PARA:
                    case RTCOM.COM_WRITE_RECMODE:
                    case RTCOM.COM_WRITE_RECSIZE:
                        if (MyDefine.myXET.elapse > 10)//1秒
                        {
                            MyDefine.myXET.mePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    case RTCOM.NET_WRITE_PARA:
                    case RTCOM.NET_WRITE_RECMODE:
                    case RTCOM.NET_WRITE_RECSIZE:
                        if (MyDefine.myXET.elapse > 10)//1秒
                        {
                            MyDefine.myXET.nePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    case RTCOM.COM_READ_A1M01DAT:
                    case RTCOM.COM_READ_A1M23DAT:
                    case RTCOM.COM_READ_A1M45DAT:
                    case RTCOM.COM_READ_A1M67DAT:
                    case RTCOM.COM_READ_A1M89DAT:
                    case RTCOM.COM_READ_A2M01DAT:
                    case RTCOM.COM_READ_A2M23DAT:
                    case RTCOM.COM_READ_A2M45DAT:
                    case RTCOM.COM_READ_A2M67DAT:
                    case RTCOM.COM_READ_A2M89DAT:
                    case RTCOM.COM_WRITE_A1M01DAT:
                    case RTCOM.COM_WRITE_A1M23DAT:
                    case RTCOM.COM_WRITE_A1M45DAT:
                    case RTCOM.COM_WRITE_A1M67DAT:
                    case RTCOM.COM_WRITE_A1M89DAT:
                    case RTCOM.COM_WRITE_A2M01DAT:
                    case RTCOM.COM_WRITE_A2M23DAT:
                    case RTCOM.COM_WRITE_A2M45DAT:
                    case RTCOM.COM_WRITE_A2M67DAT:
                    case RTCOM.COM_WRITE_A2M89DAT:
                        if (MyDefine.myXET.elapse > 30)//3秒
                        {
                            MyDefine.myXET.mePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    case RTCOM.NET_READ_A1M01DAT:
                    case RTCOM.NET_READ_A1M23DAT:
                    case RTCOM.NET_READ_A1M45DAT:
                    case RTCOM.NET_READ_A1M67DAT:
                    case RTCOM.NET_READ_A1M89DAT:
                    case RTCOM.NET_READ_A2M01DAT:
                    case RTCOM.NET_READ_A2M23DAT:
                    case RTCOM.NET_READ_A2M45DAT:
                    case RTCOM.NET_READ_A2M67DAT:
                    case RTCOM.NET_READ_A2M89DAT:
                    case RTCOM.NET_WRITE_A1M01DAT:
                    case RTCOM.NET_WRITE_A1M23DAT:
                    case RTCOM.NET_WRITE_A1M45DAT:
                    case RTCOM.NET_WRITE_A1M67DAT:
                    case RTCOM.NET_WRITE_A1M89DAT:
                    case RTCOM.NET_WRITE_A2M01DAT:
                    case RTCOM.NET_WRITE_A2M23DAT:
                    case RTCOM.NET_WRITE_A2M45DAT:
                    case RTCOM.NET_WRITE_A2M67DAT:
                    case RTCOM.NET_WRITE_A2M89DAT:
                        if (MyDefine.myXET.elapse > 30)//3秒
                        {
                            MyDefine.myXET.nePort_Read_Heart();
                        }
                        else
                        {
                            MyDefine.myXET.elapse++;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        //启动
        private void MenuRunForm_Load(object sender, EventArgs e)
        {
            //日志文件初始
            String str = System.DateTime.Now.ToString("yyMMdd.HHmmss.");
            //MyDefine.myXET.meTxtPath = MyDefine.myXET.userLOG + "\\" + str + MyDefine.myXET.userName + ".txt";
            MyDefine.myXET.meCsvPath = MyDefine.myXET.userLOG + "\\" + str + MyDefine.myXET.userName + ".csv";
            //MyDefine.myXET.openTxtLogFile();
            MyDefine.myXET.openCsvLogFile();

            //数据文件初始
            MyDefine.myXET.meDataCsvPath = MyDefine.myXET.userDATA + "\\" + str + MyDefine.myXET.userName + ".data" + ".csv";
            MyDataExporter.MeCsvPath = MyDefine.myXET.meDataCsvPath;
            MyDataExporter.OpenCsvDataFile();

            //事件委托
            MyDefine.myXET.myUpdate += new freshHandler(receiveData);

            //注册鼠标滚动事件
            pictureBox1.MouseWheel += new MouseEventHandler(PictureBox_Show_MouseWheel);

            //表格初始化
            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Font = new System.Drawing.Font("Arial", 11);
            //
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            //
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            //dataGridView1.Columns[0].HeaderText = "序号";
            //dataGridView1.Columns[1].HeaderText = "时间戳";
            //dataGridView1.Columns[2].HeaderText = "流水号";
            //dataGridView1.Columns[3].HeaderText = "扭矩";
            //dataGridView1.Columns[4].HeaderText = "单位";
            //dataGridView1.Columns[5].HeaderText = "角度°";
            //dataGridView1.Columns[6].HeaderText = "描述";

            //读心跳发生器
            timer1.Enabled = true;

            //提示读出缓存只做一次
            askEPT = true;
        }

        //退出
        private void MenuRunForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyDefine.myXET.myUpdate -= new freshHandler(receiveData);

            //MyDefine.myXET.closeTxtLogFile();
            MyDefine.myXET.closeCsvLogFile();

            MyDataExporter.CloseCsvDataFile();
        }

        //重画曲线
        private void MenuRunForm_SizeChanged(object sender, EventArgs e)
        {
            myPicture.xline_pick = 0;
            myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height);

            pictureBoxScope_axis();
        }

        //画坐标层
        private void pictureBoxScope_axis()
        {
            String myAngStr;
            String myTorStr;
            SizeF mySize;

            int left; //计算刻度线的左右坐标
            int right; //计算刻度线的左右坐标

            if (MyDefine.myXET.DEV.isActive == false)
            {
                return;
            }
            string angle = MyDefine.myXET.languageNum == 0 ? "角度" : "ANGLE";
            switch (comboBox5.SelectedIndex)
            {
                default:
                case 0: myAngStr = angle + "(15°/sec)"; break;
                case 1: myAngStr = angle + "(30°/sec)"; break;
                case 2: myAngStr = angle + "(60°/sec)"; break;
                case 3: myAngStr = angle + "(120°/sec)"; break;
                case 4: myAngStr = angle + "(250°/sec)"; break;
                case 5: myAngStr = angle + "(500°/sec)"; break;
                case 6: myAngStr = angle + "(1000°/sec)"; break;
                case 7: myAngStr = angle + "(2000°/sec)"; break;
            }
            string torque = MyDefine.myXET.languageNum == 0 ? "扭矩" : "TORQUE";
            switch (comboBox4.SelectedIndex)
            {
                default:
                case 0: myTorStr = torque + " (N·m)"; break;
                case 1: myTorStr = torque + "(lbf·in)"; break;
                case 2: myTorStr = torque + "(lbf·ft)"; break;
                case 3: myTorStr = torque + "(kgf·cm)"; break;
            }

            //层图
            Bitmap img = new Bitmap(myPicture.Width, myPicture.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //角度圆
            g.DrawEllipse(new Pen(myPicture.color_axis, 4.0f), myPicture.angCxStart, myPicture.angCyStart, myPicture.angDiameter, myPicture.angDiameter);
            g.DrawEllipse(new Pen(myPicture.color_angle_init, 6.0f), myPicture.angCentreX - 3, myPicture.angCentreY - 3, 6, 6);
            //扭矩圆
            g.DrawEllipse(new Pen(myPicture.color_axis, 4.0f), myPicture.torCxStart, myPicture.torCyStart, myPicture.torDiameter, myPicture.torDiameter);
            g.DrawEllipse(new Pen(myPicture.color_torque_init, 6.0f), myPicture.torCentreX - 3, myPicture.torAxisY - 3, 6, 6);

            //角轴轴线
            left = myPicture.angAxisX - (myPicture.boardGap / 4);
            right = myPicture.angAxisX + (myPicture.boardGap / 4);
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(myPicture.angAxStart, myPicture.angAxisY), new Point(myPicture.angAxStop, myPicture.angAxisY));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(myPicture.angAxisX, myPicture.angAyStart), new Point(myPicture.angAxisX, myPicture.angAyStop));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.angLine0), new Point(right, myPicture.angLine0));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.angLine1), new Point(right, myPicture.angLine1));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.angLine2), new Point(right, myPicture.angLine2));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.angLine3), new Point(right, myPicture.angLine3));
            //扭矩轴线
            left = myPicture.torAxisX - (myPicture.boardGap / 4);
            right = myPicture.torAxisX + (myPicture.boardGap / 4);
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(myPicture.torAxStart, myPicture.torAxisY), new Point(myPicture.torAxStop, myPicture.torAxisY));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(myPicture.torAxisX, myPicture.torAyStart), new Point(myPicture.torAxisX, myPicture.torAyStop));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.torLine0), new Point(right, myPicture.torLine0));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.torLine1), new Point(right, myPicture.torLine1));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.torLine2), new Point(right, myPicture.torLine2));
            g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(left, myPicture.torLine3), new Point(right, myPicture.torLine3));

            //文字角度文字
            mySize = g.MeasureString(myAngStr, new System.Drawing.Font("Courier New", 11));
            g.DrawString(myAngStr, new System.Drawing.Font("Courier New", 11), myPicture.brush_angle_init, myPicture.angCentreX - mySize.Width / 2, myPicture.yline_angleSpeed);

            //文字扭矩
            mySize = g.MeasureString(myTorStr, new System.Drawing.Font("Courier New", 11));
            g.DrawString(myTorStr, new System.Drawing.Font("Courier New", 11), myPicture.brush_torque_init, myPicture.torCentreX - mySize.Width / 2, myPicture.yline_torqueUnit);

            //铺图
            pictureBox1.BackgroundImage = img;

            //
            g.Dispose();
        }

        //画曲线
        private void pictureBoxScope_draw()
        {
            //铺图
            String myBatStr;
            String myLevStr;
            String myStr;
            SizeF mySize;

            if (MyDefine.myXET.DEV.isActive == false)
            {
                return;
            }
            string battery = MyDefine.myXET.languageNum == 0 ? "电量" : "POWER";
            switch (MyDefine.myXET.DEV.battery)
            {
                default:
                //case 3: myBatStr = "电池 100%"; break;
                //case 2: myBatStr = "电池 60%"; break;
                //case 1: myBatStr = "电池 30%"; break;
                //case 0: myBatStr = "电池 0%"; break;
                case 3: myBatStr = battery + " III"; break;
                case 2: myBatStr = battery + " II"; break;
                case 1: myBatStr = battery + " I"; break;
                case 0: myBatStr = battery + " -"; break;
            }

            if (MyDefine.myXET.DEV.angleLevel > MyDefine.myXET.DEV.angleSpeed)
            {
                myLevStr = MyDefine.myXET.languageNum == 0 ? "操作过快, 请提升角度档位" : "Operation is too fast, please raise the Angle gear.";
            }
            else
            {
                string angleLevel = MyDefine.myXET.languageNum == 0 ? "操作慢, 建议角度档位 " : "Slow operation, recommended Angle gear";
                switch (MyDefine.myXET.DEV.angleLevel)
                {
                    default:
                    case 0: myLevStr = angleLevel + "15°/sec"; break;
                    case 1: myLevStr = angleLevel + "30°/sec"; break;
                    case 2: myLevStr = angleLevel + "60°/sec"; break;
                    case 3: myLevStr = angleLevel + "120°/sec"; break;
                    case 4: myLevStr = angleLevel + "250°/sec"; break;
                    case 5: myLevStr = angleLevel + "500°/sec"; break;
                    case 6: myLevStr = angleLevel + "1000°/sec"; break;
                    case 7: myLevStr = angleLevel + "2000°/sec"; break;
                }
            }

            //层图
            Bitmap img = new Bitmap(myPicture.Width, myPicture.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //画线,选中的点
            if (myPicture.xline_pick > 1)
            {
                g.DrawLine(new Pen(myPicture.color_info, 1.0f), new Point(myPicture.xline_pick, 0), new Point(myPicture.xline_pick, myPicture.Height));
            }
            //画角度曲线
            if (myPicture.angPoint.Count > 1)
            {
                g.DrawCurve(new Pen(myPicture.color_angle, 2.0f), myPicture.angPoint.ToArray(), 0);
            }
            //画扭矩曲线
            if (myPicture.torPoint.Count > 1)
            {
                g.DrawCurve(new Pen(myPicture.color_torque, 2.0f), myPicture.torPoint.ToArray(), 0);
            }
            //画角度仪表盘
            g.DrawLine(new Pen(myPicture.color_angle, 5.0f), new Point(myPicture.angCentreX, myPicture.angCentreY), new Point((int)myPicture.angArrowX, (int)myPicture.angArrowY));
            //画扭矩仪表盘
            g.DrawArc(new Pen(myPicture.color_torque, 5.0f), myPicture.torCxStart, myPicture.torCyStart, myPicture.torDiameter, myPicture.torDiameter, 90, myPicture.torArcEnd);

            //显示统计值
            if (myPicture.xline_pick > 1)
            {
                int px = myPicture.xline_pick + 3;

                //获取时间
                DateTime dt = DateTime.ParseExact(mTable[bemPick].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                //语言选择为中文时
                if (MyDefine.myXET.languageNum == 0)
                {
                    g.DrawString("时间: " + dt.ToString("HH:mm:ss"), myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 20);
                    g.DrawString("扭矩: " + mTable[bemPick].torque + " " + mTable[bemPick].unit, myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 40);
                    g.DrawString("角度: " + mTable[bemPick].angle + "°", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 60);
                    g.DrawString("峰值: " + torqueMax + " " + mTable[bemPick].unit, myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 80);
                }
                else//语言选择为英文
                {
                    g.DrawString("Time: " + dt.ToString("HH:mm:ss"), myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 20);
                    g.DrawString("Torque: " + mTable[bemPick].torque + " " + mTable[bemPick].unit, myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 40);
                    g.DrawString("Angle: " + mTable[bemPick].angle + "°", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 60);
                    g.DrawString("Peak: " + torqueMax + " " + mTable[bemPick].unit, myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 80);
                }

            }
            //文字提示信息
            if (infoTick > 0)
            {
                if (infoErr != MyDefine.myXET.DEV.torqueErr)
                {
                    //语言选择为中文时
                    if (MyDefine.myXET.languageNum == 0)
                    {
                        g.DrawString("超量程使用, 建议返厂校准", new System.Drawing.Font("Courier New", 12), myPicture.brush_info, myPicture.xline_info, myPicture.yline_torqueErr);
                    }
                    else//语言选择为英文
                    {
                        g.DrawString("It is recommended to return to the factory for calibration.", new System.Drawing.Font("Courier New", 12), myPicture.brush_info, myPicture.xline_info, myPicture.yline_torqueErr);
                    }
                }
                else if (MyDefine.myXET.DEV.angleLevel != MyDefine.myXET.DEV.angleSpeed)
                {
                    g.DrawString(myLevStr, new System.Drawing.Font("Courier New", 12), myPicture.brush_info, myPicture.xline_info, myPicture.yline_angleLevel);
                }
            }
            if (MyDefine.myXET.DEV.queuePercent > 0)
            {
                //语言选择为中文时
                if (MyDefine.myXET.languageNum == 0)
                {
                    g.DrawString("读出缓存完成" + MyDefine.myXET.DEV.queuePercent.ToString() + "%", new System.Drawing.Font("Courier New", 20), myPicture.brush_info, myPicture.xline_info, myPicture.yline_queueSize);
                }
                else//语言选择为英文
                {
                    g.DrawString("Read cache complete." + MyDefine.myXET.DEV.queuePercent.ToString() + "%", new System.Drawing.Font("Courier New", 20), myPicture.brush_info, myPicture.xline_info, myPicture.yline_queueSize);
                }
            }

            //文字角度
            myStr = (MyDefine.myXET.DEV.angle / 10.0f).ToString("f1");
            mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold));
            g.DrawString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold), myPicture.brush_angle, myPicture.angCentreX - mySize.Width / 2, myPicture.yline_angle);

            //文字扭矩
            myStr = (MyDefine.myXET.DEV.torque / 100.0f).ToString("f2");
            mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold));
            g.DrawString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold), myPicture.brush_torque, myPicture.torCentreX - mySize.Width / 2, myPicture.yline_torque);

            //
            if (MyDefine.myXET.DEV.modePt == 0)
            {
                //文字角度max
                myStr = (myPicture.angleMax / 10.0f).ToString("f1") + "(MAX)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), myPicture.brush_angle, myPicture.angCentreX - (mySize.Width / 2), myPicture.yline_anglePeak);

                //文字扭矩max
                myStr = (myPicture.torqueMax / 100.0f).ToString("f2") + "(MAX)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), myPicture.brush_torque, myPicture.torCentreX - mySize.Width / 2, myPicture.yline_torquePeak);
            }
            else
            {
                //文字角度peak
                myStr = (MyDefine.myXET.DEV.anglePeak / 10.0f).ToString("f1") + "(PEAK)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), myPicture.brush_angle, myPicture.angCentreX - mySize.Width / 2, myPicture.yline_anglePeak);

                //文字扭矩peak
                myStr = (MyDefine.myXET.DEV.torquePeak / 100.0f).ToString("f2") + "(PEAK)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), myPicture.brush_torque, myPicture.torCentreX - mySize.Width / 2, myPicture.yline_torquePeak);
            }

            //文字电量
            mySize = g.MeasureString(myBatStr, new System.Drawing.Font("Courier New", 11));
            g.DrawString(myBatStr, new System.Drawing.Font("Courier New", 11), myPicture.brush_torque_init, myPicture.torCentreX - mySize.Width / 2, myPicture.yline_battery);

            pictureBox1.Image = img;

            g.Dispose();

            //
        }

        //更新表格
        private void dataGridView_update()
        {
            Int32 idx;

            if (MyDefine.myXET.DEV.isActive == false)
            {
                return;
            }
            //加行
            while (table_index < mTable.Count)
            {

                if (comboBox1.SelectedIndex == 0)
                {
                    //行数
                    idx = dataGridView1.Rows.Add();
                    //数据
                    dataGridView1.Rows[idx].Cells[0].Value = mTable[table_index].lines;
                    dataGridView1.Rows[idx].Cells[1].Value = mTable[table_index].stamp;
                    dataGridView1.Rows[idx].Cells[2].Value = mTable[table_index].opsn;
                    dataGridView1.Rows[idx].Cells[3].Value = mTable[table_index].torque;
                    dataGridView1.Rows[idx].Cells[4].Value = mTable[table_index].unit;
                    dataGridView1.Rows[idx].Cells[5].Value = mTable[table_index].angle;
                    dataGridView1.Rows[idx].Cells[6].Value = mTable[table_index].info;
                    table_index++;
                }
                else if (isZero)
                {
                    //行数
                    idx = dataGridView1.Rows.Add();
                    //数据
                    dataGridView1.Rows[idx].Cells[0].Value = mTable[table_index].lines;
                    dataGridView1.Rows[idx].Cells[1].Value = mTable[table_index].stamp;
                    dataGridView1.Rows[idx].Cells[2].Value = mTable[table_index].opsn;
                    dataGridView1.Rows[idx].Cells[3].Value = dicTorque[mTable[table_index].opsn];
                    dataGridView1.Rows[idx].Cells[4].Value = mTable[table_index].unit;
                    dataGridView1.Rows[idx].Cells[5].Value = dicAngle[mTable[table_index].opsn];
                    dataGridView1.Rows[idx].Cells[6].Value = mTable[table_index].info;
                    table_index++;
                    isZero = false;
                    MyDefine.myXET.num_clear++;
                    MenuRunForm_SizeChanged(null, null);
                }
                else
                {
                    table_index++;
                }
            }

            //删除
            while (mTable.Count > DataTable.TABLESIZE)
            {
                dataGridView1.Rows.RemoveAt(0);

                mTable.RemoveAt(0);
            }

            //移到最后一行
            if (isScroll && dataGridView1.RowCount > 0)
            {
                isScroll = false;
                //try
                //{
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                //}
                //catch
                //{

                //}
            }
        }

        //鼠标点击表
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dataGridView();
            }
            else
            {
                //获取鼠标点击表的位置
                int idx = dataGridView1.CurrentRow.Index;
                //获取流水号 
                String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";
                List<string> lines = new List<string> { };
                int count = 0;
                if (File.Exists(mePath))
                {
                    foreach (string line in File.ReadAllLines(mePath))
                    {
                        if (line.Substring(0, line.IndexOf("=")) != "reportOpsn")
                        {
                            lines.Add(line);
                            count++;
                        }
                    }
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                TextWriter meWrite = new StreamWriter(meFS);
                if (count-- > 0)
                {
                    meWrite.WriteLine(lines[count]);
                }
                meWrite.WriteLine("reportOpsn=" + dataGridView1.Rows[idx].Cells[2].Value.ToString());
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                createReport();
            }
        }

        //点击图片，显示轴线
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //取消选中
            for (int j = 0; j < dataGridView1.SelectedRows.Count; j++)
            {
                dataGridView1.SelectedRows[j].Selected = false;
            }
            //获取y轴坐标
            myPicture.yline_pick = e.Y;
            //获取选中的时间戳
            string str = myPicture.getViewIdx_pictureBox(e.X);

            //设置是否选中的数据在表中是没有的
            if (str == null || str == "")
            {
                myPicture.xline_pick = 0;
            }
            else
            {
                bool b_axis = false;
                if (comboBox1.SelectedIndex == 0)
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (str.Equals(dataGridView1.Rows[i].Cells[1].Value.ToString()))
                        {
                            //获得峰值
                            torqueMax = dicTorque[dataGridView1.Rows[i].Cells[2].Value.ToString()];
                            //选中表格
                            dataGridView1.Rows[i].Selected = true;
                            b_axis = true;
                            bemPick = i;
                            //移到表格
                            if (i > 15)
                            {
                                dataGridView1.FirstDisplayedScrollingRowIndex = i - 15;
                            }
                            else
                            {
                                dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mTable.Count; i++)
                    {
                        if (str.Equals(mTable[i].stamp))
                        {
                            torqueMax = dicTorque[mTable[i].opsn];
                            bemPick = i;
                            b_axis = true;
                            break;
                        }
                    }
                }
                if (!b_axis)
                {
                    myPicture.xline_pick = 0;
                }
            }
            //画顶层
            pictureBoxScope_draw();
        }

        //获取选点表格对应的流水号数据
        private void dataGridView()
        {
            int bp;
            //取消标线
            myPicture.xline_pick = 0;
            //初始化下标
            myPicture.bemStart = 10000;
            myPicture.bemStop = -10000;
            //曲线放大倍数为1
            myPicture.rate = 1;
            myPicture.start_x = 0;
            myPicture.old_rate = 1;

            //捕捉点击没有数据的表格报错异常
            try
            {
                if (dataGridView1.CurrentRow.Index < mTable.Count)
                {
                    //清空记录表格内容
                    myPicture.bem_stamp.Clear();
                    myPicture.bem_angle.Clear();
                    myPicture.bem_torque.Clear();

                    //初始化位置
                    bp = dataGridView1.CurrentRow.Index;
                    dataGridView1.Rows[bp].Selected = true;

                    //获取流水号
                    string str = dataGridView1.Rows[bp].Cells[2].Value.ToString();
                    int index;
                    //获取相应流水号的起止范围
                    for (index = 0; index < mTable.Count; index++)
                    {
                        if (mTable[index].opsn == str)
                        {
                            if (index < myPicture.bemStart)
                            {
                                myPicture.bemStart = index;
                            }
                            else
                            {
                                myPicture.bemStop = index;
                            }
                        }
                    }
                    //for (index = 0; index < dataGridView1.RowCount; index++)
                    //{
                    //    if (dataGridView1.Rows[index].Cells[2].Value.ToString() == str)
                    //    {
                    //        if (index < myPicture.bemStart)
                    //        {
                    //            myPicture.bemStart = index;
                    //        }
                    //        if (index > myPicture.bemStop)
                    //        {
                    //            myPicture.bemStop = index;
                    //        }
                    //    }
                    //}
                    for (int i = myPicture.bemStart; i <= myPicture.bemStop; i++)
                    {
                        myPicture.bem_stamp.Add((string)mTable[i].stamp);
                        myPicture.bem_torque.Add((int)(mTable[i].torque * 100));
                        myPicture.bem_angle.Add((int)(mTable[i].angle * 10));
                    }

                    //画顶层
                    pictureBoxScope_draw();
                }
            }
            catch
            {

            }
        }

        //鼠标滚动控制放大缩小
        private void PictureBox_Show_MouseWheel(object sender, MouseEventArgs e)
        {
            myPicture.old_rate = myPicture.rate;
            int rate_x = e.X;
            int x_t;
            if (e.Delta > 0)
            {
                if (myPicture.rate >= 1)
                {
                    myPicture.rate *= 1.2f;
                    myPicture.rate = (float)Math.Round(myPicture.rate, 2);//四舍五入，保留两位小数
                    if (myPicture.rate >= 5.18f)
                    {
                        myPicture.rate = 5.18f;
                    }
                }
                else
                {
                    myPicture.rate += 0.1f;
                }
            }
            if (e.Delta < 0)
            {
                if (myPicture.rate > 1)
                {
                    myPicture.rate /= 1.2f;
                }
                else
                {
                    myPicture.rate -= 0.1f;
                    if (myPicture.rate <= 0.5f)
                    {
                        myPicture.rate = 0.5f;
                    }
                }
                myPicture.rate = (float)Math.Round(myPicture.rate, 2);
            }
            //清除轴线
            myPicture.xline_pick = 0;

            //计算放大位置
            x_t = (int)Math.Round((rate_x - myPicture.pointStart) / myPicture.old_rate, 0) + myPicture.start_x;
            x_t = x_t > 0 ? x_t - (int)Math.Round((rate_x - myPicture.pointStart) / myPicture.rate, 0) : 0;
            x_t = x_t > 0 ? x_t : 0;
            myPicture.start_x = x_t;

            //更新曲线计算
            myPicture.getPoint_pictureBox(MyDefine.myXET.REC);
        }

        //生成报告
        private void createReport()
        {
            //构造窗口
            MenuExportReportForm exportReportForm = new MenuExportReportForm();

            //初始化数据
            if (dicIndex[mTable[mTable.Count - 1].opsn].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
            {
                dicIndex[mTable[mTable.Count - 1].opsn] += ":" + (mTable.Count - 1);
            }


            //打开窗口用户输入
            exportReportForm.StartPosition = FormStartPosition.CenterParent;
            exportReportForm.ShowDialog();
            this.BringToFront();
            if (mTable.Count > 0 && exportReportForm.report)
            {
                this.reportFileName = exportReportForm.reportFileName;
                this.reportCompany = exportReportForm.reportCompany;
                this.reportLoad = exportReportForm.reportLoad;
                this.reportCommodity = exportReportForm.reportCommodity;
                this.reportStandard = exportReportForm.reportStandard;
                this.reportOpsn = exportReportForm.reportOpsn;
                this.reportDate = exportReportForm.reportDate;
                try
                {
                    string[] ag = dicIndex[reportOpsn].Split(':');
                    report_start = Convert.ToInt32(ag[0]);
                    report_stop = Convert.ToInt32(ag[1]);
                    if (report_stop - report_start > DataPicture.minLength)
                    {
                        //设置峰值、角度、单位、操作模式的数据
                        reportPeak = dicTorque[reportOpsn].ToString();
                        reportAngle = dicAngle[reportOpsn].ToString();
                        reportUnit = mTable[report_start].unit;
                        reportMode = mTable[report_start].info;
                        if (reportMode == "")
                        {
                            reportMode = "TRACK(缓存)";
                        }
                        else
                        {
                            reportMode = reportMode.Split(',')[0];
                        }

                        //获取时间戳
                        DateTime dt = DateTime.ParseExact(mTable[report_start].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                        reportStamp = dt.ToString("HH:mm:ss");
                        dt = DateTime.ParseExact(mTable[report_stop].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                        reportStamp += "-" + dt.ToString("HH:mm:ss");
                    }
                    else
                    {
                        if (MyDefine.myXET.languageNum == 0)//语言选择为中文时
                        {
                            MessageBox.Show(String.Format("流水号为{0}的数据不符合要求", reportOpsn));
                        }
                        else
                        {
                            MessageBox.Show(String.Format("The data serial number {0} is invalid.", reportOpsn));
                        }
                        return;
                    }
                }
                catch
                {
                    if (MyDefine.myXET.languageNum == 0)//语言选择为中文时
                    {
                        MessageBox.Show(String.Format("没有找到流水号为{0}的数据", reportOpsn));
                    }
                    else
                    {
                        MessageBox.Show(String.Format("No data with serial number {0} was found", reportOpsn));
                    }
                    return;
                }

                //生成报告
                CreatDataPdf();
            }
        }

        // 页眉页脚水印
        public class IsHandF : PdfPageEventHelper, IPdfPageEvent
        {
            //页事件
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                //页眉页脚使用字体
                iTextSharp.text.Font fontJingdu = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 7.0f, iTextSharp.text.Font.NORMAL);

                //页眉 Top - y
                //页脚 Bottom + y
                PdfContentByte myIfo = writer.DirectContent;
                Phrase footer3 = new Phrase("芜湖艾瑞特机电设备有限公司", fontJingdu);
                ColumnText.ShowTextAligned(myIfo, Element.ALIGN_CENTER, footer3, document.Right / 2, document.Bottom + 12, 0);

                #region 水印
                try
                {
                    PdfContentByte myPic = writer.DirectContentUnder;//水印在内容下方添加
                    PdfGState myGS = new PdfGState();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(MyDefine.myXET.userPIC + @"\logo.jpg");//水印图片
                    image.RotationDegrees = 30;//旋转角度
                    myGS.FillOpacity = 0.1f;//透明度
                    myPic.SetGState(myGS);

                    float width = document.Right + document.RightMargin; //pdf宽
                    float height = document.Top + document.TopMargin; //pdf高
                    float xnum = 3; //一行3个logo
                    float ynum = 5; //一列5个logo
                    float xspace = (width - (xnum * image.Right)) / xnum; //logo间距
                    float yspace = (height - (ynum * image.Top)) / ynum; //logo间距
                    for (int x = 0; x < xnum; x++)
                    {
                        for (int y = 0; y < ynum; y++)
                        {
                            image.SetAbsolutePosition(0.5f * xspace + x * (xspace + image.Right), 0.5f * yspace + y * (yspace + image.Top));
                            myPic.AddImage(image);
                        }
                    }
                }
                catch
                {

                }
                #endregion
            }
        }

        //创建pdf
        private void CreatDataPdf()
        {
            #region 参数
            int page = 0;//记录页数
            const int LENL = 24;//设置数据占位长度
            const int LENS = 16;
            int myMax = 55;//获取最大长度
            String blankLine = " ";

            List<String> myLS = new List<String>();
            MenuDataProcessForm mdp = new MenuDataProcessForm();
            #endregion

            #region 报告文件

            //保存报告路径和文件名
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = reportLoad + @"\" + reportFileName + ".pdf";
            if (File.Exists(fileDialog.FileName))
            {
                DialogResult dr = (MyDefine.myXET.languageNum == 0 ?
                    MessageBox.Show("存在同名文件，确定要覆盖它吗？", "提示", MessageBoxButtons.YesNo) :
                    MessageBoxEX.Show("A file with the same name exists. Are you sure you want to overwrite it?", "Hint", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" }));
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            //创建新文档对象,页边距(X,X,Y,Y)
            Document document = new Document(PageSize.A4, 48, 16, 16, 0);

            PdfWriter writer;
            try
            {
                //路径设置; FileMode.Create文档不在会创建，存在会覆盖
                writer = PdfWriter.GetInstance(document, new FileStream(fileDialog.FileName, FileMode.Create));
            }
            catch
            {
                if (MyDefine.myXET.languageNum == 0)
                {
                    MessageBox.Show("请先关闭该文档", "提示");
                }
                else
                {
                    MessageBox.Show("Please close this document first.", "Hint");
                }
                return;
            }

            //添加信息
            document.AddTitle("芜湖艾瑞特机电设备有限公司");
            document.AddSubject(" 扭力测试曲线报告");
            document.AddKeywords("report");

            //创建字体，STSONG.TTF空格不等宽
            iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 14.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontItem = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontContent = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fontMessage = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 8.0f, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fontJingdu = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 7.0f, iTextSharp.text.Font.NORMAL);

            //页眉页脚水印
            writer.PageEvent = new IsHandF();

            //将pdf变量设置为只读选项
            writer.SetEncryption(null, null, PdfWriter.AllowPrinting, PdfWriter.STANDARD_ENCRYPTION_128);

            //打开
            document.Open();

            #endregion

            #region 每页的处理
            //添加元素
            document.Add(mdp.CreateParagraph("Page" + (++page), fontMessage, Element.ALIGN_RIGHT));
            //document.Add(new Paragraph(blankLine, fontItem));

            document.Add(mdp.CreateParagraph(reportCompany, fontTitle, Element.ALIGN_CENTER));//标题（公司名称）
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(mdp.CreateParagraph("扭力测试曲线报告", fontItem, Element.ALIGN_CENTER));//副标题（报告名称）
            document.Add(mdp.CreateParagraph("Torque test curve Report", fontItem, Element.ALIGN_CENTER));//副标题（报告名称）

            for (int j = 0; j < 3; j++)
            {
                document.Add(new Paragraph(blankLine, fontItem));
            }

            myLS.Clear();
            myLS.Add("Base information".PadRight(LENL, ' '));
            myLS.Add("COMMODITY".PadRight(LENL, ' '));
            myLS.Add("STANDRAD".PadRight(LENL, ' '));
            myLS.Add("THE NO".PadRight(LENL, ' '));
            myLS.Add("DATE".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + reportCommodity;
            myLS[2] = myLS[2] + ":  " + reportStandard;
            myLS[3] = myLS[3] + ":  " + reportOpsn;
            myLS[4] = myLS[4] + ":  " + reportDate;
            //myMax = mdp.GetJoinLen(myLS, LENL, 24);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(基本信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(品    名)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(执行标准)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(序 列 号)";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "(报告日期)";

            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));

            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            myLS.Clear();
            myLS.Add("General information".PadRight(LENL, ' '));
            myLS.Add("扭矩峰值(MAX)".PadRight(LENL - 4, ' '));
            myLS.Add("角    度(ANGLEI)".PadRight(LENL - 2, ' '));
            myLS.Add("扭矩单位(UNIT)".PadRight(LENL - 4, ' '));
            myLS[1] = myLS[1] + ":  " + reportPeak;
            myLS[2] = myLS[2] + ":  " + reportAngle;
            myLS[3] = myLS[3] + ":  " + reportUnit.Replace("·", ".");
            //myMax = mdp.GetJoinLen(myLS, LENL, 30);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "操作模式(MODE)".PadRight(17, ' ') + ":  " + reportMode;
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "角 速 度(ANGLESPEED)".PadRight(18, ' ') + ":  " + reportAngleSpeed;
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "操作时间(STAMP)".PadRight(17, ' ') + ":  " + reportStamp;

            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            ///////////////////////////////////////////////////////////////////

            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(mdp.CreateParagraph("扭矩角度曲线图", fontItem, Element.ALIGN_CENTER));
            document.Add(mdp.CreateParagraph("Torque angle graph", fontItem, Element.ALIGN_CENTER));
            document.Add(new Paragraph(blankLine, fontItem));

            //画图的点
            List<Point> point_torque = new List<Point>();

            for (int i = 0; i < report_stop - report_start; i++)
            {
                point_torque.Add(new Point(i + 28, (int)(320 - mTable[report_start + i].torque * 8)));
            }
            //画图
            document.Add(iTextSharp.text.Image.GetInstance(mdp.GetBitmapPoint(mTable[report_start].unit, reportStamp, point_torque), System.Drawing.Imaging.ImageFormat.Bmp));
            #endregion

            #region 关闭
            //关闭
            document.Close();
            writer.Close();

            //调出pdf
            Process.Start(fileDialog.FileName);
            #endregion
        }
        //复原
        private void button4_Click(object sender, EventArgs e)
        {
            //取消标线
            myPicture.xline_pick = 0;
            //初始化下标
            myPicture.bemStart = 10000;
            myPicture.bemStop = -10000;
            //曲线放大倍数为1
            myPicture.rate = 1;
            myPicture.start_x = 0;
            myPicture.old_rate = 1;

            //画顶层
            pictureBoxScope_draw();
        }

        //设置表格格式
        private void button5_Click(object sender, EventArgs e)
        {
            MenuDataSettingForm mydataSettingForm = new MenuDataSettingForm();
            mydataSettingForm.StartPosition = FormStartPosition.CenterParent;
            mydataSettingForm.ShowDialog();
            this.BringToFront();
        }

        //更新AXMX Name
        private void updateAXMXName()
        {
            //获取AXMX Name
            string axmx = comboBox2.Text + comboBox3.Text;
            label_axmxName.Text = MySettingsManager.GetAXMXSetting(axmx);
        }
    }
}
