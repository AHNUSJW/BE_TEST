using BEM4W;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;

//20240920 Lumi

namespace BEM4
{
    public partial class MenuCalForm : Form
    {
        private DataGridViewTextBoxEditingControl CellEdit = null;

        Boolean isPARA = false;         //是否更新参数
        String unit = "";               //单位
        Byte mode = 0;                  //modeRec
        A1Table a1Table = new A1Table();//A1Mx表
        A2Table a2Table = new A2Table();//A2Mx表

        Boolean isImportCFG = false;    //是否写入配置

        public MenuCalForm()
        {
            InitializeComponent();
        }

        //
        private void data_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                Clipboard.SetText(((TextBox)sender).SelectedText.Trim()); //Ctrl+C 复制
            }

            if (e.KeyData == (Keys.Control | Keys.V))
            {
                ((TextBox)sender).SelectedText = Clipboard.GetText(); //Ctrl+V 粘贴
            }

            if (e.KeyData == (Keys.Control | Keys.X))
            {
                ((TextBox)sender).Cut(); //Ctrl+X 剪切
            }
        }

        //数据输入
        private void data_KeyPress(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字,负号,小数点和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '.') && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            //小数点只能出现1位
            if ((e.KeyChar == '.') && ((DataGridViewTextBoxEditingControl)sender).Text.Contains("."))
            {
                e.Handled = true;
                return;
            }

            //第一位不能为小数点
            if ((e.KeyChar == '.') && (((DataGridViewTextBoxEditingControl)sender).Text.Length == 0))
            {
                e.Handled = true;
                return;
            }
        }

        //table表键盘输入拦截
        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
            CellEdit.KeyPress += data_KeyPress;
        }

        private void receivePara()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(receivePara);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    //MessageBox.Show("MenuCalForm receiveData err 2");
                }
            }
            //本线程的操作请求
            else
            {
                switch (MyDefine.myXET.rtCOM)
                {
                    case RTCOM.COM_READ_HEART:
                        if (isPARA == false)
                        {
                            switch (MyDefine.myXET.DEV.modeRec)
                            {
                                case 0: radioButton1.Checked = true; break;
                                case 1: radioButton2.Checked = true; break;
                                case 2: radioButton3.Checked = true; break;
                            }
                        }
                        else if (MyDefine.myXET.DEV.queueSize == 0)
                        {
                            isPARA = false;
                            button3.BackColor = Color.Green;
                            MyDefine.myXET.mePort_Read_Heart();
                        }
                        break;

                    case RTCOM.NET_READ_HEART:
                        if (isPARA == false)
                        {
                            switch (MyDefine.myXET.DEV.modeRec)
                            {
                                case 0: radioButton1.Checked = true; break;
                                case 1: radioButton2.Checked = true; break;
                                case 2: radioButton3.Checked = true; break;
                            }
                        }
                        else if (MyDefine.myXET.DEV.queueSize == 0)
                        {
                            isPARA = false;
                            button3.BackColor = Color.Green;
                            MyDefine.myXET.nePort_Read_Heart();
                        }
                        break;

                    case RTCOM.COM_WRITE_RECSIZE:
                        isPARA = false;
                        button3.BackColor = Color.Green;
                        MyDefine.myXET.mePort_Read_Heart();
                        break;

                    case RTCOM.NET_WRITE_RECSIZE:
                        isPARA = false;
                        button3.BackColor = Color.Green;
                        MyDefine.myXET.nePort_Read_Heart();
                        break;

                    case RTCOM.COM_WRITE_RECMODE:
                        if (isPARA)
                        {
                            //需要更新参数
                            if (mode != MyDefine.myXET.DEV.modeRec)
                            {
                                MyDefine.myXET.mePort_Write_RECMODE(mode);
                            }
                            //不需要更新参数
                            else
                            {
                                isPARA = false;
                                MyDefine.myXET.mePort_Read_Heart();
                            }
                        }
                        break;

                    case RTCOM.NET_WRITE_RECMODE:
                        if (isPARA)
                        {
                            //需要更新参数
                            if (mode != MyDefine.myXET.DEV.modeRec)
                            {
                                MyDefine.myXET.nePort_Write_RECMODE(mode);
                            }
                            //不需要更新参数
                            else
                            {
                                isPARA = false;
                                MyDefine.myXET.nePort_Read_Heart();
                            }
                        }
                        break;

                    case RTCOM.COM_WRITE_A1M01DAT:
                        UpdateTableA1M0();
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M23";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M23";
                        }
                        MyDefine.myXET.mePort_Write_A1M23DAT(a1Table.A1M2.torqueLow, a1Table.A1M2.torqueHigh, a1Table.A1M3.torqueLow, a1Table.A1M3.torqueHigh);
                        break;
                    case RTCOM.NET_WRITE_A1M01DAT:
                        UpdateTableA1M0();
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M23";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M23";
                        }
                        MyDefine.myXET.nePort_Write_A1M23DAT(a1Table.A1M2.torqueLow, a1Table.A1M2.torqueHigh, a1Table.A1M3.torqueLow, a1Table.A1M3.torqueHigh);
                        break;

                    case RTCOM.COM_WRITE_A1M23DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M45";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M45";
                        }
                        MyDefine.myXET.mePort_Write_A1M45DAT(a1Table.A1M4.torqueLow, a1Table.A1M4.torqueHigh, a1Table.A1M5.torqueLow, a1Table.A1M5.torqueHigh);
                        break;
                    case RTCOM.NET_WRITE_A1M23DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M45";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M45";
                        }
                        MyDefine.myXET.nePort_Write_A1M45DAT(a1Table.A1M4.torqueLow, a1Table.A1M4.torqueHigh, a1Table.A1M5.torqueLow, a1Table.A1M5.torqueHigh);
                        break;

                    case RTCOM.COM_WRITE_A1M45DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M67";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M67";
                        }
                        MyDefine.myXET.mePort_Write_A1M67DAT(a1Table.A1M6.torqueLow, a1Table.A1M6.torqueHigh, a1Table.A1M7.torqueLow, a1Table.A1M7.torqueHigh);
                        break;
                    case RTCOM.NET_WRITE_A1M45DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M67";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M67";
                        }
                        MyDefine.myXET.nePort_Write_A1M67DAT(a1Table.A1M6.torqueLow, a1Table.A1M6.torqueHigh, a1Table.A1M7.torqueLow, a1Table.A1M7.torqueHigh);
                        break;

                    case RTCOM.COM_WRITE_A1M67DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M89";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M89";
                        }
                        MyDefine.myXET.mePort_Write_A1M89DAT(a1Table.A1M8.torqueLow, a1Table.A1M8.torqueHigh, a1Table.A1M9.torqueLow, a1Table.A1M9.torqueHigh);
                        break;
                    case RTCOM.NET_WRITE_A1M67DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A1M89";
                        }
                        else
                        {
                            button_writeCFG.Text = "A1M89";
                        }
                        MyDefine.myXET.nePort_Write_A1M89DAT(a1Table.A1M8.torqueLow, a1Table.A1M8.torqueHigh, a1Table.A1M9.torqueLow, a1Table.A1M9.torqueHigh);
                        break;

                    case RTCOM.COM_WRITE_A1M89DAT:
                        UpdateTableA1MX();

                        if (!isImportCFG)
                        {
                            isPARA = false;
                            button1.Text = MyDefine.myXET.languageNum == 0 ? "更 新" : "Refresh";
                            button1.BackColor = Color.Green;
                            MyDefine.myXET.oldAx = 0xFF;
                            MyDefine.myXET.oldMx = 0xFF;
                            MyDefine.myXET.oldTU = 0x00;
                            MyDefine.myXET.mePort_Read_Heart();
                        }
                        else
                        {
                            setA2MXFromCFGTable();
                        }

                        break;

                    case RTCOM.NET_WRITE_A1M89DAT:
                        UpdateTableA1MX();

                        if (!isImportCFG)
                        {
                            isPARA = false;
                            button1.Text = MyDefine.myXET.languageNum == 0 ? "更 新" : "Refresh";
                            button1.BackColor = Color.Green;
                            MyDefine.myXET.oldAx = 0xFF;
                            MyDefine.myXET.oldMx = 0xFF;
                            MyDefine.myXET.oldTU = 0x00;
                            MyDefine.myXET.nePort_Read_Heart();
                        }
                        else
                        {
                            setA2MXFromCFGTable();
                        }
                        break;

                    case RTCOM.COM_WRITE_A2M01DAT:
                        UpdateTableA2M0();
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M23";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M23";
                        }
                        MyDefine.myXET.mePort_Write_A2M23DAT(a2Table.A2M2.torquePre, a2Table.A2M2.angleLow, a2Table.A2M2.angleHigh, a2Table.A2M3.torquePre, a2Table.A2M3.angleLow, a2Table.A2M3.angleHigh);
                        break;

                    case RTCOM.NET_WRITE_A2M01DAT:
                        UpdateTableA2M0();
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M23";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M23";
                        }
                        MyDefine.myXET.nePort_Write_A2M23DAT(a2Table.A2M2.torquePre, a2Table.A2M2.angleLow, a2Table.A2M2.angleHigh, a2Table.A2M3.torquePre, a2Table.A2M3.angleLow, a2Table.A2M3.angleHigh);
                        break;

                    case RTCOM.COM_WRITE_A2M23DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M45";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M45";
                        }
                        MyDefine.myXET.mePort_Write_A2M45DAT(a2Table.A2M4.torquePre, a2Table.A2M4.angleLow, a2Table.A2M4.angleHigh, a2Table.A2M5.torquePre, a2Table.A2M5.angleLow, a2Table.A2M5.angleHigh);
                        break;
                    case RTCOM.NET_WRITE_A2M23DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M45";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M45";
                        }
                        MyDefine.myXET.nePort_Write_A2M45DAT(a2Table.A2M4.torquePre, a2Table.A2M4.angleLow, a2Table.A2M4.angleHigh, a2Table.A2M5.torquePre, a2Table.A2M5.angleLow, a2Table.A2M5.angleHigh);
                        break;

                    case RTCOM.COM_WRITE_A2M45DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M67";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M67";
                        }
                        MyDefine.myXET.mePort_Write_A2M67DAT(a2Table.A2M6.torquePre, a2Table.A2M6.angleLow, a2Table.A2M6.angleHigh, a2Table.A2M7.torquePre, a2Table.A2M7.angleLow, a2Table.A2M7.angleHigh);
                        break;
                    case RTCOM.NET_WRITE_A2M45DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M67";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M67";
                        }
                        MyDefine.myXET.nePort_Write_A2M67DAT(a2Table.A2M6.torquePre, a2Table.A2M6.angleLow, a2Table.A2M6.angleHigh, a2Table.A2M7.torquePre, a2Table.A2M7.angleLow, a2Table.A2M7.angleHigh);
                        break;

                    case RTCOM.COM_WRITE_A2M67DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M89";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M89";
                        }
                        MyDefine.myXET.mePort_Write_A2M89DAT(a2Table.A2M8.torquePre, a2Table.A2M8.angleLow, a2Table.A2M8.angleHigh, a2Table.A2M9.torquePre, a2Table.A2M9.angleLow, a2Table.A2M9.angleHigh);
                        break;
                    case RTCOM.NET_WRITE_A2M67DAT:
                        if (!isImportCFG)
                        {
                            button1.Text = "A2M89";
                        }
                        else
                        {
                            button_writeCFG.Text = "A2M89";
                        }
                        MyDefine.myXET.nePort_Write_A2M89DAT(a2Table.A2M8.torquePre, a2Table.A2M8.angleLow, a2Table.A2M8.angleHigh, a2Table.A2M9.torquePre, a2Table.A2M9.angleLow, a2Table.A2M9.angleHigh);
                        break;

                    case RTCOM.COM_WRITE_A2M89DAT:
                        UpdateTableA2MX();
                        isPARA = false;
                        if (!isImportCFG)
                        {
                            button2.Text = MyDefine.myXET.languageNum == 0 ? "更 新" : "Refresh";
                            button2.BackColor = Color.Green;
                        }
                        else
                        {
                            isImportCFG = false;
                            button_writeCFG.Text = MyDefine.myXET.languageNum == 0 ? "写 入" : "Refresh";
                            button_writeCFG.BackColor = Color.Green;
                        }
                        MyDefine.myXET.oldAx = 0xFF;
                        MyDefine.myXET.oldMx = 0xFF;
                        MyDefine.myXET.oldTU = 0x00;
                        MyDefine.myXET.mePort_Read_Heart();
                        break;

                    case RTCOM.NET_WRITE_A2M89DAT:
                        UpdateTableA2MX();
                        isPARA = false;
                        if (!isImportCFG)
                        {
                            button2.Text = MyDefine.myXET.languageNum == 0 ? "更 新" : "Refresh";
                            button2.BackColor = Color.Green;
                        }
                        else
                        {
                            isImportCFG = false;
                            button_writeCFG.Text = MyDefine.myXET.languageNum == 0 ? "写 入" : "Refresh";
                            button_writeCFG.BackColor = Color.Green;
                        }
                        MyDefine.myXET.oldAx = 0xFF;
                        MyDefine.myXET.oldMx = 0xFF;
                        MyDefine.myXET.oldTU = 0x00;
                        MyDefine.myXET.nePort_Read_Heart();
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateTableA1M0()
        {
            dataGridView1.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M0.torqueTarget / 100.0f;
        }

        private void UpdateTableA1MX()
        {
            dataGridView2.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueLow / 100.0f;
            dataGridView2.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueHigh / 100.0f;
            dataGridView2.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueLow / 100.0f;
            dataGridView2.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueHigh / 100.0f;
            dataGridView2.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueLow / 100.0f;
            dataGridView2.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueHigh / 100.0f;
            dataGridView2.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueLow / 100.0f;
            dataGridView2.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueHigh / 100.0f;
            dataGridView2.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueLow / 100.0f;
            dataGridView2.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueHigh / 100.0f;
            dataGridView2.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueLow / 100.0f;
            dataGridView2.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueHigh / 100.0f;
            dataGridView2.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueLow / 100.0f;
            dataGridView2.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueHigh / 100.0f;
            dataGridView2.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueLow / 100.0f;
            dataGridView2.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueHigh / 100.0f;
            dataGridView2.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueLow / 100.0f;
            dataGridView2.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueHigh / 100.0f;
        }

        private void UpdateTableA2M0()
        {
            dataGridView3.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.torquePre / 100.0f;
            dataGridView3.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.angleTarget / 10.0f;
        }

        private void UpdateTableA2MX()
        {
            //
            dataGridView4.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.torquePre / 100.0f;
            dataGridView4.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleLow / 10.0f;
            dataGridView4.Rows[0].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleHigh / 10.0f;
            dataGridView4.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.torquePre / 100.0f;
            dataGridView4.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleLow / 10.0f;
            dataGridView4.Rows[1].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleHigh / 10.0f;
            dataGridView4.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.torquePre / 100.0f;
            dataGridView4.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleLow / 10.0f;
            dataGridView4.Rows[2].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleHigh / 10.0f;
            dataGridView4.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.torquePre / 100.0f;
            dataGridView4.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleLow / 10.0f;
            dataGridView4.Rows[3].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleHigh / 10.0f;
            dataGridView4.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.torquePre / 100.0f;
            dataGridView4.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleLow / 10.0f;
            dataGridView4.Rows[4].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleHigh / 10.0f;
            dataGridView4.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.torquePre / 100.0f;
            dataGridView4.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleLow / 10.0f;
            dataGridView4.Rows[5].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleHigh / 10.0f;
            dataGridView4.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.torquePre / 100.0f;
            dataGridView4.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleLow / 10.0f;
            dataGridView4.Rows[6].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleHigh / 10.0f;
            dataGridView4.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.torquePre / 100.0f;
            dataGridView4.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleLow / 10.0f;
            dataGridView4.Rows[7].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleHigh / 10.0f;
            dataGridView4.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.torquePre / 100.0f;
            dataGridView4.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleLow / 10.0f;
            dataGridView4.Rows[8].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleHigh / 10.0f;
        }

        private void UpdateTableCFG()
        {
            // 获取目录中最新的 .cfg 文件
            string directoryPath = MyDefine.myXET.userDEV;
            var directoryInfo = new DirectoryInfo(directoryPath);
            var latestFile = directoryInfo.GetFiles("*.cfg")
                                          .OrderByDescending(f => f.LastWriteTime)
                                          .FirstOrDefault();

            if (latestFile == null) return;

            // 读取最新的 .cfg 文件内容
            string filePath = latestFile.FullName;
            string json = File.ReadAllText(filePath);

            ImportDEVFormJson(filePath, json, false);
        }

        private void InitTableA1M0()
        {
            //属性
            dataGridView1.Size = new System.Drawing.Size(313, 83);
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 210;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView1.Columns[0].HeaderText = "模 式";
            //dataGridView1.Columns[1].HeaderText = "目标扭矩(" + unit + ")";
            dataGridView1.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "目标扭矩(" + unit + ")" : "target torque(" + unit + ")";

            //加行
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "A1 M0";
            dataGridView1.Rows[0].Cells[0].ReadOnly = true;
            dataGridView1.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M0.torqueTarget / 100.0f;
        }

        private void InitTableA1MX()
        {
            //属性
            dataGridView2.Size = new System.Drawing.Size(523, 403);
            dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView2.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView2.EnableHeadersVisualStyles = false;
            dataGridView2.ColumnHeadersHeight = 40;
            dataGridView2.RowTemplate.Height = 40;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToOrderColumns = false;
            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView2.Columns[0].Width = 100;
            dataGridView2.Columns[1].Width = 210;
            dataGridView2.Columns[2].Width = 210;
            dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView2.Columns[0].HeaderText = "模 式";
            //dataGridView2.Columns[1].HeaderText = "扭矩下限(" + unit + ")";
            //dataGridView2.Columns[2].HeaderText = "扭矩上限(" + unit + ")";
            dataGridView2.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "扭矩下限(" + unit + ")" : "torque lower limit(" + unit + ")";
            dataGridView2.Columns[2].HeaderText = MyDefine.myXET.languageNum == 0 ? "扭矩上限(" + unit + ")" : "torque upper limit(" + unit + ")";

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[0].Cells[0].Value = "A1 M1";
            dataGridView2.Rows[0].Cells[0].ReadOnly = true;
            dataGridView2.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueLow / 100.0f;
            dataGridView2.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[1].Cells[0].Value = "A1 M2";
            dataGridView2.Rows[1].Cells[0].ReadOnly = true;
            dataGridView2.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueLow / 100.0f;
            dataGridView2.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[2].Cells[0].Value = "A1 M3";
            dataGridView2.Rows[2].Cells[0].ReadOnly = true;
            dataGridView2.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueLow / 100.0f;
            dataGridView2.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[3].Cells[0].Value = "A1 M4";
            dataGridView2.Rows[3].Cells[0].ReadOnly = true;
            dataGridView2.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueLow / 100.0f;
            dataGridView2.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[4].Cells[0].Value = "A1 M5";
            dataGridView2.Rows[4].Cells[0].ReadOnly = true;
            dataGridView2.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueLow / 100.0f;
            dataGridView2.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[5].Cells[0].Value = "A1 M6";
            dataGridView2.Rows[5].Cells[0].ReadOnly = true;
            dataGridView2.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueLow / 100.0f;
            dataGridView2.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[6].Cells[0].Value = "A1 M7";
            dataGridView2.Rows[6].Cells[0].ReadOnly = true;
            dataGridView2.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueLow / 100.0f;
            dataGridView2.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[7].Cells[0].Value = "A1 M8";
            dataGridView2.Rows[7].Cells[0].ReadOnly = true;
            dataGridView2.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueLow / 100.0f;
            dataGridView2.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueHigh / 100.0f;

            //加行
            dataGridView2.Rows.Add();
            dataGridView2.Rows[8].Cells[0].Value = "A1 M9";
            dataGridView2.Rows[8].Cells[0].ReadOnly = true;
            dataGridView2.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueLow / 100.0f;
            dataGridView2.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueHigh / 100.0f;
        }

        private void InitTableA2M0()
        {
            //属性
            dataGridView3.Size = new System.Drawing.Size(503, 83);
            dataGridView3.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView3.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView3.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView3.EnableHeadersVisualStyles = false;
            dataGridView3.ColumnHeadersHeight = 40;
            dataGridView3.RowTemplate.Height = 40;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AllowUserToDeleteRows = false;
            dataGridView3.AllowUserToOrderColumns = false;
            dataGridView3.AllowUserToResizeColumns = false;
            dataGridView3.AllowUserToResizeRows = false;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView3.Columns[0].Width = 100;
            dataGridView3.Columns[1].Width = 200;
            dataGridView3.Columns[2].Width = 200;
            dataGridView3.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView3.Columns[0].HeaderText = "模 式";
            //dataGridView3.Columns[1].HeaderText = "预设扭矩(" + unit + ")";
            //dataGridView3.Columns[2].HeaderText = "目标角度°";
            dataGridView3.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "预设扭矩(" + unit + ")" : "preset torque(" + unit + ")";

            //加行
            dataGridView3.Rows.Add();
            dataGridView3.Rows[0].Cells[0].Value = "A2 M0";
            dataGridView3.Rows[0].Cells[0].ReadOnly = true;
            dataGridView3.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.torquePre / 100.0f;
            dataGridView3.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.angleTarget / 10.0f;
        }

        private void InitTableA2MX()
        {
            //属性
            dataGridView4.Size = new System.Drawing.Size(703, 403);
            dataGridView4.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView4.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView4.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView4.EnableHeadersVisualStyles = false;
            dataGridView4.ColumnHeadersHeight = 40;
            dataGridView4.RowTemplate.Height = 40;
            dataGridView4.AllowUserToAddRows = false;
            dataGridView4.AllowUserToDeleteRows = false;
            dataGridView4.AllowUserToOrderColumns = false;
            dataGridView4.AllowUserToResizeColumns = false;
            dataGridView4.AllowUserToResizeRows = false;
            dataGridView4.RowHeadersVisible = false;
            dataGridView4.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView4.Columns[0].Width = 100;
            dataGridView4.Columns[1].Width = 200;
            dataGridView4.Columns[2].Width = 200;
            dataGridView4.Columns[3].Width = 200;
            dataGridView4.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView4.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView4.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView4.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView4.Columns[0].HeaderText = "模 式";
            //dataGridView4.Columns[1].HeaderText = "预设扭矩(" + unit + ")";
            //dataGridView4.Columns[2].HeaderText = "角度下限°";
            //dataGridView4.Columns[3].HeaderText = "目标上限°";
            dataGridView4.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "预设扭矩(" + unit + ")" : "preset torque(" + unit + ")";

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[0].Cells[0].Value = "A2 M1";
            dataGridView4.Rows[0].Cells[0].ReadOnly = true;
            dataGridView4.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.torquePre / 100.0f;
            dataGridView4.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleLow / 10.0f;
            dataGridView4.Rows[0].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[1].Cells[0].Value = "A2 M2";
            dataGridView4.Rows[1].Cells[0].ReadOnly = true;
            dataGridView4.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.torquePre / 100.0f;
            dataGridView4.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleLow / 10.0f;
            dataGridView4.Rows[1].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[2].Cells[0].Value = "A2 M3";
            dataGridView4.Rows[2].Cells[0].ReadOnly = true;
            dataGridView4.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.torquePre / 100.0f;
            dataGridView4.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleLow / 10.0f;
            dataGridView4.Rows[2].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[3].Cells[0].Value = "A2 M4";
            dataGridView4.Rows[3].Cells[0].ReadOnly = true;
            dataGridView4.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.torquePre / 100.0f;
            dataGridView4.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleLow / 10.0f;
            dataGridView4.Rows[3].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[4].Cells[0].Value = "A2 M5";
            dataGridView4.Rows[4].Cells[0].ReadOnly = true;
            dataGridView4.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.torquePre / 100.0f;
            dataGridView4.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleLow / 10.0f;
            dataGridView4.Rows[4].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[5].Cells[0].Value = "A2 M6";
            dataGridView4.Rows[5].Cells[0].ReadOnly = true;
            dataGridView4.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.torquePre / 100.0f;
            dataGridView4.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleLow / 10.0f;
            dataGridView4.Rows[5].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[6].Cells[0].Value = "A2 M7";
            dataGridView4.Rows[6].Cells[0].ReadOnly = true;
            dataGridView4.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.torquePre / 100.0f;
            dataGridView4.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleLow / 10.0f;
            dataGridView4.Rows[6].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[7].Cells[0].Value = "A2 M8";
            dataGridView4.Rows[7].Cells[0].ReadOnly = true;
            dataGridView4.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.torquePre / 100.0f;
            dataGridView4.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleLow / 10.0f;
            dataGridView4.Rows[7].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleHigh / 10.0f;

            //加行
            dataGridView4.Rows.Add();
            dataGridView4.Rows[8].Cells[0].Value = "A2 M9";
            dataGridView4.Rows[8].Cells[0].ReadOnly = true;
            dataGridView4.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.torquePre / 100.0f;
            dataGridView4.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleLow / 10.0f;
            dataGridView4.Rows[8].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleHigh / 10.0f;
        }

        //初始化CFG表格
        private void InitTableCFG()
        {
            #region A1表格
            //属性
            dataGridView7.Size = new System.Drawing.Size(313, 83);
            dataGridView7.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView7.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView7.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView7.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView7.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView7.EnableHeadersVisualStyles = false;
            dataGridView7.ColumnHeadersHeight = 40;
            dataGridView7.RowTemplate.Height = 40;
            dataGridView7.AllowUserToAddRows = false;
            dataGridView7.AllowUserToDeleteRows = false;
            dataGridView7.AllowUserToOrderColumns = false;
            dataGridView7.AllowUserToResizeColumns = false;
            dataGridView7.AllowUserToResizeRows = false;
            dataGridView7.RowHeadersVisible = false;
            dataGridView7.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView7.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView7.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView7.Columns[0].Width = 100;
            dataGridView7.Columns[1].Width = 210;
            dataGridView7.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView7.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView1.Columns[0].HeaderText = "模 式";
            //dataGridView1.Columns[1].HeaderText = "目标扭矩(" + unit + ")";
            dataGridView7.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "目标扭矩(" + unit + ")" : "target torque(" + unit + ")";

            //加行
            dataGridView7.Rows.Add();
            dataGridView7.Rows[0].Cells[0].Value = "A1 M0";
            dataGridView7.Rows[0].Cells[0].ReadOnly = true;
            dataGridView7.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M0.torqueTarget / 100.0f;

            //属性
            dataGridView8.Size = new System.Drawing.Size(523, 403);
            dataGridView8.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView8.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView8.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView8.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView8.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView8.EnableHeadersVisualStyles = false;
            dataGridView8.ColumnHeadersHeight = 40;
            dataGridView8.RowTemplate.Height = 40;
            dataGridView8.AllowUserToAddRows = false;
            dataGridView8.AllowUserToDeleteRows = false;
            dataGridView8.AllowUserToOrderColumns = false;
            dataGridView8.AllowUserToResizeColumns = false;
            dataGridView8.AllowUserToResizeRows = false;
            dataGridView8.RowHeadersVisible = false;
            dataGridView8.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView8.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView8.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView8.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView8.Columns[0].Width = 100;
            dataGridView8.Columns[1].Width = 210;
            dataGridView8.Columns[2].Width = 210;
            dataGridView8.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView8.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView8.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView2.Columns[0].HeaderText = "模 式";
            //dataGridView2.Columns[1].HeaderText = "扭矩下限(" + unit + ")";
            //dataGridView2.Columns[2].HeaderText = "扭矩上限(" + unit + ")";
            dataGridView8.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "扭矩下限(" + unit + ")" : "torque lower limit(" + unit + ")";
            dataGridView8.Columns[2].HeaderText = MyDefine.myXET.languageNum == 0 ? "扭矩上限(" + unit + ")" : "torque upper limit(" + unit + ")";

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[0].Cells[0].Value = "A1 M1";
            dataGridView8.Rows[0].Cells[0].ReadOnly = true;
            dataGridView8.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueLow / 100.0f;
            dataGridView8.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[1].Cells[0].Value = "A1 M2";
            dataGridView8.Rows[1].Cells[0].ReadOnly = true;
            dataGridView8.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueLow / 100.0f;
            dataGridView8.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[2].Cells[0].Value = "A1 M3";
            dataGridView8.Rows[2].Cells[0].ReadOnly = true;
            dataGridView8.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueLow / 100.0f;
            dataGridView8.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[3].Cells[0].Value = "A1 M4";
            dataGridView8.Rows[3].Cells[0].ReadOnly = true;
            dataGridView8.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueLow / 100.0f;
            dataGridView8.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[4].Cells[0].Value = "A1 M5";
            dataGridView8.Rows[4].Cells[0].ReadOnly = true;
            dataGridView8.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueLow / 100.0f;
            dataGridView8.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[5].Cells[0].Value = "A1 M6";
            dataGridView8.Rows[5].Cells[0].ReadOnly = true;
            dataGridView8.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueLow / 100.0f;
            dataGridView8.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[6].Cells[0].Value = "A1 M7";
            dataGridView8.Rows[6].Cells[0].ReadOnly = true;
            dataGridView8.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueLow / 100.0f;
            dataGridView8.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[7].Cells[0].Value = "A1 M8";
            dataGridView8.Rows[7].Cells[0].ReadOnly = true;
            dataGridView8.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueLow / 100.0f;
            dataGridView8.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueHigh / 100.0f;

            //加行
            dataGridView8.Rows.Add();
            dataGridView8.Rows[8].Cells[0].Value = "A1 M9";
            dataGridView8.Rows[8].Cells[0].ReadOnly = true;
            dataGridView8.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueLow / 100.0f;
            dataGridView8.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueHigh / 100.0f;

            #endregion

            #region A2表格

            //属性
            dataGridView9.Size = new System.Drawing.Size(503, 83);
            dataGridView9.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView9.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView9.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView9.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView9.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView9.EnableHeadersVisualStyles = false;
            dataGridView9.ColumnHeadersHeight = 40;
            dataGridView9.RowTemplate.Height = 40;
            dataGridView9.AllowUserToAddRows = false;
            dataGridView9.AllowUserToDeleteRows = false;
            dataGridView9.AllowUserToOrderColumns = false;
            dataGridView9.AllowUserToResizeColumns = false;
            dataGridView9.AllowUserToResizeRows = false;
            dataGridView9.RowHeadersVisible = false;
            dataGridView9.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView9.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView9.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView9.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView9.Columns[0].Width = 100;
            dataGridView9.Columns[1].Width = 200;
            dataGridView9.Columns[2].Width = 200;
            dataGridView9.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView9.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView9.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView3.Columns[0].HeaderText = "模 式";
            //dataGridView3.Columns[1].HeaderText = "预设扭矩(" + unit + ")";
            //dataGridView3.Columns[2].HeaderText = "目标角度°";
            dataGridView9.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "预设扭矩(" + unit + ")" : "preset torque(" + unit + ")";

            //加行
            dataGridView9.Rows.Add();
            dataGridView9.Rows[0].Cells[0].Value = "A2 M0";
            dataGridView9.Rows[0].Cells[0].ReadOnly = true;
            dataGridView9.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.torquePre / 100.0f;
            dataGridView9.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M0.angleTarget / 10.0f;

            //属性
            dataGridView10.Size = new System.Drawing.Size(703, 403);
            dataGridView10.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView10.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView10.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView10.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView10.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView10.EnableHeadersVisualStyles = false;
            dataGridView10.ColumnHeadersHeight = 40;
            dataGridView10.RowTemplate.Height = 40;
            dataGridView10.AllowUserToAddRows = false;
            dataGridView10.AllowUserToDeleteRows = false;
            dataGridView10.AllowUserToOrderColumns = false;
            dataGridView10.AllowUserToResizeColumns = false;
            dataGridView10.AllowUserToResizeRows = false;
            dataGridView10.RowHeadersVisible = false;
            dataGridView10.Font = new Font("Arial", 12, FontStyle.Bold);

            //加列
            dataGridView10.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView10.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView10.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView10.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView10.Columns[0].Width = 100;
            dataGridView10.Columns[1].Width = 200;
            dataGridView10.Columns[2].Width = 200;
            dataGridView10.Columns[3].Width = 200;
            dataGridView10.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView10.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView10.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView10.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            //标题
            //dataGridView4.Columns[0].HeaderText = "模 式";
            //dataGridView4.Columns[1].HeaderText = "预设扭矩(" + unit + ")";
            //dataGridView4.Columns[2].HeaderText = "角度下限°";
            //dataGridView4.Columns[3].HeaderText = "目标上限°";
            dataGridView10.Columns[1].HeaderText = MyDefine.myXET.languageNum == 0 ? "预设扭矩(" + unit + ")" : "preset torque(" + unit + ")";

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[0].Cells[0].Value = "A2 M1";
            dataGridView10.Rows[0].Cells[0].ReadOnly = true;
            dataGridView10.Rows[0].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.torquePre / 100.0f;
            dataGridView10.Rows[0].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleLow / 10.0f;
            dataGridView10.Rows[0].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M1.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[1].Cells[0].Value = "A2 M2";
            dataGridView10.Rows[1].Cells[0].ReadOnly = true;
            dataGridView10.Rows[1].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.torquePre / 100.0f;
            dataGridView10.Rows[1].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleLow / 10.0f;
            dataGridView10.Rows[1].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M2.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[2].Cells[0].Value = "A2 M3";
            dataGridView10.Rows[2].Cells[0].ReadOnly = true;
            dataGridView10.Rows[2].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.torquePre / 100.0f;
            dataGridView10.Rows[2].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleLow / 10.0f;
            dataGridView10.Rows[2].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M3.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[3].Cells[0].Value = "A2 M4";
            dataGridView10.Rows[3].Cells[0].ReadOnly = true;
            dataGridView10.Rows[3].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.torquePre / 100.0f;
            dataGridView10.Rows[3].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleLow / 10.0f;
            dataGridView10.Rows[3].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M4.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[4].Cells[0].Value = "A2 M5";
            dataGridView10.Rows[4].Cells[0].ReadOnly = true;
            dataGridView10.Rows[4].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.torquePre / 100.0f;
            dataGridView10.Rows[4].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleLow / 10.0f;
            dataGridView10.Rows[4].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M5.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[5].Cells[0].Value = "A2 M6";
            dataGridView10.Rows[5].Cells[0].ReadOnly = true;
            dataGridView10.Rows[5].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.torquePre / 100.0f;
            dataGridView10.Rows[5].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleLow / 10.0f;
            dataGridView10.Rows[5].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M6.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[6].Cells[0].Value = "A2 M7";
            dataGridView10.Rows[6].Cells[0].ReadOnly = true;
            dataGridView10.Rows[6].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.torquePre / 100.0f;
            dataGridView10.Rows[6].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleLow / 10.0f;
            dataGridView10.Rows[6].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M7.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[7].Cells[0].Value = "A2 M8";
            dataGridView10.Rows[7].Cells[0].ReadOnly = true;
            dataGridView10.Rows[7].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.torquePre / 100.0f;
            dataGridView10.Rows[7].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleLow / 10.0f;
            dataGridView10.Rows[7].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M8.angleHigh / 10.0f;

            //加行
            dataGridView10.Rows.Add();
            dataGridView10.Rows[8].Cells[0].Value = "A2 M9";
            dataGridView10.Rows[8].Cells[0].ReadOnly = true;
            dataGridView10.Rows[8].Cells[1].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.torquePre / 100.0f;
            dataGridView10.Rows[8].Cells[2].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleLow / 10.0f;
            dataGridView10.Rows[8].Cells[3].Value = MyDefine.myXET.DEV.a2mxTable.A2M9.angleHigh / 10.0f;

            #endregion
        }

        //初始化模式名称
        private void InitTableModeName()
        {
            //属性
            dataGridView5.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView5.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView5.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView5.EnableHeadersVisualStyles = false;
            dataGridView5.ColumnHeadersHeight = 40;
            dataGridView5.RowTemplate.Height = 40;
            dataGridView5.AllowUserToAddRows = false;
            dataGridView5.AllowUserToDeleteRows = false;
            dataGridView5.AllowUserToOrderColumns = false;
            dataGridView5.AllowUserToResizeColumns = false;
            dataGridView5.AllowUserToResizeRows = false;
            dataGridView5.RowHeadersVisible = false;
            dataGridView5.Font = new Font("Arial", 12, FontStyle.Regular);

            dataGridView6.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView6.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView6.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView6.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView6.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView6.EnableHeadersVisualStyles = false;
            dataGridView6.ColumnHeadersHeight = 40;
            dataGridView6.RowTemplate.Height = 40;
            dataGridView6.AllowUserToAddRows = false;
            dataGridView6.AllowUserToDeleteRows = false;
            dataGridView6.AllowUserToOrderColumns = false;
            dataGridView6.AllowUserToResizeColumns = false;
            dataGridView6.AllowUserToResizeRows = false;
            dataGridView6.RowHeadersVisible = false;
            dataGridView6.Font = new Font("Arial", 12, FontStyle.Regular);

            //加列
            dataGridView5.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView5.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView6.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView6.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            //加行
            dataGridView5.Rows.Add();
            dataGridView5.Rows[0].Cells[0].Value = "A1 M0";
            dataGridView5.Rows[0].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[1].Cells[0].Value = "A1 M1";
            dataGridView5.Rows[1].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[2].Cells[0].Value = "A1 M2";
            dataGridView5.Rows[2].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[3].Cells[0].Value = "A1 M3";
            dataGridView5.Rows[3].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[4].Cells[0].Value = "A1 M4";
            dataGridView5.Rows[4].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[5].Cells[0].Value = "A1 M5";
            dataGridView5.Rows[5].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[6].Cells[0].Value = "A1 M6";
            dataGridView5.Rows[6].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[7].Cells[0].Value = "A1 M7";
            dataGridView5.Rows[7].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[8].Cells[0].Value = "A1 M8";
            dataGridView5.Rows[8].Cells[0].ReadOnly = true;

            dataGridView5.Rows.Add();
            dataGridView5.Rows[9].Cells[0].Value = "A1 M9";
            dataGridView5.Rows[9].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[0].Cells[0].Value = "A2 M0";
            dataGridView6.Rows[0].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[1].Cells[0].Value = "A2 M1";
            dataGridView6.Rows[1].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[2].Cells[0].Value = "A2 M2";
            dataGridView6.Rows[2].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[3].Cells[0].Value = "A2 M3";
            dataGridView6.Rows[3].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[4].Cells[0].Value = "A2 M4";
            dataGridView6.Rows[4].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[5].Cells[0].Value = "A2 M5";
            dataGridView6.Rows[5].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[6].Cells[0].Value = "A2 M6";
            dataGridView6.Rows[6].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[7].Cells[0].Value = "A2 M7";
            dataGridView6.Rows[7].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[8].Cells[0].Value = "A2 M8";
            dataGridView6.Rows[8].Cells[0].ReadOnly = true;

            dataGridView6.Rows.Add();
            dataGridView6.Rows[9].Cells[0].Value = "A2 M9";
            dataGridView6.Rows[9].Cells[0].ReadOnly = true;
        }

        private void MenuCalForm_Load(object sender, EventArgs e)
        {
            switch (MyDefine.myXET.DEV.modeRec)
            {
                case 0: radioButton1.Checked = true; break;
                case 1: radioButton2.Checked = true; break;
                case 2: radioButton3.Checked = true; break;
            }

            switch (MyDefine.myXET.DEV.torqueUnit)
            {
                case UNIT.UNIT_nm: unit = "N·m"; break;
                case UNIT.UNIT_lbfin: unit = "lbf·in"; break;
                case UNIT.UNIT_lbfft: unit = "lbf·ft"; break;
                case UNIT.UNIT_kgcm: unit = "kgf·cm"; break;
            }

            InitTableA1M0();
            InitTableA1MX();
            InitTableA2M0();
            InitTableA2MX();
            InitTableModeName();//初始化模式名称表格
            InitTableCFG();//初始化CFG表格

            SelectComboBoxItemByCurrentGroupCode();//更新combobox

            MyDefine.myXET.myUpdate += new freshHandler(receivePara);

            UpdateTableCFG();
        }

        private void MenuCalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyDefine.myXET.myUpdate -= new freshHandler(receivePara);
        }

        //不缓存
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                mode = 0;

                //需要更新参数
                if (mode != MyDefine.myXET.DEV.modeRec)
                {
                    isPARA = true;
                    if (MyDefine.myXET.isType == 1)
                    {
                        MyDefine.myXET.mePort_Write_RECMODE(mode);
                    }
                    else if (MyDefine.myXET.isType == 2)
                    {
                        MyDefine.myXET.nePort_Write_RECMODE(mode);
                    }
                }
                //不需要更新参数
                else
                {
                    isPARA = false;
                }
            }
        }

        //缓存TRACK
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                mode = 1;

                //需要更新参数
                if (mode != MyDefine.myXET.DEV.modeRec)
                {
                    isPARA = true;
                    if (MyDefine.myXET.isType == 1)
                    {
                        MyDefine.myXET.mePort_Write_RECMODE(mode);
                    }
                    else if (MyDefine.myXET.isType == 2)
                    {
                        MyDefine.myXET.nePort_Write_RECMODE(mode);
                    }
                }
                //不需要更新参数
                else
                {
                    isPARA = false;
                }
            }
        }

        //缓存PEAK
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                mode = 2;

                //需要更新参数
                if (mode != MyDefine.myXET.DEV.modeRec)
                {
                    isPARA = true;
                    if (MyDefine.myXET.isType == 1)
                    {
                        MyDefine.myXET.mePort_Write_RECMODE(mode);
                    }
                    else if (MyDefine.myXET.isType == 2)
                    {
                        MyDefine.myXET.nePort_Write_RECMODE(mode);
                    }
                }
                //不需要更新参数
                else
                {
                    isPARA = false;
                }
            }
        }

        //更新A1Mx
        private void button1_Click(object sender, EventArgs e)
        {
            isImportCFG = false;
            if (MyDefine.myXET.DEV.isActive == false) return;

            a1Table.A1M0.torqueTarget = (UInt32)(Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M1.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M1.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[0].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M2.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[1].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M2.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[1].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M3.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[2].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M3.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[2].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M4.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[3].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M4.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[3].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M5.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M5.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[4].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M6.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M6.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[5].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M7.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[6].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M7.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[6].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M8.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[7].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M8.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[7].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M9.torqueLow = (UInt32)(Convert.ToDouble(dataGridView2.Rows[8].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M9.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView2.Rows[8].Cells[2].Value.ToString()) * 100 + 0.5);

            a1Table.A1M0.torqueTarget = getDataCheck(a1Table.A1M0.torqueTarget, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M1.torqueLow = getDataCheck(a1Table.A1M1.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M1.torqueHigh = getDataCheck(a1Table.A1M1.torqueHigh, a1Table.A1M1.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M2.torqueLow = getDataCheck(a1Table.A1M2.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M2.torqueHigh = getDataCheck(a1Table.A1M2.torqueHigh, a1Table.A1M2.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M3.torqueLow = getDataCheck(a1Table.A1M3.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M3.torqueHigh = getDataCheck(a1Table.A1M3.torqueHigh, a1Table.A1M3.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M4.torqueLow = getDataCheck(a1Table.A1M4.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M4.torqueHigh = getDataCheck(a1Table.A1M4.torqueHigh, a1Table.A1M4.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M5.torqueLow = getDataCheck(a1Table.A1M5.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M5.torqueHigh = getDataCheck(a1Table.A1M5.torqueHigh, a1Table.A1M5.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M6.torqueLow = getDataCheck(a1Table.A1M6.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M6.torqueHigh = getDataCheck(a1Table.A1M6.torqueHigh, a1Table.A1M6.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M7.torqueLow = getDataCheck(a1Table.A1M7.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M7.torqueHigh = getDataCheck(a1Table.A1M7.torqueHigh, a1Table.A1M7.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M8.torqueLow = getDataCheck(a1Table.A1M8.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M8.torqueHigh = getDataCheck(a1Table.A1M8.torqueHigh, a1Table.A1M8.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M9.torqueLow = getDataCheck(a1Table.A1M9.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M9.torqueHigh = getDataCheck(a1Table.A1M9.torqueHigh, a1Table.A1M9.torqueLow, MyDefine.myXET.DEV.torqueCapacity);

            isPARA = true;
            button1.Text = "A1M01";
            button1.BackColor = Color.Firebrick;
            button2.BackColor = label1.BackColor;
            button3.BackColor = label1.BackColor;
            if (MyDefine.myXET.isType == 1)
            {
                MyDefine.myXET.mePort_Write_A1M01DAT(a1Table.A1M0.torqueTarget, a1Table.A1M1.torqueLow, a1Table.A1M1.torqueHigh);
            }
            else if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Write_A1M01DAT(a1Table.A1M0.torqueTarget, a1Table.A1M1.torqueLow, a1Table.A1M1.torqueHigh);
            }
            MyDefine.myXET.oldAx = 0xFF;
            MyDefine.myXET.oldMx = 0xFF;
            MyDefine.myXET.oldTU = 0x00;
        }

        //更新A2Mx
        private void button2_Click(object sender, EventArgs e)
        {
            isImportCFG = false;
            if (MyDefine.myXET.DEV.isActive == false) return;

            a2Table.A2M0.torquePre = (UInt32)(Convert.ToDouble(dataGridView3.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M0.angleTarget = (UInt16)(Convert.ToDouble(dataGridView3.Rows[0].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M1.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M1.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[0].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M1.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[0].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M2.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[1].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M2.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[1].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M2.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[1].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M3.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[2].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M3.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[2].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M3.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[2].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M4.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[3].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M4.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[3].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M4.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[3].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M5.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[4].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M5.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[4].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M5.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[4].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M6.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[5].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M6.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[5].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M6.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[5].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M7.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[6].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M7.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[6].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M7.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[6].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M8.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[7].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M8.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[7].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M8.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[7].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M9.torquePre = (UInt32)(Convert.ToDouble(dataGridView4.Rows[8].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M9.angleLow = (UInt16)(Convert.ToDouble(dataGridView4.Rows[8].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M9.angleHigh = (UInt16)(Convert.ToDouble(dataGridView4.Rows[8].Cells[3].Value.ToString()) * 10 + 0.5);

            a2Table.A2M0.torquePre = getDataCheck(a2Table.A2M0.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M0.angleTarget = getDataCheck(a2Table.A2M0.angleTarget, 0, 3600);
            a2Table.A2M1.torquePre = getDataCheck(a2Table.A2M1.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M1.angleLow = getDataCheck(a2Table.A2M1.angleLow, 0, 3600);
            a2Table.A2M1.angleHigh = getDataCheck(a2Table.A2M1.angleHigh, a2Table.A2M1.angleLow, 3600);
            a2Table.A2M2.torquePre = getDataCheck(a2Table.A2M2.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M2.angleLow = getDataCheck(a2Table.A2M2.angleLow, 0, 3600);
            a2Table.A2M2.angleHigh = getDataCheck(a2Table.A2M2.angleHigh, a2Table.A2M2.angleLow, 3600);
            a2Table.A2M3.torquePre = getDataCheck(a2Table.A2M3.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M3.angleLow = getDataCheck(a2Table.A2M3.angleLow, 0, 3600);
            a2Table.A2M3.angleHigh = getDataCheck(a2Table.A2M3.angleHigh, a2Table.A2M3.angleLow, 3600);
            a2Table.A2M4.torquePre = getDataCheck(a2Table.A2M4.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M4.angleLow = getDataCheck(a2Table.A2M4.angleLow, 0, 3600);
            a2Table.A2M4.angleHigh = getDataCheck(a2Table.A2M4.angleHigh, a2Table.A2M4.angleLow, 3600);
            a2Table.A2M5.torquePre = getDataCheck(a2Table.A2M5.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M5.angleLow = getDataCheck(a2Table.A2M5.angleLow, 0, 3600);
            a2Table.A2M5.angleHigh = getDataCheck(a2Table.A2M5.angleHigh, a2Table.A2M5.angleLow, 3600);
            a2Table.A2M6.torquePre = getDataCheck(a2Table.A2M6.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M6.angleLow = getDataCheck(a2Table.A2M6.angleLow, 0, 3600);
            a2Table.A2M6.angleHigh = getDataCheck(a2Table.A2M6.angleHigh, a2Table.A2M6.angleLow, 3600);
            a2Table.A2M7.torquePre = getDataCheck(a2Table.A2M7.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M7.angleLow = getDataCheck(a2Table.A2M7.angleLow, 0, 3600);
            a2Table.A2M7.angleHigh = getDataCheck(a2Table.A2M7.angleHigh, a2Table.A2M7.angleLow, 3600);
            a2Table.A2M8.torquePre = getDataCheck(a2Table.A2M8.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M8.angleLow = getDataCheck(a2Table.A2M8.angleLow, 0, 3600);
            a2Table.A2M8.angleHigh = getDataCheck(a2Table.A2M8.angleHigh, a2Table.A2M8.angleLow, 3600);
            a2Table.A2M9.torquePre = getDataCheck(a2Table.A2M9.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M9.angleLow = getDataCheck(a2Table.A2M9.angleLow, 0, 3600);
            a2Table.A2M9.angleHigh = getDataCheck(a2Table.A2M9.angleHigh, a2Table.A2M9.angleLow, 3600);

            isPARA = true;
            button2.Text = "A2M01";
            button2.BackColor = Color.Firebrick;
            button1.BackColor = label1.BackColor;
            button3.BackColor = label1.BackColor;
            if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Write_A2M01DAT(a2Table.A2M0.torquePre, a2Table.A2M0.angleTarget, a2Table.A2M1.torquePre, a2Table.A2M1.angleLow, a2Table.A2M1.angleHigh);
            }
            else
            {
                MyDefine.myXET.mePort_Write_A2M01DAT(a2Table.A2M0.torquePre, a2Table.A2M0.angleTarget, a2Table.A2M1.torquePre, a2Table.A2M1.angleLow, a2Table.A2M1.angleHigh);
            }

            MyDefine.myXET.oldAx = 0xFF;
            MyDefine.myXET.oldMx = 0xFF;
            MyDefine.myXET.oldTU = 0x00;
        }

        //清除设备缓存
        private void button3_Click(object sender, EventArgs e)
        {
            isImportCFG = false;
            if (MyDefine.myXET.DEV.isActive == false) return;

            isPARA = true;
            button3.BackColor = Color.Firebrick;
            button1.BackColor = label1.BackColor;
            button2.BackColor = label1.BackColor;
            if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Clear_RECSIZE();
            }
            else
            {
                MyDefine.myXET.mePort_Clear_RECSIZE();
            }
        }

        //保存编号代码名称
        private void button4_Click(object sender, EventArgs e)
        {
            isImportCFG = false;

            label3.Text = "";

            MySettingsManager.CurrentGroupCode = comboBox1.SelectedValue.ToString();

            //名称
            //检查是否有重复的组名
            var selectedItem = comboBox1.SelectedItem as AXMXSettingsGroup;

            // 遍历 ComboBox 的所有项
            foreach (var item in comboBox1.Items)
            {
                var group = item as AXMXSettingsGroup;
                if (group != null && group != selectedItem && group.GroupName == textBox_groupname.Text)
                {
                    // 找到重复的组名
                    label3.Text = "新名称与其他组名称重复，保存不成功";
                    return;
                }
            }
            //检查正则
            string pattern = @"^.{0,15}$";
            if (!Regex.IsMatch(textBox_groupname.Text, pattern))
            {
                label3.Text = "组名称不能超过15个字符";
                return;
            }
            pattern = @"^[\u4e00-\u9fa5a-zA-Z0-9]{0,15}$";
            if (!Regex.IsMatch(textBox_groupname.Text, pattern))
            {
                label3.Text = "组名称只能输入中英文或数字";
                return;
            }

            MySettingsManager.SetGroupNameSetting(textBox_groupname.Text);
            //将datagridview5中的数据保存到Setting对应的项
            MySettingsManager.SetAXMXSetting("A1M0", dataGridView5.Rows[0].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M1", dataGridView5.Rows[1].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M2", dataGridView5.Rows[2].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M3", dataGridView5.Rows[3].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M4", dataGridView5.Rows[4].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M5", dataGridView5.Rows[5].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M6", dataGridView5.Rows[6].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M7", dataGridView5.Rows[7].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M8", dataGridView5.Rows[8].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A1M9", dataGridView5.Rows[9].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M0", dataGridView6.Rows[0].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M1", dataGridView6.Rows[1].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M2", dataGridView6.Rows[2].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M3", dataGridView6.Rows[3].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M4", dataGridView6.Rows[4].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M5", dataGridView6.Rows[5].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M6", dataGridView6.Rows[6].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M7", dataGridView6.Rows[7].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M8", dataGridView6.Rows[8].Cells[1].Value?.ToString());
            MySettingsManager.SetAXMXSetting("A2M9", dataGridView6.Rows[9].Cells[1].Value?.ToString());

            // 保存修改
            if (MySettingsManager.SaveSettings())
            {
                // 更新缓存
                MySettingsManager.LoadSettings();
                label3.Text = "保存成功";

                SelectComboBoxItemByCurrentGroupCode();//更新combobox
            }
        }

        //更改当前选择的组
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.DisplayMember == "") return;

            string groupCode = comboBox1.SelectedValue.ToString();

            textBox_groupname.Text = MySettingsManager.GetGroupNameSetting(groupCode);

            dataGridView5.Rows[0].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M0");
            dataGridView5.Rows[1].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M1");
            dataGridView5.Rows[2].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M2");
            dataGridView5.Rows[3].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M3");
            dataGridView5.Rows[4].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M4");
            dataGridView5.Rows[5].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M5");
            dataGridView5.Rows[6].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M6");
            dataGridView5.Rows[7].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M7");
            dataGridView5.Rows[8].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M8");
            dataGridView5.Rows[9].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A1M9");

            dataGridView6.Rows[0].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M0");
            dataGridView6.Rows[1].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M1");
            dataGridView6.Rows[2].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M2");
            dataGridView6.Rows[3].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M3");
            dataGridView6.Rows[4].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M4");
            dataGridView6.Rows[5].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M5");
            dataGridView6.Rows[6].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M6");
            dataGridView6.Rows[7].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M7");
            dataGridView6.Rows[8].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M8");
            dataGridView6.Rows[9].Cells[1].Value = MySettingsManager.GetAXMXSetting(groupCode, "A2M9");
        }

        // 根据CurrentGroupCode选择comboBox1中的项
        private void SelectComboBoxItemByCurrentGroupCode()
        {
            comboBox1.DataSource = MySettingsManager.SettingsGroups;
            comboBox1.DisplayMember = "GroupName";
            comboBox1.ValueMember = "GroupCode";

            string currentGroupCode = MySettingsManager.CurrentGroupCode;
            bool found = false;
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                // 获取当前项的 ValueMember 值
                var item = comboBox1.Items[i] as AXMXSettingsGroup;
                if (item != null && item.GroupCode == currentGroupCode)
                {
                    // 找到匹配项，设置为选中项
                    comboBox1.SelectedIndex = i;
                    found = true;
                    break;
                }
            }

            // 如果没有找到匹配项，设置 SelectedIndex 为 0
            if (!found && comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            textBox_groupname.Text = MySettingsManager.GetGroupNameSetting(comboBox1.SelectedValue.ToString());
        }

        #region 配置导入导出

        //导出表格A1A2配置
        private void ExportCFGToJson(string filePath)
        {
            var data = new
            {
                A1M0_torqueTarget = dataGridView7.Rows[0].Cells[1].Value?.ToString(),

                A1M1_torqueLow = dataGridView8.Rows[0].Cells[1].Value?.ToString(),
                A1M1_torqueHigh = dataGridView8.Rows[0].Cells[2].Value?.ToString(),
                A1M2_torqueLow = dataGridView8.Rows[1].Cells[1].Value?.ToString(),
                A1M2_torqueHigh = dataGridView8.Rows[1].Cells[2].Value?.ToString(),
                A1M3_torqueLow = dataGridView8.Rows[2].Cells[1].Value?.ToString(),
                A1M3_torqueHigh = dataGridView8.Rows[2].Cells[2].Value?.ToString(),
                A1M4_torqueLow = dataGridView8.Rows[3].Cells[1].Value?.ToString(),
                A1M4_torqueHigh = dataGridView8.Rows[3].Cells[2].Value?.ToString(),
                A1M5_torqueLow = dataGridView8.Rows[4].Cells[1].Value?.ToString(),
                A1M5_torqueHigh = dataGridView8.Rows[4].Cells[2].Value?.ToString(),
                A1M6_torqueLow = dataGridView8.Rows[5].Cells[1].Value?.ToString(),
                A1M6_torqueHigh = dataGridView8.Rows[5].Cells[2].Value?.ToString(),
                A1M7_torqueLow = dataGridView8.Rows[6].Cells[1].Value?.ToString(),
                A1M7_torqueHigh = dataGridView8.Rows[6].Cells[2].Value?.ToString(),
                A1M8_torqueLow = dataGridView8.Rows[7].Cells[1].Value?.ToString(),
                A1M8_torqueHigh = dataGridView8.Rows[7].Cells[2].Value?.ToString(),
                A1M9_torqueLow = dataGridView8.Rows[8].Cells[1].Value?.ToString(),
                A1M9_torqueHigh = dataGridView8.Rows[8].Cells[2].Value?.ToString(),

                A2M0_torquePre = dataGridView9.Rows[0].Cells[1].Value?.ToString(),
                A2M0_angleTarget = dataGridView9.Rows[0].Cells[2].Value?.ToString(),

                A2M1_torquePre = dataGridView10.Rows[0].Cells[1].Value?.ToString(),
                A2M1_angleLow = dataGridView10.Rows[0].Cells[2].Value?.ToString(),
                A2M1_angleHigh = dataGridView10.Rows[0].Cells[3].Value?.ToString(),
                A2M2_torquePre = dataGridView10.Rows[1].Cells[1].Value?.ToString(),
                A2M2_angleLow = dataGridView10.Rows[1].Cells[2].Value?.ToString(),
                A2M2_angleHigh = dataGridView10.Rows[1].Cells[3].Value?.ToString(),
                A2M3_torquePre = dataGridView10.Rows[2].Cells[1].Value?.ToString(),
                A2M3_angleLow = dataGridView10.Rows[2].Cells[2].Value?.ToString(),
                A2M3_angleHigh = dataGridView10.Rows[2].Cells[3].Value?.ToString(),
                A2M4_torquePre = dataGridView10.Rows[3].Cells[1].Value?.ToString(),
                A2M4_angleLow = dataGridView10.Rows[3].Cells[2].Value?.ToString(),
                A2M4_angleHigh = dataGridView10.Rows[3].Cells[3].Value?.ToString(),
                A2M5_torquePre = dataGridView10.Rows[4].Cells[1].Value?.ToString(),
                A2M5_angleLow = dataGridView10.Rows[4].Cells[2].Value?.ToString(),
                A2M5_angleHigh = dataGridView10.Rows[4].Cells[3].Value?.ToString(),
                A2M6_torquePre = dataGridView10.Rows[5].Cells[1].Value?.ToString(),
                A2M6_angleLow = dataGridView10.Rows[5].Cells[2].Value?.ToString(),
                A2M6_angleHigh = dataGridView10.Rows[5].Cells[3].Value?.ToString(),
                A2M7_torquePre = dataGridView10.Rows[6].Cells[1].Value?.ToString(),
                A2M7_angleLow = dataGridView10.Rows[6].Cells[2].Value?.ToString(),
                A2M7_angleHigh = dataGridView10.Rows[6].Cells[3].Value?.ToString(),
                A2M8_torquePre = dataGridView10.Rows[7].Cells[1].Value?.ToString(),
                A2M8_angleLow = dataGridView10.Rows[7].Cells[2].Value?.ToString(),
                A2M8_angleHigh = dataGridView10.Rows[7].Cells[3].Value?.ToString(),
                A2M9_torquePre = dataGridView10.Rows[8].Cells[1].Value?.ToString(),
                A2M9_angleLow = dataGridView10.Rows[8].Cells[2].Value?.ToString(),
                A2M9_angleHigh = dataGridView10.Rows[8].Cells[3].Value?.ToString()
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //导出扳手配置
        private void ExportDEVToJson(string filePath)
        {
            var data = new
            {
                A1M0_torqueTarget = MyDefine.myXET.DEV.a1mxTable.A1M0.torqueTarget / 100.0f,

                A1M1_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueLow / 100.0f,
                A1M1_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M1.torqueHigh / 100.0f,
                A1M2_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueLow / 100.0f,
                A1M2_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M2.torqueHigh / 100.0f,
                A1M3_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueLow / 100.0f,
                A1M3_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M3.torqueHigh / 100.0f,
                A1M4_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueLow / 100.0f,
                A1M4_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M4.torqueHigh / 100.0f,
                A1M5_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueLow / 100.0f,
                A1M5_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M5.torqueHigh / 100.0f,
                A1M6_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueLow / 100.0f,
                A1M6_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M6.torqueHigh / 100.0f,
                A1M7_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueLow / 100.0f,
                A1M7_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M7.torqueHigh / 100.0f,
                A1M8_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueLow / 100.0f,
                A1M8_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M8.torqueHigh / 100.0f,
                A1M9_torqueLow = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueLow / 100.0f,
                A1M9_torqueHigh = MyDefine.myXET.DEV.a1mxTable.A1M9.torqueHigh / 100.0f,

                A2M0_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M0.torquePre / 100.0f,
                A2M0_angleTarget = MyDefine.myXET.DEV.a2mxTable.A2M0.angleTarget / 10.0f,

                A2M1_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M1.torquePre / 100.0f,
                A2M1_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M1.angleLow / 10.0f,
                A2M1_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M1.angleHigh / 10.0f,
                A2M2_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M2.torquePre / 100.0f,
                A2M2_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M2.angleLow / 10.0f,
                A2M2_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M2.angleHigh / 10.0f,
                A2M3_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M3.torquePre / 100.0f,
                A2M3_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M3.angleLow / 10.0f,
                A2M3_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M3.angleHigh / 10.0f,
                A2M4_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M4.torquePre / 100.0f,
                A2M4_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M4.angleLow / 10.0f,
                A2M4_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M4.angleHigh / 10.0f,
                A2M5_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M5.torquePre / 100.0f,
                A2M5_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M5.angleLow / 10.0f,
                A2M5_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M5.angleHigh / 10.0f,
                A2M6_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M6.torquePre / 100.0f,
                A2M6_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M6.angleLow / 10.0f,
                A2M6_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M6.angleHigh / 10.0f,
                A2M7_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M7.torquePre / 100.0f,
                A2M7_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M7.angleLow / 10.0f,
                A2M7_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M7.angleHigh / 10.0f,
                A2M8_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M8.torquePre / 100.0f,
                A2M8_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M8.angleLow / 10.0f,
                A2M8_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M8.angleHigh / 10.0f,
                A2M9_torquePre = MyDefine.myXET.DEV.a2mxTable.A2M9.torquePre / 100.0f,
                A2M9_angleLow = MyDefine.myXET.DEV.a2mxTable.A2M9.angleLow / 10.0f,
                A2M9_angleHigh = MyDefine.myXET.DEV.a2mxTable.A2M9.angleHigh / 10.0f
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //导入配置
        private void ImportDEVFormJson(string filePath, string json, bool isMessageBox = true)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(json);

                if (!checkRequiredValues(data))
                {
                    if (isMessageBox)
                    {
                        MessageBox.Show("读取失败，文件中缺少必要的数值或数值格式不正确。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                // 将数据填充到 DataGridView 
                dataGridView7.Rows[0].Cells[1].Value = data.A1M0_torqueTarget;
                dataGridView8.Rows[0].Cells[1].Value = data.A1M1_torqueLow;
                dataGridView8.Rows[0].Cells[2].Value = data.A1M1_torqueHigh;
                dataGridView8.Rows[1].Cells[1].Value = data.A1M2_torqueLow;
                dataGridView8.Rows[1].Cells[2].Value = data.A1M2_torqueHigh;
                dataGridView8.Rows[2].Cells[1].Value = data.A1M3_torqueLow;
                dataGridView8.Rows[2].Cells[2].Value = data.A1M3_torqueHigh;
                dataGridView8.Rows[3].Cells[1].Value = data.A1M4_torqueLow;
                dataGridView8.Rows[3].Cells[2].Value = data.A1M4_torqueHigh;
                dataGridView8.Rows[4].Cells[1].Value = data.A1M5_torqueLow;
                dataGridView8.Rows[4].Cells[2].Value = data.A1M5_torqueHigh;
                dataGridView8.Rows[5].Cells[1].Value = data.A1M6_torqueLow;
                dataGridView8.Rows[5].Cells[2].Value = data.A1M6_torqueHigh;
                dataGridView8.Rows[6].Cells[1].Value = data.A1M7_torqueLow;
                dataGridView8.Rows[6].Cells[2].Value = data.A1M7_torqueHigh;
                dataGridView8.Rows[7].Cells[1].Value = data.A1M8_torqueLow;
                dataGridView8.Rows[7].Cells[2].Value = data.A1M8_torqueHigh;
                dataGridView8.Rows[8].Cells[1].Value = data.A1M9_torqueLow;
                dataGridView8.Rows[8].Cells[2].Value = data.A1M9_torqueHigh;

                dataGridView9.Rows[0].Cells[1].Value = data.A2M0_torquePre;
                dataGridView9.Rows[0].Cells[2].Value = data.A2M0_angleTarget;
                dataGridView10.Rows[0].Cells[1].Value = data.A2M1_torquePre;
                dataGridView10.Rows[0].Cells[2].Value = data.A2M1_angleLow;
                dataGridView10.Rows[0].Cells[3].Value = data.A2M1_angleHigh;
                dataGridView10.Rows[1].Cells[1].Value = data.A2M2_torquePre;
                dataGridView10.Rows[1].Cells[2].Value = data.A2M2_angleLow;
                dataGridView10.Rows[1].Cells[3].Value = data.A2M2_angleHigh;
                dataGridView10.Rows[2].Cells[1].Value = data.A2M3_torquePre;
                dataGridView10.Rows[2].Cells[2].Value = data.A2M3_angleLow;
                dataGridView10.Rows[2].Cells[3].Value = data.A2M3_angleHigh;
                dataGridView10.Rows[3].Cells[1].Value = data.A2M4_torquePre;
                dataGridView10.Rows[3].Cells[2].Value = data.A2M4_angleLow;
                dataGridView10.Rows[3].Cells[3].Value = data.A2M4_angleHigh;
                dataGridView10.Rows[4].Cells[1].Value = data.A2M5_torquePre;
                dataGridView10.Rows[4].Cells[2].Value = data.A2M5_angleLow;
                dataGridView10.Rows[4].Cells[3].Value = data.A2M5_angleHigh;
                dataGridView10.Rows[5].Cells[1].Value = data.A2M6_torquePre;
                dataGridView10.Rows[5].Cells[2].Value = data.A2M6_angleLow;
                dataGridView10.Rows[5].Cells[3].Value = data.A2M6_angleHigh;
                dataGridView10.Rows[6].Cells[1].Value = data.A2M7_torquePre;
                dataGridView10.Rows[6].Cells[2].Value = data.A2M7_angleLow;
                dataGridView10.Rows[6].Cells[3].Value = data.A2M7_angleHigh;
                dataGridView10.Rows[7].Cells[1].Value = data.A2M8_torquePre;
                dataGridView10.Rows[7].Cells[2].Value = data.A2M8_angleLow;
                dataGridView10.Rows[7].Cells[3].Value = data.A2M8_angleHigh;
                dataGridView10.Rows[8].Cells[1].Value = data.A2M9_torquePre;
                dataGridView10.Rows[8].Cells[2].Value = data.A2M9_angleLow;
                dataGridView10.Rows[8].Cells[3].Value = data.A2M9_angleHigh;
            }
            catch
            {
                if (isMessageBox)
                {
                    MessageBox.Show("读取失败，文件格式不正确或包含无效字符。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        //导出扳手配置
        private void button_exportDEV_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string defaultFileName = "配置_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.InitialDirectory = MyDefine.myXET.userDEV;
                saveFileDialog.Filter = "CFG files (*.cfg)|*.cfg|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "cfg";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = Path.GetDirectoryName(saveFileDialog.FileName);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    ExportDEVToJson(saveFileDialog.FileName);
                }
            }
        }

        //导出表格配置
        private void button_exportCFG_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string defaultFileName = "配置_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.InitialDirectory = MyDefine.myXET.userDEV;
                saveFileDialog.Filter = "CFG files (*.cfg)|*.cfg|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "cfg";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = Path.GetDirectoryName(saveFileDialog.FileName);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    ExportCFGToJson(saveFileDialog.FileName);
                }
            }
        }

        //导入配置
        private void button_importCFG_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = MyDefine.myXET.userDEV;
                openFileDialog.Filter = "CFG files (*.cfg)|*.cfg|All files (*.*)|*.*";
                openFileDialog.DefaultExt = "cfg";
                openFileDialog.AddExtension = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string json = File.ReadAllText(filePath);

                    ImportDEVFormJson(filePath, json);
                }
            }
        }

        //写入配置
        private void button_writeCFG_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.DEV.isActive == false) return;

            getA1MXFromCFGTable();

            isPARA = true;
            button_writeCFG.Text = "A1M01";
            button_writeCFG.BackColor = Color.Firebrick;
            button1.BackColor = label1.BackColor;
            button2.BackColor = label1.BackColor;
            button3.BackColor = label1.BackColor;

            isImportCFG = true;
            if (MyDefine.myXET.isType == 1)
            {
                MyDefine.myXET.mePort_Write_A1M01DAT(a1Table.A1M0.torqueTarget, a1Table.A1M1.torqueLow, a1Table.A1M1.torqueHigh);
            }
            else if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Write_A1M01DAT(a1Table.A1M0.torqueTarget, a1Table.A1M1.torqueLow, a1Table.A1M1.torqueHigh);
            }
            MyDefine.myXET.oldAx = 0xFF;
            MyDefine.myXET.oldMx = 0xFF;
            MyDefine.myXET.oldTU = 0x00;
        }

        //写入扳手配置A2MX
        private void setA2MXFromCFGTable()
        {
            if (MyDefine.myXET.DEV.isActive == false) return;

            getA2MXFromCFGTable();

            isPARA = true;
            button_writeCFG.Text = "A2M01";
            button_writeCFG.BackColor = Color.Firebrick;
            button1.BackColor = label1.BackColor;
            button2.BackColor = label1.BackColor;
            button3.BackColor = label1.BackColor;
            if (MyDefine.myXET.isType == 2)
            {
                MyDefine.myXET.nePort_Write_A2M01DAT(a2Table.A2M0.torquePre, a2Table.A2M0.angleTarget, a2Table.A2M1.torquePre, a2Table.A2M1.angleLow, a2Table.A2M1.angleHigh);
            }
            else
            {
                MyDefine.myXET.mePort_Write_A2M01DAT(a2Table.A2M0.torquePre, a2Table.A2M0.angleTarget, a2Table.A2M1.torquePre, a2Table.A2M1.angleLow, a2Table.A2M1.angleHigh);
            }

            MyDefine.myXET.oldAx = 0xFF;
            MyDefine.myXET.oldMx = 0xFF;
            MyDefine.myXET.oldTU = 0x00;
        }

        private void getA1MXFromCFGTable()
        {
            a1Table.A1M0.torqueTarget = (UInt32)(Convert.ToDouble(dataGridView7.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M1.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M1.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[0].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M2.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[1].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M2.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[1].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M3.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[2].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M3.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[2].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M4.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[3].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M4.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[3].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M5.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[4].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M5.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[4].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M6.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[5].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M6.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[5].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M7.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[6].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M7.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[6].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M8.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[7].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M8.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[7].Cells[2].Value.ToString()) * 100 + 0.5);
            a1Table.A1M9.torqueLow = (UInt32)(Convert.ToDouble(dataGridView8.Rows[8].Cells[1].Value.ToString()) * 100 + 0.5);
            a1Table.A1M9.torqueHigh = (UInt32)(Convert.ToDouble(dataGridView8.Rows[8].Cells[2].Value.ToString()) * 100 + 0.5);

            a1Table.A1M0.torqueTarget = getDataCheck(a1Table.A1M0.torqueTarget, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M1.torqueLow = getDataCheck(a1Table.A1M1.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M1.torqueHigh = getDataCheck(a1Table.A1M1.torqueHigh, a1Table.A1M1.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M2.torqueLow = getDataCheck(a1Table.A1M2.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M2.torqueHigh = getDataCheck(a1Table.A1M2.torqueHigh, a1Table.A1M2.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M3.torqueLow = getDataCheck(a1Table.A1M3.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M3.torqueHigh = getDataCheck(a1Table.A1M3.torqueHigh, a1Table.A1M3.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M4.torqueLow = getDataCheck(a1Table.A1M4.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M4.torqueHigh = getDataCheck(a1Table.A1M4.torqueHigh, a1Table.A1M4.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M5.torqueLow = getDataCheck(a1Table.A1M5.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M5.torqueHigh = getDataCheck(a1Table.A1M5.torqueHigh, a1Table.A1M5.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M6.torqueLow = getDataCheck(a1Table.A1M6.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M6.torqueHigh = getDataCheck(a1Table.A1M6.torqueHigh, a1Table.A1M6.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M7.torqueLow = getDataCheck(a1Table.A1M7.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M7.torqueHigh = getDataCheck(a1Table.A1M7.torqueHigh, a1Table.A1M7.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M8.torqueLow = getDataCheck(a1Table.A1M8.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M8.torqueHigh = getDataCheck(a1Table.A1M8.torqueHigh, a1Table.A1M8.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M9.torqueLow = getDataCheck(a1Table.A1M9.torqueLow, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a1Table.A1M9.torqueHigh = getDataCheck(a1Table.A1M9.torqueHigh, a1Table.A1M9.torqueLow, MyDefine.myXET.DEV.torqueCapacity);
        }

        private void getA2MXFromCFGTable()
        {
            a2Table.A2M0.torquePre = (UInt32)(Convert.ToDouble(dataGridView9.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M0.angleTarget = (UInt16)(Convert.ToDouble(dataGridView9.Rows[0].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M1.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[0].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M1.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[0].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M1.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[0].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M2.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[1].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M2.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[1].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M2.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[1].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M3.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[2].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M3.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[2].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M3.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[2].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M4.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[3].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M4.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[3].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M4.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[3].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M5.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[4].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M5.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[4].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M5.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[4].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M6.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[5].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M6.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[5].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M6.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[5].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M7.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[6].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M7.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[6].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M7.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[6].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M8.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[7].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M8.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[7].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M8.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[7].Cells[3].Value.ToString()) * 10 + 0.5);
            a2Table.A2M9.torquePre = (UInt32)(Convert.ToDouble(dataGridView10.Rows[8].Cells[1].Value.ToString()) * 100 + 0.5);
            a2Table.A2M9.angleLow = (UInt16)(Convert.ToDouble(dataGridView10.Rows[8].Cells[2].Value.ToString()) * 10 + 0.5);
            a2Table.A2M9.angleHigh = (UInt16)(Convert.ToDouble(dataGridView10.Rows[8].Cells[3].Value.ToString()) * 10 + 0.5);

            a2Table.A2M0.torquePre = getDataCheck(a2Table.A2M0.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M0.angleTarget = getDataCheck(a2Table.A2M0.angleTarget, 0, 3600);
            a2Table.A2M1.torquePre = getDataCheck(a2Table.A2M1.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M1.angleLow = getDataCheck(a2Table.A2M1.angleLow, 0, 3600);
            a2Table.A2M1.angleHigh = getDataCheck(a2Table.A2M1.angleHigh, a2Table.A2M1.angleLow, 3600);
            a2Table.A2M2.torquePre = getDataCheck(a2Table.A2M2.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M2.angleLow = getDataCheck(a2Table.A2M2.angleLow, 0, 3600);
            a2Table.A2M2.angleHigh = getDataCheck(a2Table.A2M2.angleHigh, a2Table.A2M2.angleLow, 3600);
            a2Table.A2M3.torquePre = getDataCheck(a2Table.A2M3.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M3.angleLow = getDataCheck(a2Table.A2M3.angleLow, 0, 3600);
            a2Table.A2M3.angleHigh = getDataCheck(a2Table.A2M3.angleHigh, a2Table.A2M3.angleLow, 3600);
            a2Table.A2M4.torquePre = getDataCheck(a2Table.A2M4.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M4.angleLow = getDataCheck(a2Table.A2M4.angleLow, 0, 3600);
            a2Table.A2M4.angleHigh = getDataCheck(a2Table.A2M4.angleHigh, a2Table.A2M4.angleLow, 3600);
            a2Table.A2M5.torquePre = getDataCheck(a2Table.A2M5.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M5.angleLow = getDataCheck(a2Table.A2M5.angleLow, 0, 3600);
            a2Table.A2M5.angleHigh = getDataCheck(a2Table.A2M5.angleHigh, a2Table.A2M5.angleLow, 3600);
            a2Table.A2M6.torquePre = getDataCheck(a2Table.A2M6.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M6.angleLow = getDataCheck(a2Table.A2M6.angleLow, 0, 3600);
            a2Table.A2M6.angleHigh = getDataCheck(a2Table.A2M6.angleHigh, a2Table.A2M6.angleLow, 3600);
            a2Table.A2M7.torquePre = getDataCheck(a2Table.A2M7.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M7.angleLow = getDataCheck(a2Table.A2M7.angleLow, 0, 3600);
            a2Table.A2M7.angleHigh = getDataCheck(a2Table.A2M7.angleHigh, a2Table.A2M7.angleLow, 3600);
            a2Table.A2M8.torquePre = getDataCheck(a2Table.A2M8.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M8.angleLow = getDataCheck(a2Table.A2M8.angleLow, 0, 3600);
            a2Table.A2M8.angleHigh = getDataCheck(a2Table.A2M8.angleHigh, a2Table.A2M8.angleLow, 3600);
            a2Table.A2M9.torquePre = getDataCheck(a2Table.A2M9.torquePre, MyDefine.myXET.DEV.torqueMinima, MyDefine.myXET.DEV.torqueCapacity);
            a2Table.A2M9.angleLow = getDataCheck(a2Table.A2M9.angleLow, 0, 3600);
            a2Table.A2M9.angleHigh = getDataCheck(a2Table.A2M9.angleHigh, a2Table.A2M9.angleLow, 3600);
        }

        private bool checkRequiredValues(dynamic data)
        {
            string[] requiredFields = {
                "A1M0_torqueTarget",
                "A1M1_torqueLow","A1M1_torqueHigh","A1M2_torqueLow","A1M2_torqueHigh","A1M3_torqueLow","A1M3_torqueHigh",
                "A1M4_torqueLow","A1M4_torqueHigh","A1M5_torqueLow","A1M5_torqueHigh","A1M6_torqueLow","A1M6_torqueHigh",
                "A1M7_torqueLow","A1M7_torqueHigh","A1M8_torqueLow","A1M8_torqueHigh","A1M9_torqueLow","A1M9_torqueHigh",
                "A2M0_torquePre","A2M0_angleTarget",
                "A2M1_torquePre","A2M1_angleLow","A2M1_angleHigh","A2M2_torquePre","A2M2_angleLow","A2M2_angleHigh",
                "A2M3_torquePre","A2M3_angleLow","A2M3_angleHigh","A2M4_torquePre","A2M4_angleLow","A2M4_angleHigh",
                "A2M5_torquePre","A2M5_angleLow","A2M5_angleHigh","A2M6_torquePre","A2M6_angleLow","A2M6_angleHigh",
                "A2M7_torquePre","A2M7_angleLow","A2M7_angleHigh","A2M8_torquePre","A2M8_angleLow","A2M8_angleHigh",
                "A2M9_torquePre","A2M9_angleLow","A2M9_angleHigh"
            };

            foreach (var field in requiredFields)
            {
                if (data[field] == null || !IsNumeric(data[field].ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsNumeric(string value)
        {
            return double.TryParse(value, out _);
        }

        #endregion

        private UInt32 getDataCheck(UInt32 dat, UInt32 low, UInt32 high)
        {
            return (dat > low) ? ((dat > high) ? high : dat) : low;
        }

        private UInt16 getDataCheck(UInt16 dat, UInt16 low, UInt16 high)
        {
            return (dat > low) ? ((dat > high) ? high : dat) : low;
        }
    }
}

//end