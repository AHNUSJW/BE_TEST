using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

//Lumi 20240717
//js 20241024

//用于导出数据
namespace BEM4W
{

    public class MyDataExporter
    {
        private static List<string> meCsvlines = new List<string>();
        private static string meCsvPath = string.Empty;

        public static string MeCsvPath { get => meCsvPath; set => meCsvPath = value; }

        //设置表头
        public static void OpenCsvDataFile()
        {
            meCsvlines.Clear();
            meCsvlines.Add("序号,时间,流水号,设备名称,报警设置,编号代码名称,目标扭矩范围,,实测扭矩,角度,模式,扭矩峰值,角度峰值,扭矩单位,角度挡位,合格判定");
        }

        //增加一行数据
        public static void SaveCsvDataFile(String str)
        {
            if (str.Length > 0)
            {
                meCsvlines.Add(str);
            }
        }

        //保存csv数据
        public static bool CloseCsvDataFile()
        {
            //for (int i = 1; i <= 10; i++)
            //{
            //    string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string serialNumber = $"SN{i:D4}";
            //    string deviceName = $"Device{i}";
            //    string alarmSetting = "A1M0";
            //    string codeName = $"Code{i:D3}";
            //    string targetTorqueRange = "10-20 Nm,";
            //    string actualTorque = $"{10 + i}";
            //    string angle = $"{5 * i}";
            //    string mode = "Auto";
            //    string peakTorque = $"{15 + i}";
            //    string peakAngle = $"{6 * i}";
            //    string torqueUnit = "Nm";
            //    string angleGear = "High";
            //    string passFail = i % 2 == 0 ? "Pass" : "Fail";

            //    string csvLine = $"{i},{time},{serialNumber},{deviceName},{alarmSetting},{codeName},{targetTorqueRange},{actualTorque},{angle},{mode},{peakTorque},{peakAngle},{torqueUnit},{angleGear},{passFail}";
            //    SaveCsvDataFile(csvLine);
            //}

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

            List<string> finalCsvLines = meCsvlines;
            //判断数据表输出模式
            switch (Properties.Settings.Default.DataExcelType)
            {
                case 1:
                    //导出完整
                    break;
                case 2:
                    //导出结果
                    finalCsvLines = FilterLastRecordForEachSnBat(meCsvlines);
                    // 清空每个流水号最后一条记录中的实测扭矩字段  
                    ClearTorqueForLastRecords(finalCsvLines);
                    break;
                case 0:
                    //不导出
                    return false;
                default:
                    break;
            }

            //更新合格判定
            finalCsvLines = UpdateTighteningResult(finalCsvLines);
            //调整表头格式
            finalCsvLines = AdjustHeaderFormat(finalCsvLines);
            //实测扭矩和扭矩峰值列交换
            finalCsvLines = SwapColumns(finalCsvLines, 8, 11);

            //写入
            try
            {
                if (File.Exists(meCsvPath))
                {
                    File.SetAttributes(meCsvPath, FileAttributes.Normal);
                }
                File.WriteAllLines(meCsvPath, finalCsvLines, Encoding.Default);
                File.SetAttributes(meCsvPath, FileAttributes.ReadOnly);
                meCsvlines.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //获取时间列
        public static string FormatDateTime(string opsn, string stamp)
        {
            // 检查字符串长度是否符合预期
            if (opsn.Length != 10 || stamp.Length != 9)
            {
                return string.Empty;
            }

            // 提取年月日
            int year = int.Parse(opsn.Substring(0, 2)) + 2000; // 假设是21世纪的年份
            int month = int.Parse(opsn.Substring(2, 2));
            int day = int.Parse(opsn.Substring(4, 2));

            // 提取时分秒
            int hour = int.Parse(stamp.Substring(0, 2));
            int minute = int.Parse(stamp.Substring(2, 2));
            int second = int.Parse(stamp.Substring(4, 2));

            // 使用DateTime构造一个日期时间对象
            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);

            // 格式化输出
            return dateTime.ToString("yyyy/M/d HH:mm:ss");
        }

        //获取报警设置
        public static string GetAXMX(string axmx)
        {
            var array = axmx.Split(',');

            // 检查数组长度是否符合预期
            if (array.Length != 2 && array.Length != 3)
            {
                return string.Empty;
            }
            else
            {
                return array[0];
            }
        }

        //获取目标扭矩范围（扭矩范围）
        public static string GetTorqueTargetPart1(string axmx)
        {
            var array = axmx.Split(',');

            // 检查数组长度是否符合预期
            if (array.Length != 2 && array.Length != 3)
            {
                return string.Empty;
            }
            else
            {
                return array[1];
            }
        }

        //获取目标扭矩范围（角度范围）
        public static string GetTorqueTargetPart2(string axmx)
        {
            var array = axmx.Split(',');

            // 检查数组长度是否符合预期
            if (array.Length != 3)
            {
                return string.Empty;
            }
            else
            {
                return array[2];
            }
        }

        // 只保留每个流水号的最后一条记录
        private static List<string> FilterLastRecordForEachSnBat(List<string> lines)
        {
            Dictionary<string, string> lastRecordMap = new Dictionary<string, string>();

            // 从第二行开始遍历（跳过表头）
            for (int i = 1; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] columns = line.Split(',');
                if (columns.Length > 2) // 确保有足够的列来获取流水号
                {
                    string snBat = columns[2]; // 流水号位于第三列
                    lastRecordMap[snBat] = line; // 更新或添加该流水号的最后一条记录
                }
            }

            // 构建新的列表，包含表头和每个流水号的最后一条记录
            List<string> filteredLines = new List<string> { lines[0] }; // 添加表头
            filteredLines.AddRange(lastRecordMap.Values); // 添加每个流水号的最后一条记录

            return filteredLines;
        }

