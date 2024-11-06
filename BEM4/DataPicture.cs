using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEM4
{
    public class DataPicture
    {
        //窗口大小
        public int Width;
        public int Height;
        public int boardHeight;
        public int boardWidth;
        public int boardGap;

        //图像比例
        public float rate = 1.0f;
        public float old_rate = 1.0f;
        //放大倍率时鼠标的位置
        public int start_x = 0;

        //导入数据的idx
        //public int dataStart = 10000;//选取数据起始idx
        //public int dataEnd = -10000; //选取数据结束idx
        public int start = 10000;//选取数据起始idx
        public int stop = -10000; //选取数据结束idx

        public bool isLoad = false;//数据已加载
        public const int minLength = 10;//流水号最小数据长度

        //绘图坐标
        public int xline_start; //选取数据起始idx之坐标X
        public int xline_stop;  //选取数据结束idx之坐标X
        public int xline_pick;  //选中数据idx之坐标X
        public int yline_pick;  //选中数据idx之坐标Y

        //角度圆坐标
        public int angDiameter; //角度圆直径
        public int angCentreY;  //角度圆心Y坐标
        public int angCentreX;  //角度圆心X坐标
        public int angCyStart;  //角度起始Y
        public int angCxStart;  //角度起始X

        //扭矩圆坐标
        public int torDiameter; //扭矩圆直径
        public int torCentreY;  //扭矩圆心Y坐标
        public int torCentreX;  //扭矩圆心X坐标
        public int torCyStart;  //扭矩起始Y
        public int torCxStart;  //扭矩起始X

        //角轴线坐标
        public int angAyStart;  //角度轴线开始
        public int angAyStop;   //角度轴线结束
        public int angAxisY;    //角度轴线
        public int angAxStart;  //角度轴线开始
        public int angAxStop;   //角度轴线结束
        public int angAxisX;    //角度轴线
        public int angLine0;    //角度刻度线
        public int angLine1;    //角度刻度线
        public int angLine2;    //角度刻度线
        public int angLine3;    //角度刻度线

        //扭矩线坐标
        public int torAyStart;  //扭矩线开始
        public int torAyStop;   //扭矩线结束
        public int torAxisY;    //扭矩线
        public int torAxStart;  //扭矩线开始
        public int torAxStop;   //扭矩线结束
        public int torAxisX;    //扭矩线
        public int torLine0;    //扭矩刻度线
        public int torLine1;    //扭矩刻度线
        public int torLine2;    //扭矩刻度线
        public int torLine3;    //扭矩刻度线

        //绘线水平区域
        public int pointStart;  //曲线起始
        public int pointLen;    //曲线水平长度

        //文字坐标
        public int xline_info;      //info坐标X
        public int yline_battery;   //info坐标Y
        public int yline_torqueErr; //info坐标Y
        public int yline_angleLevel;//info坐标Y
        public int yline_queueSize; //info坐标Y
        public int yline_angle;     //角度坐标Y
        public int yline_anglePeak; //角度坐标Y
        public int yline_angleSpeed;//角度坐标Y
        public int yline_torque;    //扭矩坐标Y
        public int yline_torquePeak;//扭矩坐标Y
        public int yline_torqueUnit;//扭矩坐标Y

        //仪表盘
        public double angArrowX;   //角度箭头坐标X
        public double angArrowY;   //角度箭头坐标Y
        public int torArcEnd;      //扭矩弧线终点
        public int angleMax;       //最大值
        public int torqueMax;      //最大值

        //曲线颜色
        public Color color_axis;
        public Color color_info;
        public Color color_angle;
        public Color color_angle_init;
        public Color color_torque;
        public Color color_torque_init;

        //文字颜色
        public Brush brush_info;
        public Brush brush_angle;
        public Brush brush_angle_init;
        public Brush brush_torque;
        public Brush brush_torque_init;

        //文字大小
        public Font font_txt;

        //数据
        public List<string> stamp = new List<string>();//流水号
        public List<int> torque = new List<int>();//扭矩
        public List<int> angle = new List<int>();//角度
        public List<string> bem_stamp = new List<string>();
        public List<int> bem_torque = new List<int>();
        public List<int> bem_angle = new List<int>();

        //画图的点
        public List<Point> torPoint = new List<Point>();
        public List<Point> angPoint = new List<Point>();

        //构造函数
        public DataPicture()
        {
            Width = 600;
            Height = 600;

            //Color.Red;
            //Color.Orange;
            //Color.Gold;
            //Color.LawnGreen;
            //Color.LimeGreen;
            //Color.Cyan;
            //Color.DeepSkyBlue;
            //Color.Fuchsia;
            //Color.FloralWhite;
            //Color.SpringGreen

            color_axis = Color.Silver;
            color_info = Color.Gold;
            color_angle = Color.LimeGreen;
            color_angle_init = Color.LimeGreen;
            color_torque = Color.Red;
            color_torque_init = Color.DeepSkyBlue;
            brush_info = Brushes.Gold;
            brush_angle = Brushes.LimeGreen;
            brush_angle_init = Brushes.LimeGreen;
            brush_torque = Brushes.Red;
            brush_torque_init = Brushes.DeepSkyBlue;

            font_txt = new System.Drawing.Font("Arial", 8);

            CalculateAxis();
        }

        //坐标参数
        private void CalculateAxis()
        {
            boardHeight = Height / 15;
            boardWidth = 20;
            boardGap = 10;

            //角度圆坐标
            angDiameter = (Height - boardHeight - boardHeight - boardHeight) / 2;
            angCentreY = boardHeight + (angDiameter / 2);
            angCentreX = boardWidth + (angDiameter / 2);
            angCyStart = boardHeight;
            angCxStart = boardWidth;

            //扭矩圆坐标
            torDiameter = angDiameter;
            torCentreY = angCentreY + boardHeight + torDiameter;
            torCentreX = angCentreX;
            torCyStart = angCyStart + boardHeight + torDiameter;
            torCxStart = angCxStart;

            //角轴线坐标
            angAyStart = boardGap;
            angAyStop = (Height - boardGap) / 2;
            angAxisY = angCentreY;
            angAxStart = boardWidth + angDiameter + boardGap;
            angAxStop = Width - boardGap;
            angAxisX = (angAxStart + angAxStop) / 2;
            angLine0 = angAxisY - (int)(angDiameter * 0.5);
            angLine1 = angAxisY - (int)(angDiameter * 0.25);
            angLine2 = angAxisY + (int)(angDiameter * 0.25);
            angLine3 = angAxisY + (int)(angDiameter * 0.5);

            //扭矩线坐标
            torAyStart = (Height + boardGap) / 2;
            torAyStop = Height - boardGap;
            torAxisY = torCyStart + torDiameter;
            torAxStart = angAxStart;
            torAxStop = angAxStop;
            torAxisX = (torAxStart + torAxStop) / 2;
            torLine0 = torCyStart;
            torLine1 = torCyStart + (int)(torDiameter * 0.25);
            torLine2 = torCyStart + (int)(torDiameter * 0.5);
            torLine3 = torCyStart + (int)(torDiameter * 0.75);

            //绘线水平区域
            pointStart = angAxStart;
            pointLen = angAxStop - angAxStart;

            //文字info坐标
            xline_info = angAxStart;
            yline_torqueErr = angCyStart + torDiameter + 10;
            yline_angleLevel = yline_torqueErr + 30;
            yline_queueSize = yline_torqueErr + 60;

            //文字角度坐标
            yline_angle = angCentreY - 70;
            yline_anglePeak = yline_angle - 30;
            yline_angleSpeed = angCentreY + 30;

            //文字扭矩坐标
            yline_torque = torCentreY - 45;
            yline_torquePeak = yline_torque - 30;
            yline_torqueUnit = torCentreY + 30;
            yline_battery = yline_torqueUnit + 30;

            angArrowX = angCentreX;
            angArrowY = angCentreY;

            //this.Clear();
        }

        //清除数据
        public void Clear()
        {
            stamp.Clear();
            torque.Clear();
            angle.Clear();
            torPoint.Clear();
            angPoint.Clear();
        }

        //计算坐标轴
        public void getAxis_pictureBox(int w, int h)
        {
            if (w < 200)
            {
                Width = 200;
            }
            else
            {
                Width = w;
            }

            if (h < 200)
            {
                Height = 200;
            }
            else
            {
                Height = h;
            }
            CalculateAxis();
        }

        //计算数据点
        public void getPoint_pictureBox(List<DPTable> rec)
        {
            int torqueTop = 3000;//扭矩

            if (rec.Count == 0)
            {
                return;
            }
            //清空数据

            Clear();
            //数据
            foreach (DPTable item in rec)
            {
                stamp.Add((string)item.stamp);
                angle.Add((int)(Convert.ToDouble(item.angle)* 10));
                torque.Add((int)(Convert.ToDouble(item.torque) * 100));
            }
            while (angle.Count > pointLen)
            {
                angle.RemoveAt(0);
            }
            while (torque.Count > pointLen)
            {
                torque.RemoveAt(0);
            }
            while (stamp.Count > pointLen)
            {
                stamp.RemoveAt(0);
            }

            //计算角度和扭矩的坐标点数组
            if (stop - start > minLength)
            {
                setPoint_torqueAndAngle(bem_angle, bem_torque, torqueTop);
            }
            else
            {
                setPoint_torqueAndAngle(angle, torque, torqueTop);
            }
            isLoad = true;
        }

        //计算角度和扭矩的坐标点数组
        public void setPoint_torqueAndAngle(List<int> ag, List<int> tq, int torqueTop)
        {
            int angleTop = 900;  //90度
            int halfCycle = 180;

            int head = 1; //找非0头尾
            int end = 1;  //找非0头尾
            //角度
            angPoint.Clear();
            for (int i = start_x; i < ag.Count; i++)
            {
                //上边 angleTop, angLine0
                //下边 angleMin, angAxisY
                angPoint.Add(new Point((int)((i - start_x) * rate + pointStart), (int)(angAxisY - (angAxisY - angLine0) * ag[i] / angleTop * rate)));
            }
            //扭矩
            torPoint.Clear();
            for (int i = start_x; i < tq.Count; i++)
            {
                //上边 torqueTop, torLine0
                //下边 torqueMin, torAxisY
                torPoint.Add(new Point((int)((i - start_x) * rate + pointStart), (int)(torAxisY - (torAxisY - torLine0) * tq[i] / torqueTop * rate)));
            }
            //最新值箭头坐标
            angArrowX = angCentreX + (angDiameter / 2 - boardGap) * System.Math.Cos(ag[ag.Count - 1] * System.Math.PI / 1800.0);
            angArrowY = angCentreY + (angDiameter / 2 - boardGap) * System.Math.Sin(ag[ag.Count - 1] * System.Math.PI / 1800.0);

            //最新值扭矩弧线终点
            if (ag[ag.Count - 1] < 0)
            {
                //逆时针拧扳手
                halfCycle = -180;
            }
            else if (ag[ag.Count - 1] > 0)
            {
                //顺时针拧扳手
                halfCycle = 180;
            }
            torArcEnd = halfCycle * tq[tq.Count - 1] / torqueTop;

            //找最大值
            torqueMax = 0;
            for (end = tq.Count - 1; end > 0; end--)
            {
                if (tq[end] > torqueMax)
                {
                    torqueMax = tq[end];
                }
                else if ((tq[end] == 0) && (torqueMax > 0))
                {
                    break;
                }
            }
            angleMax = 0;
            for (end = ag.Count - 1; end > 0; end--)
            {
                if (System.Math.Abs(ag[end]) > System.Math.Abs(angleMax))
                {
                    angleMax = ag[end];
                }
                else if ((ag[end] == 0) && (angleMax != 0))
                {
                    break;
                }
            }
        }

        //根据鼠标位置计算表格位置
        public string getViewIdx_pictureBox(int ix)
        {
            int index;
            ix -= pointStart;
            ix = (int)Math.Round(ix / rate ,0);
            xline_pick = (int)(ix * rate) + pointStart;
            //折算
            index = angPoint.FindIndex(item => item.X.Equals(xline_pick));

            if (index < 0)
            {
                //没有选点
                return null;
            }
            if (stop - start > minLength)
            {
                return bem_stamp[index + start_x];
            }
            else
            {
                return stamp[index + start_x];
            }
        }
    }
}
