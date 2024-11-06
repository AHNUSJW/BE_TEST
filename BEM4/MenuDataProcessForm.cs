using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BEM4
{
    public partial class MenuDataProcessForm : Form
    {
        private DataPicture dataPicture = new DataPicture();
        private List<DPTable> dataTables = new List<DPTable>();

        private Int32 bemPick = 0;      //picturebox中选中数据对应表格索引
        private float torqueMax = 0.0f; //记录扭矩峰值
        private Int32 index;    //取读入数据的指针

        private Dictionary<string, float> dicTorque = new Dictionary<string, float>();//读取扭矩峰值
        private Dictionary<string, float> dicAngle = new Dictionary<string, float>();//读取角度峰值
        private Dictionary<string, string> dicIndex = new Dictionary<string, string>();//记录数据段（以流水号为界）
        private int report_start;//选取数据起始idx
        private int report_stop; //选取数据结束idx
        private int[][] idex = new int[2][];

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
        public MenuDataProcessForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 实际操作函数
        /// </summary>
        private void dataForm_Update()
        {
            //其他线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(dataForm_Update);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {

                    MyDefine.myXET.dataUpdate -= new freshHandler(dataForm_Update);
                }
            }
            //本线程的操作请求
            else
            {
                index = 1;
                dataGridView1.Rows.Clear();
                dataPicture.Clear();

                if (MyDefine.myXET.DPT.Count > 1)
                {
                    dataPicture.start = 10000;
                    dataPicture.stop = -10000;
                    //清空表格
                    dataGridView1.Rows.Clear();
                    timer1.Enabled = true;
                    //数据初始化
                    dataUpdate();
                    MenuDataProcessForm_SizeChanged(null, null);
                }
            }
        }

        /// <summary>
        /// 显示界面后再刷新表格,每10ms加入部分数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Int32 idx;
            //每次加入部分数据
            for (int i = 0; i < 200; i++)
            {
                if (index < MyDefine.myXET.DPT.Count)
                {
                    //给表格添加数据
                    idx = dataGridView1.Rows.Add();
                    for (int j = 0; j < MyDefine.myXET.DPT[index].count_data && j <= 11; j++)
                    {
                        dataGridView1.Rows[idx].Cells[j].Value = MyDefine.myXET.DPT[index].data[j];
                    }
                    //更改指针数值
                    index++;
                }
                else
                {
                    timer1.Enabled = false;
                    //timer2.Enabled = true;
                    //移到最后一行
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;

                    //调整列宽
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
            }

            //移到最后一行
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        /// <summary>
        /// 画图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;

            //计算数据点
            dataPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height);
            dataPicture.getPoint_pictureBox(dataTables);

            MenuDataProcessForm_SizeChanged(null, null);
        }

        /// <summary>
        /// 更新表格
        /// </summary>
        private void dataUpdate()
        {
            dataTables.Clear();
            dicAngle.Clear();
            dicTorque.Clear();
            dicIndex.Clear();
            int index = 1;

            for (; index < MyDefine.myXET.DPT.Count; index++)
            {
                dataTables.Add(new DPTable(
                MyDefine.myXET.DPT[index].count_data,
                MyDefine.myXET.DPT[index].data
                ));
                dataTables[index - 1].dataSet();
                float ft = float.Parse(dataTables[index - 1].torque);
                float fa = float.Parse(dataTables[index - 1].angle);

                //获取对应流水号的扭矩峰值、角度峰值
                if (dicTorque.ContainsKey(dataTables[index - 1].opsn))
                {
                    dicTorque[dataTables[index - 1].opsn] = dicTorque[dataTables[index - 1].opsn] > ft ? dicTorque[dataTables[index - 1].opsn] : ft;
                    dicAngle[dataTables[index - 1].opsn] = dicAngle[dataTables[index - 1].opsn] > fa ? dicAngle[dataTables[index - 1].opsn] : fa;
                }
                else
                {
                    dicTorque.Add(dataTables[index - 1].opsn, ft);
                    dicAngle.Add(dataTables[index - 1].opsn, fa);

                    //记录数据段
                    dicIndex.Add(dataTables[index - 1].opsn, (index - 1).ToString());
                    if (index - 2 >= 0)
                    {
                        dicIndex[dataTables[index - 2].opsn] += ":" + (index - 2).ToString();
                    }
                }
            }
            //记录数据段末尾
            dicIndex[dataTables[index - 2].opsn] += ":" + (index - 2).ToString();
            timer2.Enabled = true;
        }

        /// <summary>
        /// 重画曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDataProcessForm_SizeChanged(object sender, EventArgs e)
        {
            if (dataPicture.isLoad)
            {
                dataPicture.xline_pick = 0;
                dataPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height);


                //画坐标层
                pictureBoxScope_axis();

                //画曲线
                pictureBoxScope_draw();

            }
        }

        /// <summary>
        /// 画坐标层
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void pictureBoxScope_axis()
        {
            //层图
            Bitmap img = new Bitmap(dataPicture.Width, dataPicture.Height);

            Scope_axis(img);
            //铺图
            pictureBox1.BackgroundImage = img;
        }

        /// <summary>
        /// 画曲线
        /// </summary>
        private void pictureBoxScope_draw()
        {
            //层图
            Bitmap img = new Bitmap(dataPicture.Width, dataPicture.Height);

            Scope_draw(img);
            //铺图
            pictureBox1.Image = img;
        }

        /// <summary>
        /// 鼠标点击图片,显示轴线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">位置</param>
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //取消选中
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                dataGridView1.SelectedRows[i].Selected = false;
            }

            //获取点击y的坐标
            dataPicture.yline_pick = e.Y;

            //获取选中的时间戳，计算顶层
            string str = dataPicture.getViewIdx_pictureBox(e.X);

            //设置是否选中的数据在表中是没有的
            if (str == null || str == "")
            {
                dataPicture.xline_pick = 0;
            }
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (str.Equals(dataGridView1.Rows[i].Cells[1].Value.ToString()))
                    {
                        //峰值
                        torqueMax = dicTorque[dataGridView1.Rows[i].Cells[2].Value.ToString()];

                        //选中表格
                        dataGridView1.Rows[i].Selected = true;
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
            //画顶层
            pictureBoxScope_draw();
        }

        /// <summary>
        /// 鼠标点击表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                //设置文件为只读文件
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);

                //构造窗口
                MenuExportReportForm exportReportForm = new MenuExportReportForm();
                PrepReport(exportReportForm);
            }
        }

        /// <summary>
        /// 启动，页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDataProcessForm_Load(object sender, EventArgs e)
        {
            dataPicture.xline_pick = 0;
            index = 1;

            if (MyDefine.myXET.DPT.Count > 1)
            {
                //触发加载数据timer
                timer1.Enabled = true;

                //表格初始化
                dataUpdate();
            }


            //注册鼠标滚动事件
            pictureBox1.MouseWheel += new MouseEventHandler(PictureBox_Show_MouseWheel);

            //Main主菜单,加载数据,修改规程,等操作后需要更新本地窗口
            MyDefine.myXET.dataUpdate += new freshHandler(dataForm_Update);

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDataProcessForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyDefine.myXET.dataUpdate -= new freshHandler(dataForm_Update);
        }

        #region 导出pdf

        /// <summary>
        /// 点击生成报告事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_Report_Click(object sender, EventArgs e)
        {
            //构造窗口
            MenuExportReportForm exportReportForm = new MenuExportReportForm();

            //初始化数据
            exportReportForm.bt_click = true;
            PrepReport(exportReportForm);
        }

        /// <summary>
        /// 创建段落
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public Paragraph CreateParagraph(string str, iTextSharp.text.Font font, int align)
        {
            Paragraph mp = new Paragraph(str, font);

            mp.Alignment = align;
            mp.SpacingBefore = 5.0f;
            mp.SpacingAfter = 5.0f;

            return mp;
        }

        /// <summary>
        /// 字符串长度增加add值并控制等长，最小min
        /// </summary>
        /// <param name="str"></param>
        /// <param name="min"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public int GetJoinLen(List<String> str, int min, int add)
        {
            int max = min;
            int len = 0;

            for (int i = 0; i < str.Count; i++)
            {
                len = Encoding.Default.GetBytes(str[i]).Length;
                if (len > max)
                {
                    max = len;
                }
            }

            max += add;

            return max;
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

        //生成报告准备工作
        private void PrepReport(MenuExportReportForm exportReportForm)
        {
            idex[0] = new int[100];
            idex[1] = new int[100];

            //打开窗口用户输入
            exportReportForm.StartPosition = FormStartPosition.CenterParent;
            exportReportForm.ShowDialog();
            this.BringToFront();

            if (dataTables.Count > 0 && exportReportForm.report)
            {
                this.reportFileName = exportReportForm.reportFileName;
                this.reportCompany = exportReportForm.reportCompany;
                this.reportLoad = exportReportForm.reportLoad;
                this.reportCommodity = exportReportForm.reportCommodity;
                this.reportStandard = exportReportForm.reportStandard;
                this.reportOpsn = exportReportForm.reportOpsn;
                this.reportDate = exportReportForm.reportDate;

                if (reportOpsn != null && reportOpsn != "")
                {
                    try
                    {
                        string[] ag = dicIndex[reportOpsn].Split(':');
                        idex[0][0] = Convert.ToInt32(ag[0]);
                        idex[1][0] = Convert.ToInt32(ag[1]);
                        if (idex[1][0] - idex[0][0] > DataPicture.minLength)
                        {
                            //生成报告
                            CreatDataPdf(1);
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

                }
                else
                {
                    int i = -1;
                    foreach (var item in dicIndex.Values)
                    {
                        string[] ag = item.Split(':');
                        report_start = Convert.ToInt32(ag[0]);
                        report_stop = Convert.ToInt32(ag[1]);
                        if (report_stop - report_start > DataPicture.minLength)
                        {
                            i++;
                            idex[0][i] = report_start;
                            idex[1][i] = report_stop;
                        }
                    }
                    if (i >= 0)
                    {
                        //生成报告
                        CreatDataPdf(++i);
                    }
                    else
                    {
                        if (MyDefine.myXET.languageNum == 0)//语言选择为中文时
                        {
                            MessageBox.Show("没有符合要求的数据");
                        }
                        else
                        {
                            MessageBox.Show("There is no data that meets the requirements.");
                        }
                        return;
                    }
                }
            }
        }

        // 生成报告
        private void CreatDataPdf(int count)
        {
            #region 参数
            int page = 0;//记录页数
            const int LENL = 24;//设置数据占位长度
            const int LENS = 16;
            int myMax = 55; ;//获取最大长度
            String blankLine = " ";

            List<String> myLS = new List<String>();
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
                if (dr == DialogResult.No)
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
                if (MyDefine.myXET.languageNum == 0)//语言选择为中文
                {
                    MessageBox.Show("请先关闭该文档", "提示");
                }
                else
                {
                    MessageBox.Show("Please close this document first", "Hint");
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

            #region 页处理
            for (int i = 0; i < count; i++)
            {
                #region 数据处理

                //设置峰值、角度、单位、操作模式的数据
                reportOpsn = dataTables[idex[0][i]].opsn;
                reportPeak = dicTorque[reportOpsn].ToString();
                reportAngle = dicAngle[reportOpsn].ToString();
                reportUnit = dataTables[idex[0][i]].data[8];
                reportMode = dataTables[idex[0][i]].data[5];

                //获取角速度单位
                try
                {
                    reportAngleSpeed = dataTables[idex[0][i]].data[9];
                }
                catch
                {
                    reportAngleSpeed = "";
                }
                //获取时间戳
                DateTime dt = DateTime.ParseExact(dataTables[idex[0][i]].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                reportStamp = dt.ToString("HH:mm:ss");
                dt = DateTime.ParseExact(dataTables[idex[1][i]].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                reportStamp += "-" + dt.ToString("HH:mm:ss");

                #endregion

                #region 每页的处理
                //添加元素
                document.Add(CreateParagraph("Page" + (++page), fontMessage, Element.ALIGN_RIGHT));
                //document.Add(new Paragraph(blankLine, fontItem));

                document.Add(CreateParagraph(reportCompany, fontTitle, Element.ALIGN_CENTER));//标题（公司名称）
                document.Add(new Paragraph(blankLine, fontItem));
                document.Add(CreateParagraph("扭力测试曲线报告", fontItem, Element.ALIGN_CENTER));//副标题（报告名称）
                document.Add(CreateParagraph("Torque test curve Report", fontItem, Element.ALIGN_CENTER));//副标题（报告名称）

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
                //myMax = GetJoinLen(myLS, LENL, 24);
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
                //myMax = GetJoinLen(myLS, LENL, 24);
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
                document.Add(CreateParagraph("扭矩角度曲线图", fontItem, Element.ALIGN_CENTER));
                document.Add(CreateParagraph("Torque angle graph", fontItem, Element.ALIGN_CENTER));
                document.Add(new Paragraph(blankLine, fontItem));

                //画图的点
                List<Point> point_torque = new List<Point>();

                //计算数据
                for (int j = 0; j <= idex[1][i] - idex[0][i]; j++)
                {
                    point_torque.Add(new Point(j + 28, (int)(320 - float.Parse(dataTables[j + idex[0][i]].torque) * 8)));
                }

                //画图
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(GetBitmapPoint(dataTables[idex[0][i]].unit, reportStamp, point_torque), System.Drawing.Imaging.ImageFormat.Bmp);
                //image.ScaleToFit(504, 374);
                document.Add(image);
                if (i != count - 1)
                {
                    //更新页数和创建新页
                    document.NewPage();
                    document.ResetPageCount();
                }
                #endregion
            }

            #endregion

            #region 关闭

            //关闭
            document.Close();
            writer.Close();

            //调出pdf
            Process.Start(fileDialog.FileName);
            #endregion

        }

        /// <summary>
        /// 绘制图片
        /// </summary>
        /// <param name="munit">扭矩单位</param>
        /// <param name="mtime">时间戳</param>
        /// <param name="pointTorque">扭矩点坐标集合</param>
        /// <returns></returns>
        public Bitmap GetBitmapPoint(string munit, string mtime, List<Point> pointTorque)
        {
            const int Width = 510;
            const int Height = 320;
            const int Info = 20;

            //第idx条横线
            int idx = 1;

            //刻度温度值
            int tmp = 30;

            //刻度Y坐标
            int yline = idx * 80;

            //颜色
            Color color = Color.Green;

            //层图
            Bitmap img = new Bitmap(Width, Height + Info);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //填充白色
            g.FillRectangle(Brushes.White, 0, 0, Width, Height + Info);

            //画刻度
            while (yline <= Height)
            {
                yline = idx * 80;
                //画横线
                g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(28, yline), new Point(Width - 5, yline));

                //扭矩刻度
                g.DrawString(tmp.ToString() + munit, new System.Drawing.Font("Arial", 6), Brushes.Black, 0, yline - 6);

                idx++;
                tmp -= 10;
            }
            //画x轴，加箭头
            //g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(28, Height), new Point(Width - 5, Height));
            g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(Width - 10, Height - 3), new Point(Width - 5, Height));
            g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(Width - 10, Height + 3), new Point(Width - 5, Height));

            //画y轴
            g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(28, 10), new Point(28, Height));
            g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(28, 10), new Point(25, 15));
            g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(28, 10), new Point(31, 15));

            //画扭矩线
            g.DrawCurve(new Pen(color, 1.0f), pointTorque.ToArray(), 0);
            //画横坐标点
            g.DrawString(mtime.Split('-')[0], dataPicture.font_txt, Brushes.Black, 28 - 10, 340 - 12);
            g.DrawString(mtime.Split('-')[1], dataPicture.font_txt, Brushes.Black, 28 + pointTorque.Count - 10, 340 - 12);

            g.Dispose();

            return img;
        }

        #endregion

        // 坐标层绘制
        private void Scope_axis(Bitmap img)
        {
            if (MyDefine.myXET.DPT.Count <= 0)
            {
                return;
            }
            String myAngStr;
            try
            {
                myAngStr = MyDefine.myXET.DPT[1].data[9];
            }
            catch
            {
                myAngStr = "";
            }
            String myTorStr = MyDefine.myXET.DPT[1].data[8];
            SizeF mySize;

            int left; //计算刻度线的左右坐标
            int right; //计算刻度线的左右坐标

            //绘制
            Graphics g = Graphics.FromImage(img);

            //角度圆
            g.DrawEllipse(new Pen(dataPicture.color_axis, 4.0f), dataPicture.angCxStart, dataPicture.angCyStart, dataPicture.angDiameter, dataPicture.angDiameter);
            g.DrawEllipse(new Pen(dataPicture.color_angle_init, 6.0f), dataPicture.angCentreX - 3, dataPicture.angCentreY - 3, 6, 6);
            //扭矩圆
            g.DrawEllipse(new Pen(dataPicture.color_axis, 4.0f), dataPicture.torCxStart, dataPicture.torCyStart, dataPicture.torDiameter, dataPicture.torDiameter);
            g.DrawEllipse(new Pen(dataPicture.color_torque_init, 6.0f), dataPicture.torCentreX - 3, dataPicture.torAxisY - 3, 6, 6);

            //角轴轴线
            left = dataPicture.angAxisX - (dataPicture.boardGap / 4);
            right = dataPicture.angAxisX + (dataPicture.boardGap / 4);
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(dataPicture.angAxStart, dataPicture.angAxisY), new Point(dataPicture.angAxStop, dataPicture.angAxisY));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(dataPicture.angAxisX, dataPicture.angAyStart), new Point(dataPicture.angAxisX, dataPicture.angAyStop));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.angLine0), new Point(right, dataPicture.angLine0));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.angLine1), new Point(right, dataPicture.angLine1));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.angLine2), new Point(right, dataPicture.angLine2));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.angLine3), new Point(right, dataPicture.angLine3));
            //扭矩轴线
            left = dataPicture.torAxisX - (dataPicture.boardGap / 4);
            right = dataPicture.torAxisX + (dataPicture.boardGap / 4);
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(dataPicture.torAxStart, dataPicture.torAxisY), new Point(dataPicture.torAxStop, dataPicture.torAxisY));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(dataPicture.torAxisX, dataPicture.torAyStart), new Point(dataPicture.torAxisX, dataPicture.torAyStop));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.torLine0), new Point(right, dataPicture.torLine0));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.torLine1), new Point(right, dataPicture.torLine1));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.torLine2), new Point(right, dataPicture.torLine2));
            g.DrawLine(new Pen(dataPicture.color_axis, 1.0f), new Point(left, dataPicture.torLine3), new Point(right, dataPicture.torLine3));

            //文字角度文字
            mySize = g.MeasureString(myAngStr, new System.Drawing.Font("Courier New", 11));
            g.DrawString(myAngStr, new System.Drawing.Font("Courier New", 11), dataPicture.brush_angle_init, dataPicture.angCentreX - mySize.Width / 2, dataPicture.yline_angleSpeed);

            //文字扭矩
            mySize = g.MeasureString(myTorStr, new System.Drawing.Font("Courier New", 11));
            g.DrawString(myTorStr, new System.Drawing.Font("Courier New", 11), dataPicture.brush_torque_init, dataPicture.torCentreX - mySize.Width / 2, dataPicture.yline_torqueUnit);
            //
            g.Dispose();
        }

        // 曲线绘制
        public void Scope_draw(Bitmap img)
        {
            String myLevStr;
            String myStr;
            SizeF mySize;

            //层图
            // Bitmap img = new Bitmap(dataPicture.Width, dataPicture.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //画线,选中的点
            if (dataPicture.xline_pick > 0)
            {
                g.DrawLine(new Pen(dataPicture.color_info, 1.0f), new Point(dataPicture.xline_pick, 0), new Point(dataPicture.xline_pick, dataPicture.Height));
            }
            //画角度曲线
            if (dataPicture.angPoint.Count > 1)
            {
                g.DrawCurve(new Pen(dataPicture.color_angle, 2.0f), dataPicture.angPoint.ToArray(), 0);
            }
            //画扭矩曲线
            if (dataPicture.torPoint.Count > 1)
            {
                g.DrawCurve(new Pen(dataPicture.color_torque, 2.0f), dataPicture.torPoint.ToArray(), 0);
            }
            //画角度仪表盘
            g.DrawLine(new Pen(dataPicture.color_angle, 5.0f), new Point(dataPicture.angCentreX, dataPicture.angCentreY), new Point((int)dataPicture.angArrowX, (int)dataPicture.angArrowY));
            //画扭矩仪表盘
            g.DrawArc(new Pen(dataPicture.color_torque, 5.0f), dataPicture.torCxStart, dataPicture.torCyStart, dataPicture.torDiameter, dataPicture.torDiameter, 90, dataPicture.torArcEnd);

            //显示统计值
            if (dataPicture.xline_pick > 0)
            {
                int px = dataPicture.xline_pick + 3;
                //获取时间
                DateTime dt = DateTime.ParseExact(dataTables[bemPick].stamp, "HHmmssfff", System.Globalization.CultureInfo.CurrentCulture);
                if (MyDefine.myXET.languageNum == 0)
                {
                    g.DrawString("时间: " + dt.ToString("HH:mm:ss"), dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 20);
                    g.DrawString("扭矩: " + dataTables[bemPick].torque + " " + MyDefine.myXET.DPT[bemPick + 1].data[8], dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 40);
                    g.DrawString("角度: " + dataTables[bemPick].angle + "°", dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 60);
                    g.DrawString("峰值：" + torqueMax.ToString() + " " + MyDefine.myXET.DPT[bemPick + 1].data[8], dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 80);
                }
                else
                {
                    g.DrawString("Time: " + dt.ToString("HH:mm:ss"), dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 20);
                    g.DrawString("Torque: " + dataTables[bemPick].torque + " " + MyDefine.myXET.DPT[bemPick + 1].data[8], dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 40);
                    g.DrawString("Angle: " + dataTables[bemPick].angle + "°", dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 60);
                    g.DrawString("Peak: " + torqueMax.ToString() + " " + MyDefine.myXET.DPT[bemPick + 1].data[8], dataPicture.font_txt, dataPicture.brush_info, px, dataPicture.yline_pick + 80);
                }
            }

            //文字角度
            myStr = (MyDefine.myXET.DEV.angle / 10.0f).ToString("f1");
            mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold));
            g.DrawString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold), dataPicture.brush_angle, dataPicture.angCentreX - mySize.Width / 2, dataPicture.yline_angle);

            //文字扭矩
            myStr = (MyDefine.myXET.DEV.torque / 100.0f).ToString("f2");
            mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold));
            g.DrawString(myStr, new System.Drawing.Font("Courier New", 40, FontStyle.Bold), dataPicture.brush_torque, dataPicture.torCentreX - mySize.Width / 2, dataPicture.yline_torque);

            //
            if (MyDefine.myXET.DEV.modePt == 0)
            {
                //文字角度max
                myStr = (dataPicture.angleMax / 10.0f).ToString("f1") + "(MAX)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), dataPicture.brush_angle, dataPicture.angCentreX - mySize.Width / 2, dataPicture.yline_anglePeak);

                //文字扭矩max
                myStr = (dataPicture.torqueMax / 100.0f).ToString("f2") + "(MAX)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), dataPicture.brush_torque, dataPicture.torCentreX - mySize.Width / 2, dataPicture.yline_torquePeak);
            }
            else
            {
                //文字角度peak
                myStr = (MyDefine.myXET.DEV.anglePeak / 10.0f).ToString("f1") + "(PEAK)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), dataPicture.brush_angle, dataPicture.angCentreX - mySize.Width / 2, dataPicture.yline_anglePeak);

                //文字扭矩peak
                myStr = (MyDefine.myXET.DEV.torquePeak / 100.0f).ToString("f2") + "(PEAK)";
                mySize = g.MeasureString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold));
                g.DrawString(myStr, new System.Drawing.Font("Courier New", 16, FontStyle.Bold), dataPicture.brush_torque, dataPicture.torCentreX - mySize.Width / 2, dataPicture.yline_torquePeak);
            }

            //
            g.Dispose();
        }

        //获取选点表格对应的流水号数据
        public void dataGridView()
        {
            int index;
            //取消标线
            dataPicture.xline_pick = 0;

            //初始化下标
            dataPicture.start = 10000;
            dataPicture.stop = -10000;

            //曲线放大倍数为1
            dataPicture.rate = 1;
            dataPicture.start_x = 0;
            dataPicture.old_rate = 1;

            if (dataGridView1.CurrentRow.Index < dataTables.Count)
            {
                //清空记录表格内容
                dataPicture.bem_stamp.Clear();
                dataPicture.bem_angle.Clear();
                dataPicture.bem_torque.Clear();

                //获取鼠标点击表的位置
                index = dataGridView1.CurrentRow.Index;
                dataGridView1.Rows[index].Selected = true;

                //获取流水号 
                string str = dataGridView1.Rows[index].Cells[2].Value.ToString();

                //获取相应流水号的起止范围
                string[] idx = dicIndex[str].Split(':');
                dataPicture.start = Convert.ToInt32(idx[0]);
                dataPicture.stop = Convert.ToInt32(idx[1]);
                if (dataPicture.stop - dataPicture.start > DataPicture.minLength)
                {
                    for (int i = dataPicture.start; i <= dataPicture.stop; i++)
                    {
                        dataPicture.bem_stamp.Add((string)dataTables[i].stamp);
                        dataPicture.bem_torque.Add((int)(Convert.ToDouble(dataTables[i].torque) * 100));
                        dataPicture.bem_angle.Add((int)(Convert.ToDouble(dataTables[i].angle) * 10));
                    }

                    //计算数据点
                    dataPicture.getPoint_pictureBox(dataTables);
                    //画顶层
                    pictureBoxScope_draw();
                }
            }
        }

        //鼠标滚动控制放大缩小
        private void PictureBox_Show_MouseWheel(object sender, MouseEventArgs e)
        {
            dataPicture.old_rate = dataPicture.rate;
            int rate_x = e.X;
            int x_t;
            if (e.Delta > 0)
            {
                if (dataPicture.rate >= 1)
                {
                    dataPicture.rate *= 1.2f;
                    dataPicture.rate = (float)Math.Round(dataPicture.rate, 2);//四舍五入，保留两位小数
                    if (dataPicture.rate >= 5.18f)
                    {
                        dataPicture.rate = 5.18f;
                    }
                }
                else
                {
                    dataPicture.rate += 0.1f;
                }
            }
            if (e.Delta < 0)
            {
                if (dataPicture.rate > 1)
                {
                    dataPicture.rate /= 1.2f;
                }
                else
                {
                    dataPicture.rate -= 0.1f;
                    if (dataPicture.rate <= 0.5f)
                    {
                        dataPicture.rate = 0.5f;
                    }
                }
                dataPicture.rate = (float)Math.Round(dataPicture.rate, 2);
            }
            //计算放大位置
            x_t = (int)Math.Round((rate_x - dataPicture.pointStart) / dataPicture.old_rate, 0) + dataPicture.start_x;
            x_t = x_t > 0 ? x_t - (int)Math.Round((rate_x - dataPicture.pointStart) / dataPicture.rate, 0) : 0;
            x_t = x_t > 0 ? x_t : 0;
            dataPicture.start_x = x_t;

            timer2.Enabled = true;
        }
        //复原
        private void button1_Click(object sender, EventArgs e)
        {
            //取消标线
            dataPicture.xline_pick = 0;

            //初始化下标
            dataPicture.start = 10000;
            dataPicture.stop = -10000;

            //曲线放大倍数为1
            dataPicture.rate = 1;
            dataPicture.start_x = 0;
            dataPicture.old_rate = 1;
            //计算数据点
            dataPicture.getPoint_pictureBox(dataTables);
            //画顶层
            pictureBoxScope_draw();

        }
    }
}