        // 清空每个流水号最后一条记录中的实测扭矩字段
        private static void ClearTorqueForLastRecords(List<string> lines)
        {
            Dictionary<string, int> snBatToLastIndexMap = new Dictionary<string, int>();

            // 首先，构建流水号到最后一条记录索引的映射  
            for (int i = 1; i < lines.Count; i++)
            {
                string[] columns = lines[i].Split(',');
                if (columns.Length > 2)
                {
                    string snBat = columns[2]; // 假设流水号位于第三列  
                    snBatToLastIndexMap[snBat] = i; // 记录流水号的最后一条记录的索引  
                }
            }
            int torqueColumnIndex = 8; //实测扭矩的列数（从0开始）

            foreach (var kvp in snBatToLastIndexMap)
            {
                string[] columns = lines[kvp.Value].Split(',');
                if (columns.Length > torqueColumnIndex)
                {
                    columns[torqueColumnIndex] = string.Empty; // 清空实测扭矩字段  
                    lines[kvp.Value] = string.Join(",", columns); // 将修改后的列组合回字符串  
                }
            }
        }

        // 获取合格判定
        private static List<string> UpdateTighteningResult(List<string> lines)
        {
            // 创建一个字典来跟踪每个流水号的最后一条记录的索引
            Dictionary<string, int> serialNumberToLastIndexMap = new Dictionary<string, int>();
            for (int i = 1; i < lines.Count; i++) // 从1开始，跳过表头
            {
                var columns = lines[i].Split(',');
                if (columns.Length > 3) // 确保有足够的列
                {
                    string serialNumber = columns[2]; // 流水号位于第三列
                    serialNumberToLastIndexMap[serialNumber] = i; // 更新最后一条记录的索引
                }
            }

            // 修改每个流水号的最后一条记录的合格判定值为"ok"
            foreach (var lastIndex in serialNumberToLastIndexMap.Values)
            {
                var columns = lines[lastIndex].Split(',');
                if (columns.Length > 13)
                {
                    string alarmSetting = columns[4]; // 报警设置位于第五列
                    double peakTorque = double.Parse(columns[11]); // 扭矩峰值位于第十二列
                    double peakAngle = double.Parse(columns[12]); // 角度峰值位于第十三列
                    double targetTorqueRange;// 目标扭矩范围位于第七列
                    double targetAngleRange; // 角度范围位于第八列

                    switch (alarmSetting)
                    {
                        case "A1M0":
                            //A1M0模式满足目标扭矩合格
                            targetTorqueRange = ExtractValue(columns[6]);
                            if (peakTorque >= targetTorqueRange)
                            {
                                columns[columns.Length - 1] = "OK";
                            }
                            else
                            {
                                columns[columns.Length - 1] = "NG";
                            }
                            break;
                        case "A1M1":
                        case "A1M2":
                        case "A1M3":
                        case "A1M4":
                        case "A1M5":
                        case "A1M6":
                        case "A1M7":
                        case "A1M8":
                        case "A1M9":
                            //A1M1-A1M9模式满足扭矩上下限合格
                            // 获取目标扭矩范围的上下限
                            var (lowerBoundTorque, upperBoundTorque, _) = ExtractRangeValues(columns[6]);
                            // 判断扭矩峰值是否在范围内
                            if (peakTorque >= lowerBoundTorque && peakTorque <= upperBoundTorque)
                            {
                                columns[columns.Length - 1] = "OK";
                            }
                            else
                            {
                                columns[columns.Length - 1] = "NG";
                            }
                            break;
                        case "A2M0":
                            //A2M0模式先满足目标扭矩再满足目标角度
                            targetTorqueRange = ExtractValue(columns[6]);
                            targetAngleRange = ExtractValue(columns[7]);
                            if (peakTorque >= targetTorqueRange && peakAngle >= targetAngleRange)
                            {
                                columns[columns.Length - 1] = "OK";
                            }
                            else
                            {
                                columns[columns.Length - 1] = "NG";
                            }
                            break;
                        case "A2M1":
                        case "A2M2":
                        case "A2M3":
                        case "A2M4":
                        case "A2M5":
                        case "A2M6":
                        case "A2M7":
                        case "A2M8":
                        case "A2M9":
                            //A2M1 - A2M9模式先满足目标扭矩再满足角度上下限
                            targetTorqueRange = ExtractValue(columns[6]);
                            var (lowerBoundAngle, upperBoundAngle, _) = ExtractRangeValues(columns[7]);
                            if (peakTorque >= targetTorqueRange && peakAngle >= lowerBoundAngle && peakAngle <= upperBoundAngle)
                            {
                                columns[columns.Length - 1] = "OK";
                            }
                            else
                            {
                                columns[columns.Length - 1] = "NG";
                            }
                            break;
                        default:
                            break;
                    }
                    lines[lastIndex] = string.Join(",", columns); // 重新组合为一行
                }
            }

            return lines;
        }

