using System;

namespace BEM4
{
    public enum RTCOM : Byte //通讯状态机
    {
        COM_NULL,

        COM_READ_HEART,
        COM_READ_A1M01DAT,
        COM_READ_A1M23DAT,
        COM_READ_A1M45DAT,
        COM_READ_A1M67DAT,
        COM_READ_A1M89DAT,
        COM_READ_A2M01DAT,
        COM_READ_A2M23DAT,
        COM_READ_A2M45DAT,
        COM_READ_A2M67DAT,
        COM_READ_A2M89DAT,

        COM_WRITE_ZERO,
        COM_WRITE_OFF,
        COM_WRITE_PARA,
        COM_WRITE_RECMODE,
        COM_WRITE_A1M01DAT,
        COM_WRITE_A1M23DAT,
        COM_WRITE_A1M45DAT,
        COM_WRITE_A1M67DAT,
        COM_WRITE_A1M89DAT,
        COM_WRITE_A2M01DAT,
        COM_WRITE_A2M23DAT,
        COM_WRITE_A2M45DAT,
        COM_WRITE_A2M67DAT,
        COM_WRITE_A2M89DAT,

        COM_WRITE_RECSIZE,
        COM_WRITE_RECDAT,

        COM_WRITE_UNBIND,

        NET_NULL,

        NET_READ_HEART,
        NET_READ_A1M01DAT,
        NET_READ_A1M23DAT,
        NET_READ_A1M45DAT,
        NET_READ_A1M67DAT,
        NET_READ_A1M89DAT,
        NET_READ_A2M01DAT,
        NET_READ_A2M23DAT,
        NET_READ_A2M45DAT,
        NET_READ_A2M67DAT,
        NET_READ_A2M89DAT,

        NET_WRITE_ZERO,
        NET_WRITE_OFF,
        NET_WRITE_PARA,
        NET_WRITE_RECMODE,
        NET_WRITE_A1M01DAT,
        NET_WRITE_A1M23DAT,
        NET_WRITE_A1M45DAT,
        NET_WRITE_A1M67DAT,
        NET_WRITE_A1M89DAT,
        NET_WRITE_A2M01DAT,
        NET_WRITE_A2M23DAT,
        NET_WRITE_A2M45DAT,
        NET_WRITE_A2M67DAT,
        NET_WRITE_A2M89DAT,

        NET_WRITE_RECSIZE,
        NET_WRITE_RECDAT,
    }

    public class CMD
    {
        public const Int16 RxSize = 2048;
        public const Int16 TxSize = 2048;

        public Byte[] CMD_READ_HEART = { 0x03, 0x51, 0x0D }; //扭矩,角度,峰值扭矩,峰值角度,电量,Ax模式,PT模式,Mx模式,记录模式,单位,ER0,角度挡位,提示挡位,isupdate,isEmpty
        public Byte[] CMD_READ_A1M01DAT = { 0x03, 0x55, 0x0C }; //读A1M0,A1M1报警参数 + (回复带capacity)
        public Byte[] CMD_READ_A1M23DAT = { 0x03, 0x56, 0x0C }; //读A1M2,A1M3报警参数
        public Byte[] CMD_READ_A1M45DAT = { 0x03, 0x57, 0x0C }; //读A1M4,A1M5报警参数
        public Byte[] CMD_READ_A1M67DAT = { 0x03, 0x58, 0x0C }; //读A1M6,A1M7报警参数
        public Byte[] CMD_READ_A1M89DAT = { 0x03, 0x59, 0x0C }; //读A1M8,A1M9报警参数
        public Byte[] CMD_READ_A2M01DAT = { 0x03, 0x5A, 0x0C }; //读A2M0,A2M1报警参数
        public Byte[] CMD_READ_A2M23DAT = { 0x03, 0x5B, 0x0E }; //读A2M2,A2M3报警参数
        public Byte[] CMD_READ_A2M45DAT = { 0x03, 0x5C, 0x0E }; //读A2M4,A2M5报警参数
        public Byte[] CMD_READ_A2M67DAT = { 0x03, 0x5D, 0x0E }; //读A2M6,A2M7报警参数
        public Byte[] CMD_READ_A2M89DAT = { 0x03, 0x5E, 0x0E }; //读A2M8,A2M9报警参数

        public Byte[] CMD_WRITE_ZERO = { 0x06, 0x61, 0x00 }; //清零
        public Byte[] CMD_WRITE_OFF = { 0x06, 0x62, 0x00 }; //关机
        public Byte[] CMD_WRITE_PARA = { 0x06, 0x63, 0x05 }; //设置A1A2模式,PT模式,单位,M0~M9模式,角度挡位
        public Byte[] CMD_WRITE_RECMODE = { 0x06, 0x64, 0x01 }; //设置数据记录模式
        public Byte[] CMD_WRITE_A1M01DAT = { 0x06, 0x65, 0x09 }; //设置A1M0,A1M1报警参数 + (回复带capacity)
        public Byte[] CMD_WRITE_A1M23DAT = { 0x06, 0x66, 0x0C }; //设置A1M2,A1M3报警参数
        public Byte[] CMD_WRITE_A1M45DAT = { 0x06, 0x67, 0x0C }; //设置A1M4,A1M5报警参数
        public Byte[] CMD_WRITE_A1M67DAT = { 0x06, 0x68, 0x0C }; //设置A1M6,A1M7报警参数
        public Byte[] CMD_WRITE_A1M89DAT = { 0x06, 0x69, 0x0C }; //设置A1M8,A1M9报警参数
        public Byte[] CMD_WRITE_A2M01DAT = { 0x06, 0x6A, 0x0C }; //设置A2M0,A2M1报警参数
        public Byte[] CMD_WRITE_A2M23DAT = { 0x06, 0x6B, 0x0E }; //设置A2M2,A2M3报警参数
        public Byte[] CMD_WRITE_A2M45DAT = { 0x06, 0x6C, 0x0E }; //设置A2M4,A2M5报警参数
        public Byte[] CMD_WRITE_A2M67DAT = { 0x06, 0x6D, 0x0E }; //设置A2M6,A2M7报警参数
        public Byte[] CMD_WRITE_A2M89DAT = { 0x06, 0x6E, 0x0E }; //设置A2M8,A2M9报警参数

        public Byte[] CMD_WRITE_RECSIZE = { 0x06, 0x71, 0x01 };  //读大小
        public Byte[] CMD_WRITE_RECDAT = { 0x06, 0x72, 0x02 };   //读记录

        public Byte[] CMD_WRITE_UNBIND = { 0x23, 0x52, 0x26, 0x45, 0x23 };//蓝牙解绑
    }

    public static class MyDefine
    {
        public static XET myXET = new XET();//参数数据使用

    }

    public delegate void freshHandler();//定义委托
}

//end
