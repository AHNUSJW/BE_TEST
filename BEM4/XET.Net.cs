using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace BEM4
{
    public partial class XET
    {
        /// <summary>
        /// 容器的结构类型为：Code
        ///容器至少为一个socket类型。
        /// </summary>
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 22000;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="ar"></param>
        public void RevCallback(IAsyncResult ar)
        {
            // 获取客户请求的socket
            StateObject state = (StateObject)ar.AsyncState;
            //Socket socketClient= ar.AsyncState as Socket;
            Socket handler = state.workSocket;
            if (handler != null)
            {
                try
                {
                    IPEndPoint clientipe = (IPEndPoint)handler.RemoteEndPoint;
                    // Read data from the client socket.
                    //完成一次连接。数据存储在state.buffer里，bytesRead为读取的长度。
                    bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        byte[] a = new byte[bytesRead];
                        Buffer.BlockCopy(state.buffer, 0, a, 0, bytesRead);
                        a.CopyTo(neRXD, 0);
                        CurrentClient = clientipe;
                        nePort_DataReceived();
                        // 发送数据byteData，回调函数SendCallback。容器handler
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RevCallback), state);
                        RevBool = true;
                        RevBool = false;
                    }
                }
                catch (Exception ex)
                {
                    if(ex is ObjectDisposedException)
                    {
                        isTcp = false;
                        if (clientConnectionItems.Count > 0)
                        {
                            clientConnectionItems.Clear();
                        }
                        //关闭连接
                        if (handler.Connected)
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    }
                }
            }

        }

        //接收触发函数
        private void nePort_DataReceived()
        {
            switch (rtCOM)
            {
                case RTCOM.NET_READ_HEART:
                    nePort_ReceiveReadHeart();
                    break;
                case RTCOM.NET_READ_A1M01DAT:
                    nePort_ReceiveReadA1M01DAT();
                    break;
                case RTCOM.NET_READ_A1M23DAT:
                    nePort_ReceiveReadA1M23DAT();
                    break;
                case RTCOM.NET_READ_A1M45DAT:
                    nePort_ReceiveReadA1M45DAT();
                    break;
                case RTCOM.NET_READ_A1M67DAT:
                    nePort_ReceiveReadA1M67DAT();
                    break;
                case RTCOM.NET_READ_A1M89DAT:
                    nePort_ReceiveReadA1M89DAT();
                    break;
                case RTCOM.NET_READ_A2M01DAT:
                    nePort_ReceiveReadA2M01DAT();
                    break;
                case RTCOM.NET_READ_A2M23DAT:
                    nePort_ReceiveReadA2M23DAT();
                    break;
                case RTCOM.NET_READ_A2M45DAT:
                    nePort_ReceiveReadA2M45DAT();
                    break;
                case RTCOM.NET_READ_A2M67DAT:
                    nePort_ReceiveReadA2M67DAT();
                    break;
                case RTCOM.NET_READ_A2M89DAT:
                    nePort_ReceiveReadA2M89DAT();
                    break;

                case RTCOM.NET_WRITE_PARA:
                    nePort_ReceiveWritePARA();
                    break;
                case RTCOM.NET_WRITE_RECMODE:
                    nePort_ReceiveWriteRECMODE();
                    break;
                case RTCOM.NET_WRITE_A1M01DAT:
                    nePort_ReceiveWriteA1M01DAT();
                    break;
                case RTCOM.NET_WRITE_A1M23DAT:
                    nePort_ReceiveWriteA1M23DAT();
                    break;
                case RTCOM.NET_WRITE_A1M45DAT:
                    nePort_ReceiveWriteA1M45DAT();
                    break;
                case RTCOM.NET_WRITE_A1M67DAT:
                    nePort_ReceiveWriteA1M67DAT();
                    break;
                case RTCOM.NET_WRITE_A1M89DAT:
                    nePort_ReceiveWriteA1M89DAT();
                    break;
                case RTCOM.NET_WRITE_A2M01DAT:
                    nePort_ReceiveWriteA2M01DAT();
                    break;
                case RTCOM.NET_WRITE_A2M23DAT:
                    nePort_ReceiveWriteA2M23DAT();
                    break;
                case RTCOM.NET_WRITE_A2M45DAT:
                    nePort_ReceiveWriteA2M45DAT();
                    break;
                case RTCOM.NET_WRITE_A2M67DAT:
                    nePort_ReceiveWriteA2M67DAT();
                    break;
                case RTCOM.NET_WRITE_A2M89DAT:
                    nePort_ReceiveWriteA2M89DAT();
                    break;

                case RTCOM.NET_WRITE_RECSIZE:
                    nePort_ReceiveWriteRECSIZE();
                    break;
                case RTCOM.NET_WRITE_RECDAT:
                    nePort_ReceiveWriteRECDAT();
                    break;

                default:
                    break;
            }
        }
        //
        //断开异常连接
        private void closeConnection(string str)
        {
            //客户端产生异常了
            Socket socket = clientConnectionItems[str];
            clientConnectionItems.Remove(str);
            //关闭连接
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        #region 网口接收
        //网口接收
        private void nePort_ReceiveReadHeart()
        {
            //50ms间隔回复65个数据,共1170字节
            long tick = System.DateTime.Now.Ticks;

            //一帧长度18字节
            const Byte LEN = 18;

            //读
            for(int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //解析
            while (rxCount >= LEN)
            {
                //帧头
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x51) && (rxBuff(2) == 0x0D))
                {
                    //取数
                    for (byte i = 0; i < (LEN - 2); i++)
                    {
                        meCRC[i] = rxBuff(i);
                    }

                    //校验
                    myUIT.b0 = rxBuff(LEN - 2);
                    myUIT.b1 = rxBuff(LEN - 1);
                    if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
                    {
                        myUIT.b3 = 0;
                        myUIT.b2 = meCRC[5];
                        myUIT.b1 = meCRC[4];
                        myUIT.b0 = meCRC[3];
                        DEV.torque = myUIT.ui;

                        myUIT.b1 = meCRC[7];
                        myUIT.b0 = meCRC[6];
                        DEV.angle = myUIT.s0;

                        myUIT.b3 = 0;
                        myUIT.b2 = meCRC[10];
                        myUIT.b1 = meCRC[9];
                        myUIT.b0 = meCRC[8];
                        DEV.torquePeak = myUIT.ui;

                        myUIT.b1 = meCRC[12];
                        myUIT.b0 = meCRC[11];
                        DEV.anglePeak = myUIT.s0;

                        DEV.battery = meCRC[13] & 0x03;
                        DEV.modeAx = (Byte)((meCRC[13] >> 2) & 0x01);
                        DEV.modePt = (Byte)((meCRC[13] >> 3) & 0x01);
                        DEV.modeMx = (Byte)(meCRC[13] >> 4);
                        DEV.modeRec = (Byte)(meCRC[14] & 0x03);
                        DEV.torqueUnit = (UNIT)((meCRC[14] >> 2) & 0x03);
                        DEV.torqueErr = (meCRC[14] >> 4) & 0x01;
                        DEV.angleSpeed = (meCRC[14] >> 5) & 0x07;
                        DEV.angleLevel = meCRC[15] & 0x0F;
                        DEV.isUpdate = ((meCRC[15] >> 4) & 0x01) == 1 ? true : false;
                        DEV.isEmpty = ((meCRC[15] >> 5) & 0x01) == 1 ? true : false;

                        //更新时间戳
                        myTime = new DateTime(tick);
                        DEV.stamp = myTime.ToString("HHmmssfff");

                        //更新流水号
                        getDeviceOpsn();

                        //缓存
                        REC.Add(new Record(DEV));

                        //旧值
                        DEV.torqueOld = DEV.torque;
                        DEV.torquePeakOld = DEV.torquePeak;
                        DEV.angleOld = DEV.angle;
                        DEV.anglePeakOld = DEV.anglePeak;

                        //加50ms时间
                        tick += (50 * 10000);

                        //心跳计数
                        count++;
                        elapse = 0;

                        //
                        rxBuff_Read_Inc(LEN);
                    }
                    else
                    {
                        rxBuff_Read_Inc();
                    }
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA1M01DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x55) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M0.torqueTarget = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M1.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M1.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.torqueCapacity = myUIT.ui;
                DEV.torqueMinima = getMinimaTorque(DEV.torqueCapacity);

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA1M01DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA1M23DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x56) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M2.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M2.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M3.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M3.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA1M23DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA1M45DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x57) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M4.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M4.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M5.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M5.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA1M45DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA1M67DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x58) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M6.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M6.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M7.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M7.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA1M67DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA1M89DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x59) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M8.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M8.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M9.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M9.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA1M89DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA2M01DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x5A) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M0.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M0.angleTarget = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[10];
                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M1.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[12];
                myUIT.b0 = meCRC[11];
                DEV.a2mxTable.A2M1.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M1.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA2M01DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA2M23DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x5B) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M2.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M2.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M2.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M3.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M3.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M3.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA2M23DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA2M45DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x5C) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M4.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M4.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M4.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M5.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M5.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M5.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA2M45DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA2M67DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x5D) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M6.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M6.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M6.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M7.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M7.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M7.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA2M67DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveReadA2M89DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x03) && (rxBuff(1) == 0x5E) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M8.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M8.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M8.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M9.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M9.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M9.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadA2M89DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWritePARA()
        {
            const Byte LEN = 10;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x63) && (rxBuff(2) == 0x05))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                DEV.modeAx = meCRC[3];
                DEV.modePt = meCRC[4];
                DEV.modeMx = meCRC[5];
                DEV.torqueUnit = (UNIT)meCRC[6];
                DEV.angleSpeed = meCRC[7];

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWritePARA();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteRECMODE()
        {
            const Byte LEN = 6;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x64) && (rxBuff(2) == 0x01))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                DEV.modeRec = meCRC[3];

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteRECMODE();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA1M01DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x65) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M0.torqueTarget = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M1.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M1.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.torqueCapacity = myUIT.ui;
                DEV.torqueMinima = getMinimaTorque(DEV.torqueCapacity);

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA1M01DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA1M23DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x66) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M2.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M2.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M3.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M3.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA1M23DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA1M45DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x67) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M4.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M4.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M5.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M5.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA1M45DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA1M67DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x68) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M6.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M6.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M7.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M7.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA1M67DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA1M89DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x69) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a1mxTable.A1M8.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[8];
                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a1mxTable.A1M8.torqueHigh = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[11];
                myUIT.b1 = meCRC[10];
                myUIT.b0 = meCRC[9];
                DEV.a1mxTable.A1M9.torqueLow = myUIT.ui;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[14];
                myUIT.b1 = meCRC[13];
                myUIT.b0 = meCRC[12];
                DEV.a1mxTable.A1M9.torqueHigh = myUIT.ui;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA1M89DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA2M01DAT()
        {
            const Byte LEN = 17;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x6A) && (rxBuff(2) == 0x0C))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M0.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M0.angleTarget = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[10];
                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M1.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[12];
                myUIT.b0 = meCRC[11];
                DEV.a2mxTable.A2M1.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M1.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA2M01DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA2M23DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x6B) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M2.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M2.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M2.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M3.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M3.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M3.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA2M23DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA2M45DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x6C) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M4.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M4.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M4.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M5.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M5.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M5.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA2M45DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA2M67DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x6D) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M6.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M6.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M6.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M7.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M7.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M7.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA2M67DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteA2M89DAT()
        {
            const Byte LEN = 19;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x6E) && (rxBuff(2) == 0x0E))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b3 = 0;
                myUIT.b2 = meCRC[5];
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                DEV.a2mxTable.A2M8.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[7];
                myUIT.b0 = meCRC[6];
                DEV.a2mxTable.A2M8.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[9];
                myUIT.b0 = meCRC[8];
                DEV.a2mxTable.A2M8.angleHigh = myUIT.us0;

                myUIT.b3 = 0;
                myUIT.b2 = meCRC[12];
                myUIT.b1 = meCRC[11];
                myUIT.b0 = meCRC[10];
                DEV.a2mxTable.A2M9.torquePre = myUIT.ui;

                myUIT.b1 = meCRC[14];
                myUIT.b0 = meCRC[13];
                DEV.a2mxTable.A2M9.angleLow = myUIT.us0;

                myUIT.b1 = meCRC[16];
                myUIT.b0 = meCRC[15];
                DEV.a2mxTable.A2M9.angleHigh = myUIT.us0;

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteA2M89DAT();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteRECSIZE()
        {
            long tick = System.DateTime.Now.Ticks;

            const Byte LEN = 7;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x71) && (rxBuff(2) == 0x02))
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //帧长度
            if (rxCount < LEN)
            {
                return;
            }

            //校验
            for (byte i = 0; i < (LEN - 2); i++)
            {
                meCRC[i] = rxBuff(i);
            }

            myUIT.b0 = rxBuff(LEN - 2);
            myUIT.b1 = rxBuff(LEN - 1);
            if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
            {
                myUIT.b1 = meCRC[4];
                myUIT.b0 = meCRC[3];
                if (myUIT.us0 > 0)
                {
                    DEV.queueSize = myUIT.us0;
                    DEV.queueArray = new RECDAT[DEV.queueSize];
                    DEV.queuePercent = 1;
                    //初始化第一个index=0xFF方便检测是否读完
                    myTime = new DateTime(tick);
                    DEV.queueArray[0] = new RECDAT();
                    DEV.queueArray[0].index = 0xFF;
                    DEV.queueArray[0].stamp = myTime.ToString("HHmmssfff");
                    //初始化index=0方便检测完整性
                    for (UInt16 i = 1; i < DEV.queueSize; i++)
                    {
                        //
                        myTime = new DateTime(tick);
                        //
                        DEV.queueArray[i] = new RECDAT();
                        DEV.queueArray[i].index = 0;
                        DEV.queueArray[i].stamp = myTime.ToString("HHmmssfff");
                        //
                        tick -= 10000;
                    }
                }
                else
                {
                    DEV.queueSize = 0;
                    DEV.queuePercent = 0;
                }

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteRECSIZE();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //网口接收
        private void nePort_ReceiveWriteRECDAT()
        {
            Byte LEN; //0x07和0x0D两种
            UInt16 idx;

            //读
            for (int i = 0; i < bytesRead; i++)
            {
                meRXD[rxWrite] = neRXD[i];

                rxBuff_Write_Inc();
            }

            //
            while (rxCount >= (rxBuff(2) + 5))
            {
                //长度17的
                if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x72) && (rxBuff(2) == 0x0C))
                {
                    LEN = 17;

                    //校验
                    for (byte i = 0; i < (LEN - 2); i++)
                    {
                        meCRC[i] = rxBuff(i);
                    }

                    myUIT.b0 = rxBuff((Int16)(LEN - 2));
                    myUIT.b1 = rxBuff((Int16)(LEN - 1));
                    if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
                    {
                        myUIT.b1 = meCRC[4];
                        myUIT.b0 = meCRC[3];
                        idx = myUIT.us0;

                        DEV.queuePercent = 100 - (idx * 99 / DEV.queueSize);
                        DEV.queueArray[idx].index = idx;
                        DEV.queueArray[idx + 1].index = (UInt16)(idx + 1);

                        myUIT.b3 = 0;
                        myUIT.b2 = meCRC[7];
                        myUIT.b1 = meCRC[6];
                        myUIT.b0 = meCRC[5];
                        DEV.queueArray[idx + 1].torque = myUIT.ui;

                        myUIT.b1 = meCRC[9];
                        myUIT.b0 = meCRC[8];
                        DEV.queueArray[idx + 1].angle = myUIT.s0;

                        myUIT.b3 = 0;
                        myUIT.b2 = meCRC[12];
                        myUIT.b1 = meCRC[11];
                        myUIT.b0 = meCRC[10];
                        DEV.queueArray[idx].torque = myUIT.ui;

                        myUIT.b1 = meCRC[14];
                        myUIT.b0 = meCRC[13];
                        DEV.queueArray[idx].angle = myUIT.s0;

                        //
                        rxBuff_Read_Inc(LEN);
                    }
                    else
                    {
                        rxBuff_Read_Inc();
                    }
                }
                //长度12的
                else if ((rxBuff(0) == 0x06) && (rxBuff(1) == 0x72) && (rxBuff(2) == 0x07))
                {
                    LEN = 12;

                    //校验
                    for (byte i = 0; i < (LEN - 2); i++)
                    {
                        meCRC[i] = rxBuff(i);
                    }

                    myUIT.b0 = rxBuff((Int16)(LEN - 2));
                    myUIT.b1 = rxBuff((Int16)(LEN - 1));
                    if (myUIT.us0 == AP_CRC16_MODBUS(meCRC, LEN - 2, true))
                    {
                        myUIT.b1 = meCRC[4];
                        myUIT.b0 = meCRC[3];
                        idx = myUIT.us0;

                        DEV.queueArray[idx].index = idx;

                        myUIT.b3 = 0;
                        myUIT.b2 = meCRC[7];
                        myUIT.b1 = meCRC[6];
                        myUIT.b0 = meCRC[5];
                        DEV.queueArray[idx].torque = myUIT.ui;

                        myUIT.b1 = meCRC[9];
                        myUIT.b0 = meCRC[8];
                        DEV.queueArray[idx].angle = myUIT.s0;

                        //
                        rxBuff_Read_Inc(LEN);
                    }
                    else
                    {
                        rxBuff_Read_Inc();
                    }
                }
                //校验不对
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }
        #endregion

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            int bytesSent = handler.EndSend(ar);
            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();
        }

        #region 网口发送
        //发送
        public bool nePort_Read_Heart()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_HEART;
                //
                meTXD[0] = myCMD.CMD_READ_HEART[0];
                meTXD[1] = myCMD.CMD_READ_HEART[1];
                meTXD[2] = myCMD.CMD_READ_HEART[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                //
                count = 0;
                elapse = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A1M01DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A1M01DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M01DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M01DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M01DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A1M23DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A1M23DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M23DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M23DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M23DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A1M45DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A1M45DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M45DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M45DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M45DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A1M67DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A1M67DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M67DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M67DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M67DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A1M89DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A1M89DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M89DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M89DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M89DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A2M01DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A2M01DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M01DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M01DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M01DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A2M23DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A2M23DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M23DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M23DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M23DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A2M45DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A2M45DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M45DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M45DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M45DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A2M67DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A2M67DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M67DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M67DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M67DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_A2M89DAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_READ_A2M89DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M89DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M89DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M89DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_Zero()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_ZERO;
                //
                meTXD[0] = myCMD.CMD_WRITE_ZERO[0];
                meTXD[1] = myCMD.CMD_WRITE_ZERO[1];
                meTXD[2] = myCMD.CMD_WRITE_ZERO[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_OFF()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_OFF;
                //
                meTXD[0] = myCMD.CMD_WRITE_OFF[0];
                meTXD[1] = myCMD.CMD_WRITE_OFF[1];
                meTXD[2] = myCMD.CMD_WRITE_OFF[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 5, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_PARA(Byte modePt, Byte modeAx, Byte modeMx, Byte torqueUnit, Byte angleSpeed)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_PARA;
                //
                meTXD[0] = myCMD.CMD_WRITE_PARA[0];
                meTXD[1] = myCMD.CMD_WRITE_PARA[1];
                meTXD[2] = myCMD.CMD_WRITE_PARA[2];
                //
                meTXD[3] = modeAx;
                meTXD[4] = modePt;
                meTXD[5] = modeMx;
                meTXD[6] = torqueUnit;
                meTXD[7] = angleSpeed;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 8, true);
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 10, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_RECMODE(Byte modeRec)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_RECMODE;
                //
                meTXD[0] = myCMD.CMD_WRITE_RECMODE[0];
                meTXD[1] = myCMD.CMD_WRITE_RECMODE[1];
                meTXD[2] = myCMD.CMD_WRITE_RECMODE[2];
                //
                meTXD[3] = modeRec;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 4, true);
                meTXD[4] = myUIT.b0;
                meTXD[5] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 6, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A1M01DAT(UInt32 target, UInt32 low, UInt32 high)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A1M01DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A1M01DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A1M01DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A1M01DAT[2];
                //
                myUIT.ui = target;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.ui = low;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                meTXD[8] = myUIT.b2;
                myUIT.ui = high;
                meTXD[9] = myUIT.b0;
                meTXD[10] = myUIT.b1;
                meTXD[11] = myUIT.b2;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 12, true);
                meTXD[12] = myUIT.b0;
                meTXD[13] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 14, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A1M23DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A1M23DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A1M23DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A1M23DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A1M23DAT[2];
                //
                myUIT.ui = lowA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.ui = highA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                meTXD[8] = myUIT.b2;
                myUIT.ui = lowB;
                meTXD[9] = myUIT.b0;
                meTXD[10] = myUIT.b1;
                meTXD[11] = myUIT.b2;
                myUIT.ui = highB;
                meTXD[12] = myUIT.b0;
                meTXD[13] = myUIT.b1;
                meTXD[14] = myUIT.b2;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 15, true);
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 17, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A1M45DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A1M45DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A1M45DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A1M45DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A1M45DAT[2];
                //
                myUIT.ui = lowA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.ui = highA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                meTXD[8] = myUIT.b2;
                myUIT.ui = lowB;
                meTXD[9] = myUIT.b0;
                meTXD[10] = myUIT.b1;
                meTXD[11] = myUIT.b2;
                myUIT.ui = highB;
                meTXD[12] = myUIT.b0;
                meTXD[13] = myUIT.b1;
                meTXD[14] = myUIT.b2;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 15, true);
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 17, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A1M67DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A1M67DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A1M67DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A1M67DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A1M67DAT[2];
                //
                myUIT.ui = lowA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.ui = highA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                meTXD[8] = myUIT.b2;
                myUIT.ui = lowB;
                meTXD[9] = myUIT.b0;
                meTXD[10] = myUIT.b1;
                meTXD[11] = myUIT.b2;
                myUIT.ui = highB;
                meTXD[12] = myUIT.b0;
                meTXD[13] = myUIT.b1;
                meTXD[14] = myUIT.b2;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 15, true);
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 17, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A1M89DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A1M89DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A1M89DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A1M89DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A1M89DAT[2];
                //
                myUIT.ui = lowA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.ui = highA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                meTXD[8] = myUIT.b2;
                myUIT.ui = lowB;
                meTXD[9] = myUIT.b0;
                meTXD[10] = myUIT.b1;
                meTXD[11] = myUIT.b2;
                myUIT.ui = highB;
                meTXD[12] = myUIT.b0;
                meTXD[13] = myUIT.b1;
                meTXD[14] = myUIT.b2;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 15, true);
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 17, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A2M01DAT(UInt32 presetA, UInt16 target, UInt32 presetB, UInt16 low, UInt16 high)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A2M01DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A2M01DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A2M01DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A2M01DAT[2];
                //
                myUIT.ui = presetA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.us0 = target;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                myUIT.ui = presetB;
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                meTXD[10] = myUIT.b2;
                myUIT.us0 = low;
                meTXD[11] = myUIT.b0;
                meTXD[12] = myUIT.b1;
                myUIT.us0 = high;
                meTXD[13] = myUIT.b0;
                meTXD[14] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 15, true);
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 17, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A2M23DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A2M23DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A2M23DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A2M23DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A2M23DAT[2];
                //
                myUIT.ui = presetA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.us0 = lowA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                myUIT.us0 = highA;
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                myUIT.ui = presetB;
                meTXD[10] = myUIT.b0;
                meTXD[11] = myUIT.b1;
                meTXD[12] = myUIT.b2;
                myUIT.us0 = lowB;
                meTXD[13] = myUIT.b0;
                meTXD[14] = myUIT.b1;
                myUIT.us0 = highB;
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 17, true);
                meTXD[17] = myUIT.b0;
                meTXD[18] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 19, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A2M45DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A2M45DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A2M45DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A2M45DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A2M45DAT[2];
                //
                myUIT.ui = presetA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.us0 = lowA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                myUIT.us0 = highA;
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                myUIT.ui = presetB;
                meTXD[10] = myUIT.b0;
                meTXD[11] = myUIT.b1;
                meTXD[12] = myUIT.b2;
                myUIT.us0 = lowB;
                meTXD[13] = myUIT.b0;
                meTXD[14] = myUIT.b1;
                myUIT.us0 = highB;
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 17, true);
                meTXD[17] = myUIT.b0;
                meTXD[18] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 19, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A2M67DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A2M67DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A2M67DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A2M67DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A2M67DAT[2];
                //
                myUIT.ui = presetA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.us0 = lowA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                myUIT.us0 = highA;
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                myUIT.ui = presetB;
                meTXD[10] = myUIT.b0;
                meTXD[11] = myUIT.b1;
                meTXD[12] = myUIT.b2;
                myUIT.us0 = lowB;
                meTXD[13] = myUIT.b0;
                meTXD[14] = myUIT.b1;
                myUIT.us0 = highB;
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 17, true);
                meTXD[17] = myUIT.b0;
                meTXD[18] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 19, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Write_A2M89DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_A2M89DAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_A2M89DAT[0];
                meTXD[1] = myCMD.CMD_WRITE_A2M89DAT[1];
                meTXD[2] = myCMD.CMD_WRITE_A2M89DAT[2];
                //
                myUIT.ui = presetA;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                meTXD[5] = myUIT.b2;
                myUIT.us0 = lowA;
                meTXD[6] = myUIT.b0;
                meTXD[7] = myUIT.b1;
                myUIT.us0 = highA;
                meTXD[8] = myUIT.b0;
                meTXD[9] = myUIT.b1;
                myUIT.ui = presetB;
                meTXD[10] = myUIT.b0;
                meTXD[11] = myUIT.b1;
                meTXD[12] = myUIT.b2;
                myUIT.us0 = lowB;
                meTXD[13] = myUIT.b0;
                meTXD[14] = myUIT.b1;
                myUIT.us0 = highB;
                meTXD[15] = myUIT.b0;
                meTXD[16] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 17, true);
                meTXD[17] = myUIT.b0;
                meTXD[18] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 19, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_RECSIZE()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_RECSIZE;
                //
                meTXD[0] = myCMD.CMD_WRITE_RECSIZE[0];
                meTXD[1] = myCMD.CMD_WRITE_RECSIZE[1];
                meTXD[2] = myCMD.CMD_WRITE_RECSIZE[2];
                //
                meTXD[3] = 0;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 4, true);
                meTXD[4] = myUIT.b0;
                meTXD[5] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 6, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Clear_RECSIZE()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_RECSIZE;
                //
                meTXD[0] = myCMD.CMD_WRITE_RECSIZE[0];
                meTXD[1] = myCMD.CMD_WRITE_RECSIZE[1];
                meTXD[2] = myCMD.CMD_WRITE_RECSIZE[2];
                //
                meTXD[3] = 1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 4, true);
                meTXD[4] = myUIT.b0;
                meTXD[5] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 6, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_RECDAT()
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_RECDAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_RECDAT[0];
                meTXD[1] = myCMD.CMD_WRITE_RECDAT[1];
                meTXD[2] = myCMD.CMD_WRITE_RECDAT[2];
                //
                meTXD[3] = 0xFF;
                meTXD[4] = 0xFF;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 5, true);
                meTXD[5] = myUIT.b0;
                meTXD[6] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 7, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool nePort_Read_RECDAT(UInt16 idx)
        {
            //网口发送
            if (clientConnectionItems.Count > 0)
            {
                //
                rtCOM = RTCOM.NET_WRITE_RECDAT;
                //
                meTXD[0] = myCMD.CMD_WRITE_RECDAT[0];
                meTXD[1] = myCMD.CMD_WRITE_RECDAT[1];
                meTXD[2] = myCMD.CMD_WRITE_RECDAT[2];
                //
                myUIT.us0 = idx;
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 5, true);
                meTXD[5] = myUIT.b0;
                meTXD[6] = myUIT.b1;
                //
                string str = "";
                try
                {
                    foreach (var socket in clientConnectionItems)
                    {
                        str = socket.Key;
                        socket.Value.BeginSend(meTXD, 0, 7, 0, new AsyncCallback(SendCallback), socket.Value);
                    }
                }
                catch
                {
                    closeConnection(str);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