        // 调整数据表的第一行第二行
        private static List<string> AdjustHeaderFormat(List<string> originalLines)
        {
            if (originalLines.Count == 0) return originalLines;
            List<string> modifiedLines = new List<string>
            {
                // 添加原始的第一行数据
                originalLines[0],
                // 在第一行数据后重复两次第一行数据，并在两行重复之间插入一行空行
                originalLines[1], // 插入第一次重复
                "", // 插入空行
                originalLines[1] // 插入第二次重复
            };

            // 添加剩余的数据行
            modifiedLines.AddRange(originalLines.Skip(2));

            return modifiedLines;
        }

        //交换列
        private static List<string> SwapColumns(List<string> lines, int index1, int index2)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var columns = lines[i].Split(',');
                if (columns.Length > Math.Max(index1, index2))
                {
                    string temp = columns[index1];
                    columns[index1] = columns[index2];
                    columns[index2] = temp;
                    lines[i] = string.Join(",", columns);
                }
            }
            return lines;
        }

        // 提取字符串中的数值部分
        private static double ExtractValue(string valueWithUnit)
        {
            // 使用正则表达式匹配字符串开头的数字部分（包括小数）
            var match = Regex.Match(valueWithUnit, @"^\d+(\.\d+)?");
            if (match.Success)
            {
                // 如果成功匹配到数字，尝试将其转换为double类型
                return double.TryParse(match.Value, out double numericValue) ? numericValue : 0;
            }
            else
            {
                // 如果没有匹配到数字，返回0
                return 0;
            }
        }

        //获取字符串中的范围
        private static (double, double, string) ExtractRangeValues(string rangeString)
        {
            // 使用正则表达式匹配范围内的两个数值和可选的单位
            // 正则表达式解释：
            // (\d+(\.\d+)?) 匹配一个数字，可能包含小数部分
            // ~             匹配范围符号
            // (\d+(\.\d+)?) 匹配另一个数字，可能包含小数部分
            // (.*)          匹配任意字符（包括单位），直到字符串末尾
            var match = Regex.Match(rangeString, @"(\d+(\.\d+)?)~(\d+(\.\d+)?)(.*)");
            if (match.Success)
            {
                // 将匹配到的字符串转换为double类型
                double lowerBound = double.Parse(match.Groups[1].Value);
                double upperBound = double.Parse(match.Groups[3].Value);
                string unit = match.Groups[5].Value.Trim(); // 去除单位前后的空格
                return (lowerBound, upperBound, unit);
            }
            else
            {
                // 如果匹配失败，返回(0, 0, "")表示无效的范围
                return (0, 0, "");
            }
        }
    }
}