using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BEM4
{
    public enum MODE : Byte
    {
        [Description("模式 A1 M0")] A1M0,
        [Description("模式 A1 MX")] A1MX,
        [Description("模式 A2 M0")] A2M0,
        [Description("模式 A2 MX")] A2MX,
    }

    public enum UNIT : Byte
    {
        [Description("N·m")] UNIT_nm = 0,
        [Description("lbf·in")] UNIT_lbfin = 1,
        [Description("lbf·ft")] UNIT_lbfft = 2,
        [Description("kgf·cm")] UNIT_kgcm = 3,
    }

    //连接类型
    public enum ConnectType
    {
        BLE,
        WIFI
    }

    public class A1M0Table
    {
        public UInt32 torqueTarget;
    }

    public class A1MXTable
    {
        public UInt32 torqueLow;
        public UInt32 torqueHigh;
    }

    public class A2M0Table
    {
        public UInt32 torquePre;
        public UInt16 angleTarget;
    }

    public class A2MXTable
    {
        public UInt32 torquePre;
        public UInt16 angleLow;
        public UInt16 angleHigh;
    }

    public class A1Table
    {
        public A1M0Table A1M0 = new A1M0Table();
        public A1MXTable A1M1 = new A1MXTable();
        public A1MXTable A1M2 = new A1MXTable();
        public A1MXTable A1M3 = new A1MXTable();
        public A1MXTable A1M4 = new A1MXTable();
        public A1MXTable A1M5 = new A1MXTable();
        public A1MXTable A1M6 = new A1MXTable();
        public A1MXTable A1M7 = new A1MXTable();
        public A1MXTable A1M8 = new A1MXTable();
        public A1MXTable A1M9 = new A1MXTable();
    }

    public class A2Table
    {
        public A2M0Table A2M0 = new A2M0Table();
        public A2MXTable A2M1 = new A2MXTable();
        public A2MXTable A2M2 = new A2MXTable();
        public A2MXTable A2M3 = new A2MXTable();
        public A2MXTable A2M4 = new A2MXTable();
        public A2MXTable A2M5 = new A2MXTable();
        public A2MXTable A2M6 = new A2MXTable();
        public A2MXTable A2M7 = new A2MXTable();
        public A2MXTable A2M8 = new A2MXTable();
        public A2MXTable A2M9 = new A2MXTable();
    }

    public class RECDAT
    {
        public String stamp;
        public UInt16 index;
        public UInt32 torque;
        public Int16 angle;
    }

    //数据保存用
    public class Record
    {
        public String stamp;            //时间戳
        public String opsn;             //流水号
        public String info;             //记录描述信息

        public UInt32 torque;           //实时改变    扭矩
        public UInt32 torquePeak;       //实时改变    扭矩峰值
        public Int16 angle;             //实时改变    角度
        public Int16 anglePeak;         //实时改变    角度峰值

        public Int32 battery;           //实时改变    电量              0~3    2bit
        public Byte modeAx;             //实时参数    模式A1,A2         0~1    1bit
        public Byte modePt;             //实时参数    模式PEAK,TRACK    0~1    1bit
        public Byte modeMx;             //实时参数    模式M0~M9         0~9    4bit
        public Byte modeRec;            //实时参数    记录模式          0~1    1bit
        public UNIT torqueUnit;         //实时参数    扭矩单位          0~3    2bit
        public Int32 torqueErr;         //实时参数    扭矩超载          0~1    1bit
        public Int32 angleSpeed;        //实时参数    角度挡位          0~7    3bit
        public Int32 angleLevel;        //实时参数    角度使用挡位提示  0~8    4bit
        public Boolean isUpdate;        //实时改变    有按键调参        0~1    1bit
        public Boolean isEmpty;         //实时改变    有缓存数据        0~1    1bit

        public Record()
        {
            stamp = "";
            opsn = "";
            info = "";

            torque = 0;
            torquePeak = 0;
            angle = 0;
            anglePeak = 0;

            battery = 0;
            modeAx = 0;
            modePt = 0;
            modeMx = 0;
            modeRec = 0;
            torqueUnit = UNIT.UNIT_nm;
            torqueErr = 0;
            angleSpeed = 0;
            angleLevel = 0;
            isUpdate = false;
            isEmpty = true;
        }

        public Record(Device rec)
        {
            stamp = rec.stamp;
            opsn = rec.opsn;
            info = rec.info;

            torque = rec.torque;
            torquePeak = rec.torquePeakOld;
            angle = rec.angle;
            anglePeak = rec.anglePeakOld;

            battery = rec.battery;
            modeAx = rec.modeAx;
            modePt = rec.modePt;
            modeMx = rec.modeMx;
            modeRec = rec.modeRec;
            torqueUnit = rec.torqueUnit;
            torqueErr = rec.torqueErr;
            angleSpeed = rec.angleSpeed;
            angleLevel = rec.angleLevel;
            isUpdate = rec.isUpdate;
            isEmpty = rec.isEmpty;
        }
    }

    //保存导入数据
    public class DataProcessTable
    {
        #region 原方案
        /*public string lines;//序号
        public String stamp;//时间戳
        public String opsn;//流水号
        public string torque;//扭矩
        public string angle;//角度
        public string modePt;//模式PEAK,TRACK
        public string torquePeak;//扭矩峰值
        public string anglePeak;//角度峰值
        public String unit;//扭矩单位
        public String angleSpeed;//角度挡位
        public string strAXMX;//报警设置
        public string torqueLH;//报警目标上下限

        public DataProcessTable(string lines, string stamp,string opsn, string torque, string angle, string modePt, 
            string torquePeak, string anglePeak, string unit, string angleSpeed, string strAXMX, string torqueLH)
        {
            this.lines = lines;
            this.stamp = stamp;
            this.opsn = opsn;
            this.torque = torque;
            this.angle = angle;
            this.modePt = modePt;
            this.torquePeak = torquePeak;
            this.anglePeak = anglePeak;
            this.unit = unit;
            this.angleSpeed = angleSpeed;
            this.strAXMX = strAXMX;
            this.torqueLH = torqueLH;
        }
        */
        #endregion

        public int count_data;//读取的数据个数
        public string[] data;//读取的数据集合

        public DataProcessTable(int count_data, string[] data)
        {
            this.count_data = count_data;
            this.data = data;
        }
    }

    //设备通讯用
    public class Device
    {
        public Boolean isActive;        //
        public String mac;              //BLE MAC

        public String stamp;            //时间戳
        public String opsn;             //流水号
        public String info;             //记录描述信息

        public UInt32 torque;           //实时改变    扭矩
        public UInt32 torquePeak;       //实时改变    扭矩峰值
        public Int16 angle;             //实时改变    角度
        public Int16 anglePeak;         //实时改变    角度峰值

        public UInt32 torqueOld;        //记录torque
        public UInt32 torquePeakOld;    //记录torquePeak
        public Int16 angleOld;          //记录angle
        public Int16 anglePeakOld;      //记录anglePeak

        public Int32 battery;           //实时改变    电量              0~3    2bit
        public Byte modeAx;             //实时参数    模式A1,A2         0~1    1bit
        public Byte modePt;             //实时参数    模式PEAK,TRACK    0~1    1bit
        public Byte modeMx;             //实时参数    模式M0~M9         0~9    4bit
        public Byte modeRec;            //实时参数    记录模式          0~1    1bit
        public UNIT torqueUnit;         //实时参数    扭矩单位          0~3    2bit
        public Int32 torqueErr;         //实时参数    扭矩超载          0~1    1bit
        public Int32 angleSpeed;        //实时参数    角度挡位          0~7    3bit
        public Int32 angleLevel;        //实时参数    角度使用挡位提示  0~8    4bit
        public Boolean isUpdate;        //实时改变    有按键调参        0~1    1bit
        public Boolean isUnit;          //实时改变    有按键调参
        public Boolean isEmpty;         //实时改变    有缓存数据        0~1    1bit

        public UInt32 torqueLow;        //参数        扭矩报警下限
        public UInt32 torqueHigh;       //参数        扭矩报警上限
        public UInt32 torqueTarget;     //参数        扭矩报警目标值
        public UInt32 torquePre;        //参数        扭矩预设值@A2模式
        public UInt32 torqueMinima;     //参数        扭矩最小可调值
        public UInt32 torqueCapacity;   //参数        扭矩量程
        public UInt32 angleLow;         //参数        角度报警上限
        public UInt32 angleHigh;        //参数        角度报警下限
        public UInt32 angleTarget;      //参数        角度报警目标值
        public A1Table a1mxTable;       //参数        A1Mx表
        public A2Table a2mxTable;       //参数        A2Mx表
        public UInt16 queueSize;        //数据        队列大小
        public RECDAT[] queueArray;     //数据        队列大小
        public Int32 queuePercent;      //数据        进度

        public Device()
        {
            isActive = false;
            mac = "00:00:00:00:00:00";

            stamp = "";
            opsn = "";
            info = "";

            torque = 0;
            torquePeak = 0;
            angle = 0;
            anglePeak = 0;

            torqueOld = 0;
            torquePeakOld = 0;
            angleOld = 0;
            anglePeakOld = 0;

            battery = 0;
            modeAx = 0;
            modePt = 0;
            modeMx = 0;
            modeRec = 0;
            torqueUnit = UNIT.UNIT_nm;
            torqueErr = 0;
            angleSpeed = 0;
            angleLevel = 0;
            isUpdate = false;
            isEmpty = true;

            torqueLow = 150;
            torqueHigh = 3000;
            torqueTarget = 3000;
            torquePre = 150;
            torqueMinima = 150;
            torqueCapacity = 3000;
            angleLow = 100;
            angleHigh = 200;
            angleTarget = 300;

            a1mxTable = new A1Table();
            a1mxTable.A1M0.torqueTarget = 0;
            a1mxTable.A1M1.torqueLow = 0;
            a1mxTable.A1M1.torqueHigh = 0;
            a1mxTable.A1M2.torqueLow = 0;
            a1mxTable.A1M2.torqueHigh = 0;
            a1mxTable.A1M3.torqueLow = 0;
            a1mxTable.A1M3.torqueHigh = 0;
            a1mxTable.A1M4.torqueLow = 0;
            a1mxTable.A1M4.torqueHigh = 0;
            a1mxTable.A1M5.torqueLow = 0;
            a1mxTable.A1M5.torqueHigh = 0;
            a1mxTable.A1M6.torqueLow = 0;
            a1mxTable.A1M6.torqueHigh = 0;
            a1mxTable.A1M7.torqueLow = 0;
            a1mxTable.A1M7.torqueHigh = 0;
            a1mxTable.A1M8.torqueLow = 0;
            a1mxTable.A1M8.torqueHigh = 0;
            a1mxTable.A1M9.torqueLow = 0;
            a1mxTable.A1M9.torqueHigh = 0;

            a2mxTable = new A2Table();
            a2mxTable.A2M0.torquePre = 0;
            a2mxTable.A2M0.angleTarget = 0;
            a2mxTable.A2M1.torquePre = 0;
            a2mxTable.A2M1.angleLow = 0;
            a2mxTable.A2M1.angleHigh = 0;
            a2mxTable.A2M2.torquePre = 0;
            a2mxTable.A2M2.angleLow = 0;
            a2mxTable.A2M2.angleHigh = 0;
            a2mxTable.A2M3.torquePre = 0;
            a2mxTable.A2M3.angleLow = 0;
            a2mxTable.A2M3.angleHigh = 0;
            a2mxTable.A2M4.torquePre = 0;
            a2mxTable.A2M4.angleLow = 0;
            a2mxTable.A2M4.angleHigh = 0;
            a2mxTable.A2M5.torquePre = 0;
            a2mxTable.A2M5.angleLow = 0;
            a2mxTable.A2M5.angleHigh = 0;
            a2mxTable.A2M6.torquePre = 0;
            a2mxTable.A2M6.angleLow = 0;
            a2mxTable.A2M6.angleHigh = 0;
            a2mxTable.A2M7.torquePre = 0;
            a2mxTable.A2M7.angleLow = 0;
            a2mxTable.A2M7.angleHigh = 0;
            a2mxTable.A2M8.torquePre = 0;
            a2mxTable.A2M8.angleLow = 0;
            a2mxTable.A2M8.angleHigh = 0;
            a2mxTable.A2M9.torquePre = 0;
            a2mxTable.A2M9.angleLow = 0;
            a2mxTable.A2M9.angleHigh = 0;

            queueSize = 0;
            queuePercent = 0;
        }
    }

    public partial class Wrench
    {
        public Byte count = 0;          //心跳计数
        public Byte elapse = 0;         //心跳后时间流逝

        public System.DateTime myTime;

        public String snDate = "";      //流水号
        public UInt32 snBat = 1;        //流水号

        public Byte oldAx = 0xFF;       //监测modeAx变化
        public Byte oldMx = 0xFF;       //监测modeMx变化
        public UNIT oldTU = 0x00;       //检测扭矩单位

        public String strAXMX = "";     //记录AxMx
        public String strUNIT = "";     //记录单位

        public MODE axmx = MODE.A1M0;

        public Device DEV;              //所有的扭矩固定2小数点,所有的角度固定1小数点
        public List<Record> REC;
        public List<DataProcessTable> DPT;
    }
}

//end
