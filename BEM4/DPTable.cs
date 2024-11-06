using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEM4
{
    public class DPTable
    {
        public string lines;//序号
        public string stamp;//时间戳
        public string opsn;//流水号
        public string torque;//扭矩
        public string angle;//角度
        public string modePt;//模式PEAK,TRACK
        public string torquePeak;//扭矩峰值
        public string anglePeak;//角度峰值
        public string unit;//扭矩单位
        public string angleSpeed;//角度挡位
        public string strAXMX;//报警设置
        public string torqueLH;//报警目标上下限

        public int count_data;//读取的数据个数
        public string[] data;//读取的数据集合
        

        public DPTable(int count_data, string[] data)
        {
            this.count_data = count_data;
            this.data = data;
        }

        public void dataSet()
        {
            lines = data[0];
            stamp = data[1];
            opsn = data[2];
            torque = data[3];
            angle = data[4];
            data[5] = data[5].Replace("����", "缓存");
            modePt = data[5];
            torquePeak = data[6];
            anglePeak = data[7];
            data[8] = data[8].Replace("��", "·");
            unit = data[8];
            try
            {
                data[9] = data[9].Replace("��", "°");//15��/sec
                data[11] = data[11].Replace("��", "·");
                anglePeak = data[9];
                strAXMX = data[10];
                torqueLH = data[11];
            }
            catch
            {
                anglePeak = "";
                anglePeak = "";
                torqueLH = "";
            }
            
        }
    }
}
