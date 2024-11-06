using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace BEM4
{
    public partial class XET
    {
        public void debugShow(Byte[] dat, int len)
        {
            String txt = "";

            for (int i = 0; i < len; i++)
            {
                txt += dat[i].ToString("X2");
                txt += " ";
            }
            MessageBox.Show(txt);
        }

        private void rxBuff_Read_Inc()
        {
            rxRead++;
            rxCount--;

            while (rxRead >= CMD.RxSize)
            {
                rxRead -= CMD.RxSize;
            }
        }

        private void rxBuff_Read_Inc(Int16 num)
        {
            rxRead += num;
            rxCount -= num;

            while (rxRead >= CMD.RxSize)
            {
                rxRead -= CMD.RxSize;
            }
        }

        private void rxBuff_Write_Inc()
        {
            rxWrite++;
            rxCount++;

            while (rxWrite >= CMD.RxSize)
            {
                rxWrite -= CMD.RxSize;
            }

            if (rxRead == rxWrite)
            {
                rxBuff_Read_Inc();
            }
        }

        private Byte rxBuff(Int16 idx)
        {
            idx += rxRead;

            if (idx >= CMD.RxSize)
            {
                return meRXD[idx - CMD.RxSize];
            }
            else
            {
                return meRXD[idx];
            }
        }

        private UInt32 getMinimaTorque(UInt32 cap)
        {
            // console.log("word_inlb_from_NM(600   )", parseInt(600.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(1200  )", parseInt(1200.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(2000  )", parseInt(2000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(3000  )", parseInt(3000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(6000  )", parseInt(6000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(8500  )", parseInt(8500.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(13500 )", parseInt(13500.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(20000 )", parseInt(20000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(34000 )", parseInt(34000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(40000 )", parseInt(40000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(55000 )", parseInt(55000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(85000 )", parseInt(85000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(100000)", parseInt(100000.0 * 8.85075 + 0.5))
            // console.log("word_ftlb_from_NM(600   )", parseInt(600.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(1200  )", parseInt(1200.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(2000  )", parseInt(2000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(3000  )", parseInt(3000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(6000  )", parseInt(6000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(8500  )", parseInt(8500.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(13500 )", parseInt(13500.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(20000 )", parseInt(20000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(34000 )", parseInt(34000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(40000 )", parseInt(40000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(55000 )", parseInt(55000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(85000 )", parseInt(85000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(100000)", parseInt(100000.0 * 0.73756214927727 + 0.5))
            // console.log("word_kgcm_from_NM(600   )", parseInt(600.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(1200  )", parseInt(1200.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(2000  )", parseInt(2000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(3000  )", parseInt(3000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(6000  )", parseInt(6000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(8500  )", parseInt(8500.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(13500 )", parseInt(13500.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(20000 )", parseInt(20000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(34000 )", parseInt(34000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(40000 )", parseInt(40000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(55000 )", parseInt(55000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(85000 )", parseInt(85000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(100000)", parseInt(100000.0 * 10.1971621 + 0.5))
            // console.log("word_inlb_from_NM(30  )", parseInt(30.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(60  )", parseInt(60.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(100 )", parseInt(100.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(150 )", parseInt(150.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(300 )", parseInt(300.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(420 )", parseInt(420.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(680 )", parseInt(680.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(1000)", parseInt(1000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(1700)", parseInt(1700.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(2000)", parseInt(2000.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(2500)", parseInt(2500.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(4250)", parseInt(4250.0 * 8.85075 + 0.5))
            // console.log("word_inlb_from_NM(5000)", parseInt(5000.0 * 8.85075 + 0.5))
            // console.log("word_ftlb_from_NM(30  )", parseInt(30.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(60  )", parseInt(60.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(100 )", parseInt(100.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(150 )", parseInt(150.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(300 )", parseInt(300.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(420 )", parseInt(420.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(680 )", parseInt(680.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(1000)", parseInt(1000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(1700)", parseInt(1700.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(2000)", parseInt(2000.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(2500)", parseInt(2500.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(4250)", parseInt(4250.0 * 0.73756214927727 + 0.5))
            // console.log("word_ftlb_from_NM(5000)", parseInt(5000.0 * 0.73756214927727 + 0.5))
            // console.log("word_kgcm_from_NM(30  )", parseInt(30.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(60  )", parseInt(60.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(100 )", parseInt(100.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(150 )", parseInt(150.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(300 )", parseInt(300.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(420 )", parseInt(420.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(680 )", parseInt(680.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(1000)", parseInt(1000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(1700)", parseInt(1700.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(2000)", parseInt(2000.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(2500)", parseInt(2500.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(4250)", parseInt(4250.0 * 10.1971621 + 0.5))
            // console.log("word_kgcm_from_NM(5000)", parseInt(5000.0 * 10.1971621 + 0.5))
            // {28,30,300,600},
            // {55,60,600,1200},
            // {80,100,1000,2000},
            // {120,150,1500,3000},
            // {200,300,3000,6000},
            // {300,420,4250,8500},
            // {500,680,6750,13500},
            // {500,1000,10000,20000},
            // {1000,1700,17000,34000},
            // {1500,2000,20000,40000},
            // {2000,2500,27500,55000},
            // {3500,4250,42500,85000},
            // {3500,5000,50000,100000}
            switch (cap)
            {
                //Nm;
                default:
                case 600:
                    return 30;
                case 1200:
                    return 60;
                case 2000:
                    return 100;
                case 3000:
                    return 150;
                case 6000:
                    return 300;
                case 8500:
                    return 420;
                case 13500:
                    return 680;
                case 20000:
                    return 1000;
                case 34000:
                    return 1700;
                case 40000:
                    return 2000;
                case 55000:
                    return 2500;
                case 85000:
                    return 4250;
                case 100000:
                    return 5000;
                //inlb;
                case 5310:
                    return 266;
                case 10621:
                    return 531;
                case 17702:
                    return 885;
                case 26552:
                    return 1328;
                case 53105:
                    return 2655;
                case 75231:
                    return 3717;
                case 119485:
                    return 6019;
                case 177015:
                    return 8851;
                case 300926:
                    return 15046;
                case 354030:
                    return 17702;
                case 486791:
                    return 22127;
                case 752314:
                    return 37616;
                case 885075:
                    return 44254;
                //ftlb;
                case 443:
                    return 22;
                case 885:
                    return 44;
                case 1475:
                    return 74;
                case 2213:
                    return 111;
                case 4425:
                    return 221;
                case 6269:
                    return 310;
                case 9957:
                    return 502;
                case 14751:
                    return 738;
                case 25077:
                    return 1254;
                case 29502:
                    return 1475;
                case 40566:
                    return 1844;
                case 62693:
                    return 3135;
                case 73756:
                    return 3688;
                //kgcm;
                case 6118:
                    return 306;
                case 12237:
                    return 612;
                case 20394:
                    return 1020;
                case 30591:
                    return 1530;
                case 61183:
                    return 3059;
                case 86676:
                    return 4283;
                case 137662:
                    return 6934;
                case 203943:
                    return 10197;
                case 346704:
                    return 17335;
                case 407886:
                    return 20394;
                case 560844:
                    return 25493;
                case 866759:
                    return 43338;
                case 1019716:
                    return 50986;
            }
        }

        private void getDeviceOpsn()
        {
            //
            if ((oldAx != DEV.modeAx) || (oldMx != DEV.modeMx) || (oldTU != DEV.torqueUnit) || DEV.isUnit)
            {
                oldAx = DEV.modeAx;
                oldMx = DEV.modeMx;

                if (oldTU != DEV.torqueUnit)
                {
                    DEV.isUnit = true;
                    oldTU = DEV.torqueUnit;
                }
                else
                {
                    DEV.isUnit = false;
                }

                switch (DEV.torqueUnit)
                {
                    case UNIT.UNIT_nm: strUNIT = "N·m"; break;
                    case UNIT.UNIT_lbfin: strUNIT = "lbf·in"; break;
                    case UNIT.UNIT_lbfft: strUNIT = "lbf·ft"; break;
                    case UNIT.UNIT_kgcm: strUNIT = "kgf·cm"; break;
                }

                switch (DEV.modeAx)
                {
                    case 0: //A1
                        switch (DEV.modeMx)
                        {
                            case 0:
                                DEV.torqueTarget = DEV.a1mxTable.A1M0.torqueTarget;
                                strAXMX = "A1M0," + (DEV.torqueTarget / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1M0;
                                break;
                            case 1:
                                DEV.torqueLow = DEV.a1mxTable.A1M1.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M1.torqueHigh;
                                strAXMX = "A1M1," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 2:
                                DEV.torqueLow = DEV.a1mxTable.A1M2.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M2.torqueHigh;
                                strAXMX = "A1M2," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 3:
                                DEV.torqueLow = DEV.a1mxTable.A1M3.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M3.torqueHigh;
                                strAXMX = "A1M3," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 4:
                                DEV.torqueLow = DEV.a1mxTable.A1M4.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M4.torqueHigh;
                                strAXMX = "A1M4," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 5:
                                DEV.torqueLow = DEV.a1mxTable.A1M5.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M5.torqueHigh;
                                strAXMX = "A1M5," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 6:
                                DEV.torqueLow = DEV.a1mxTable.A1M6.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M6.torqueHigh;
                                strAXMX = "A1M6," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 7:
                                DEV.torqueLow = DEV.a1mxTable.A1M7.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M7.torqueHigh;
                                strAXMX = "A1M7," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 8:
                                DEV.torqueLow = DEV.a1mxTable.A1M8.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M8.torqueHigh;
                                strAXMX = "A1M8," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                            case 9:
                                DEV.torqueLow = DEV.a1mxTable.A1M9.torqueLow;
                                DEV.torqueHigh = DEV.a1mxTable.A1M9.torqueHigh;
                                strAXMX = "A1M9," + (DEV.torqueLow / 100.0f).ToString() + "~" + (DEV.torqueHigh / 100.0f).ToString() + strUNIT;
                                axmx = MODE.A1MX;
                                break;
                        }
                        break;
                    case 1: //A2
                        switch (DEV.modeMx)
                        {
                            case 0:
                                DEV.torquePre = DEV.a2mxTable.A2M0.torquePre;
                                DEV.angleTarget = DEV.a2mxTable.A2M0.angleTarget;
                                strAXMX = "A2M0," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleTarget / 10.0f) + "°";
                                axmx = MODE.A2M0;
                                break;
                            case 1:
                                DEV.torquePre = DEV.a2mxTable.A2M1.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M1.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M1.angleHigh;
                                strAXMX = "A2M1," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 2:
                                DEV.torquePre = DEV.a2mxTable.A2M2.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M2.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M2.angleHigh;
                                strAXMX = "A2M2," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 3:
                                DEV.torquePre = DEV.a2mxTable.A2M3.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M3.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M3.angleHigh;
                                strAXMX = "A2M3," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 4:
                                DEV.torquePre = DEV.a2mxTable.A2M4.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M4.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M4.angleHigh;
                                strAXMX = "A2M4," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 5:
                                DEV.torquePre = DEV.a2mxTable.A2M5.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M5.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M5.angleHigh;
                                strAXMX = "A2M5," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 6:
                                DEV.torquePre = DEV.a2mxTable.A2M6.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M6.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M6.angleHigh;
                                strAXMX = "A2M6," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 7:
                                DEV.torquePre = DEV.a2mxTable.A2M7.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M7.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M7.angleHigh;
                                strAXMX = "A2M7," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 8:
                                DEV.torquePre = DEV.a2mxTable.A2M8.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M8.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M8.angleHigh;
                                strAXMX = "A2M8," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                            case 9:
                                DEV.torquePre = DEV.a2mxTable.A2M9.torquePre;
                                DEV.angleLow = DEV.a2mxTable.A2M9.angleLow;
                                DEV.angleHigh = DEV.a2mxTable.A2M9.angleHigh;
                                strAXMX = "A2M9," + (DEV.torquePre / 100.0f).ToString() + strUNIT + "," + (DEV.angleLow / 10.0f).ToString() + "~" + (DEV.angleHigh / 10.0f).ToString() + "°";
                                axmx = MODE.A2MX;
                                break;
                        }
                        break;
                }
            }

            //Track模式下
            if (DEV.modePt == 0)
            {
                if (DEV.torque > 0)
                {
                    //开始扭矩
                    if (DEV.torqueOld == 0)
                    {
                        snDate = System.DateTime.Now.ToString("yyMMdd");
                        snBat++;
                        DEV.opsn = snDate + snBat.ToString("").PadLeft(4, '0');
                        DEV.info = "Track," + strAXMX;
                    }
                    //扭矩中
                    else
                    {
                    }
                }
                else
                {
                    //结束扭矩
                    if (DEV.torqueOld > 0)
                    {
                    }
                    //静止中
                    else
                    {
                        DEV.opsn = "";
                    }
                }
            }
            //Peak模式下
            else
            {
                if (DEV.torquePeak > 0)
                {
                    //开始扭矩
                    if (DEV.torquePeakOld == 0)
                    {
                        snDate = System.DateTime.Now.ToString("yyMMdd");
                        snBat++;
                        DEV.opsn = snDate + snBat.ToString("").PadLeft(4, '0');
                        DEV.info = "Peak," + strAXMX;
                    }
                    //扭矩中
                    else
                    {
                        if (DEV.torque > 0)//有数据
                        {
                            DEV.opsn = snDate + snBat.ToString("").PadLeft(4, '0');
                            DEV.info = "Peak," + strAXMX;
                        }
                        else
                        {
                            DEV.opsn = "";
                        }
                    }
                }
                else
                {
                    //结束扭矩
                    if (DEV.torquePeakOld > 0)
                    {
                        num_clear++;
                    }
                    //静止中
                    else
                    {
                        DEV.opsn = "";
                    }
                }
            }
        }

        #region
        //串口接收
        private void mePort_ReceiveReadHeart()
        {
            //50ms间隔回复65个数据,共1170字节
            long tick = System.DateTime.Now.Ticks;

            //一帧长度18字节
            const Byte LEN = 18;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA1M01DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA1M23DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA1M45DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA1M67DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA1M89DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA2M01DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA2M23DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA2M45DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA2M67DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveReadA2M89DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWritePARA()
        {
            const Byte LEN = 10;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteRECMODE()
        {
            const Byte LEN = 6;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA1M01DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA1M23DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA1M45DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA1M67DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA1M89DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA2M01DAT()
        {
            const Byte LEN = 17;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA2M23DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA2M45DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA2M67DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteA2M89DAT()
        {
            const Byte LEN = 19;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteRECSIZE()
        {
            long tick = System.DateTime.Now.Ticks;

            const Byte LEN = 7;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收
        private void mePort_ReceiveWriteRECDAT()
        {
            Byte LEN; //0x07和0x0D两种
            UInt16 idx;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

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

        //串口接收 解绑
        private void mePort_ReceiveWriteUNBIND()
        {
            const Byte LEN = 6;

            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if ((rxBuff(0) == 0x23) && (rxBuff(1) == 0x52) && (rxBuff(2) == 0x26) && (rxBuff(3) == 0x45))
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
            if (rxBuff(LEN - 1) == 0x23)
            {
                if(rxBuff(LEN - 2) == 1)
                {
                    MyDefine.myXET.IsUnbind = true;
                }
                else
                {
                    MyDefine.myXET.IsUnbind = false;
                }

                //
                rxBuff_Read_Inc(LEN);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveWriteUNBIND();
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        #endregion

        //接收触发函数,实际会由串口线程创建
        private void mePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (rtCOM)
            {
                case RTCOM.COM_READ_HEART:
                    mePort_ReceiveReadHeart();
                    break;
                case RTCOM.COM_READ_A1M01DAT:
                    mePort_ReceiveReadA1M01DAT();
                    break;
                case RTCOM.COM_READ_A1M23DAT:
                    mePort_ReceiveReadA1M23DAT();
                    break;
                case RTCOM.COM_READ_A1M45DAT:
                    mePort_ReceiveReadA1M45DAT();
                    break;
                case RTCOM.COM_READ_A1M67DAT:
                    mePort_ReceiveReadA1M67DAT();
                    break;
                case RTCOM.COM_READ_A1M89DAT:
                    mePort_ReceiveReadA1M89DAT();
                    break;
                case RTCOM.COM_READ_A2M01DAT:
                    mePort_ReceiveReadA2M01DAT();
                    break;
                case RTCOM.COM_READ_A2M23DAT:
                    mePort_ReceiveReadA2M23DAT();
                    break;
                case RTCOM.COM_READ_A2M45DAT:
                    mePort_ReceiveReadA2M45DAT();
                    break;
                case RTCOM.COM_READ_A2M67DAT:
                    mePort_ReceiveReadA2M67DAT();
                    break;
                case RTCOM.COM_READ_A2M89DAT:
                    mePort_ReceiveReadA2M89DAT();
                    break;

                case RTCOM.COM_WRITE_PARA:
                    mePort_ReceiveWritePARA();
                    break;
                case RTCOM.COM_WRITE_RECMODE:
                    mePort_ReceiveWriteRECMODE();
                    break;
                case RTCOM.COM_WRITE_A1M01DAT:
                    mePort_ReceiveWriteA1M01DAT();
                    break;
                case RTCOM.COM_WRITE_A1M23DAT:
                    mePort_ReceiveWriteA1M23DAT();
                    break;
                case RTCOM.COM_WRITE_A1M45DAT:
                    mePort_ReceiveWriteA1M45DAT();
                    break;
                case RTCOM.COM_WRITE_A1M67DAT:
                    mePort_ReceiveWriteA1M67DAT();
                    break;
                case RTCOM.COM_WRITE_A1M89DAT:
                    mePort_ReceiveWriteA1M89DAT();
                    break;
                case RTCOM.COM_WRITE_A2M01DAT:
                    mePort_ReceiveWriteA2M01DAT();
                    break;
                case RTCOM.COM_WRITE_A2M23DAT:
                    mePort_ReceiveWriteA2M23DAT();
                    break;
                case RTCOM.COM_WRITE_A2M45DAT:
                    mePort_ReceiveWriteA2M45DAT();
                    break;
                case RTCOM.COM_WRITE_A2M67DAT:
                    mePort_ReceiveWriteA2M67DAT();
                    break;
                case RTCOM.COM_WRITE_A2M89DAT:
                    mePort_ReceiveWriteA2M89DAT();
                    break;

                case RTCOM.COM_WRITE_RECSIZE:
                    mePort_ReceiveWriteRECSIZE();
                    break;
                case RTCOM.COM_WRITE_RECDAT:
                    mePort_ReceiveWriteRECDAT();
                    break;

                case RTCOM.COM_WRITE_UNBIND:
                    mePort_ReceiveWriteUNBIND();
                    break;

                default:
                    break;
            }
        }

        //发送
        public bool mePort_Read_Heart()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_HEART;
                //
                meTXD[0] = myCMD.CMD_READ_HEART[0];
                meTXD[1] = myCMD.CMD_READ_HEART[1];
                meTXD[2] = myCMD.CMD_READ_HEART[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
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
        public bool mePort_Read_A1M01DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A1M01DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M01DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M01DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M01DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A1M23DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A1M23DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M23DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M23DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M23DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A1M45DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A1M45DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M45DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M45DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M45DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A1M67DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A1M67DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M67DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M67DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M67DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A1M89DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A1M89DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A1M89DAT[0];
                meTXD[1] = myCMD.CMD_READ_A1M89DAT[1];
                meTXD[2] = myCMD.CMD_READ_A1M89DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A2M01DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A2M01DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M01DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M01DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M01DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A2M23DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A2M23DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M23DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M23DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M23DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A2M45DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A2M45DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M45DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M45DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M45DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A2M67DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A2M67DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M67DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M67DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M67DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_A2M89DAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_READ_A2M89DAT;
                //
                meTXD[0] = myCMD.CMD_READ_A2M89DAT[0];
                meTXD[1] = myCMD.CMD_READ_A2M89DAT[1];
                meTXD[2] = myCMD.CMD_READ_A2M89DAT[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_Zero()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_ZERO;
                //
                meTXD[0] = myCMD.CMD_WRITE_ZERO[0];
                meTXD[1] = myCMD.CMD_WRITE_ZERO[1];
                meTXD[2] = myCMD.CMD_WRITE_ZERO[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_OFF()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_OFF;
                //
                meTXD[0] = myCMD.CMD_WRITE_OFF[0];
                meTXD[1] = myCMD.CMD_WRITE_OFF[1];
                meTXD[2] = myCMD.CMD_WRITE_OFF[2];
                //
                myUIT.us0 = AP_CRC16_MODBUS(meTXD, 3, true);
                meTXD[3] = myUIT.b0;
                meTXD[4] = myUIT.b1;
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_PARA(Byte modePt, Byte modeAx, Byte modeMx, Byte torqueUnit, Byte angleSpeed)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_PARA;
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
                mePort.Write(meTXD, 0, 10);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_RECMODE(Byte modeRec)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_RECMODE;
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
                mePort.Write(meTXD, 0, 6);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A1M01DAT(UInt32 target, UInt32 low, UInt32 high)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A1M01DAT;
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
                mePort.Write(meTXD, 0, 14);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A1M23DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A1M23DAT;
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
                mePort.Write(meTXD, 0, 17);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A1M45DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A1M45DAT;
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
                mePort.Write(meTXD, 0, 17);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A1M67DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A1M67DAT;
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
                mePort.Write(meTXD, 0, 17);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A1M89DAT(UInt32 lowA, UInt32 highA, UInt32 lowB, UInt32 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A1M89DAT;
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
                mePort.Write(meTXD, 0, 17);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A2M01DAT(UInt32 presetA, UInt16 target, UInt32 presetB, UInt16 low, UInt16 high)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A2M01DAT;
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
                mePort.Write(meTXD, 0, 17);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A2M23DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A2M23DAT;
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
                mePort.Write(meTXD, 0, 19);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A2M45DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A2M45DAT;
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
                mePort.Write(meTXD, 0, 19);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A2M67DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A2M67DAT;
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
                mePort.Write(meTXD, 0, 19);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Write_A2M89DAT(UInt32 presetA, UInt16 lowA, UInt16 highA, UInt32 presetB, UInt16 lowB, UInt16 highB)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_A2M89DAT;
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
                mePort.Write(meTXD, 0, 19);
                return true;
            }
            else
            {
                return false;
            }
        }

        //解绑
        public bool mePort_Write_UNBIND()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_UNBIND;
                //
                meTXD[0] = myCMD.CMD_WRITE_UNBIND[0];
                meTXD[1] = myCMD.CMD_WRITE_UNBIND[1];
                meTXD[2] = myCMD.CMD_WRITE_UNBIND[2];
                meTXD[3] = myCMD.CMD_WRITE_UNBIND[3];
                meTXD[4] = myCMD.CMD_WRITE_UNBIND[4];
                //
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_RECSIZE()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_RECSIZE;
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
                mePort.Write(meTXD, 0, 6);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Clear_RECSIZE()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_RECSIZE;
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
                mePort.Write(meTXD, 0, 6);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_RECDAT()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_RECDAT;
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
                mePort.Write(meTXD, 0, 7);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Read_RECDAT(UInt16 idx)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //
                rtCOM = RTCOM.COM_WRITE_RECDAT;
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
                mePort.Write(meTXD, 0, 7);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}