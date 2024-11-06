using System;
using System.Runtime.InteropServices;

namespace BEM4
{
    //数据转换使用
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public partial class UIT
    {
        [FieldOffset(0)] //低位
        public Byte b0;
        [FieldOffset(1)]
        public Byte b1;
        [FieldOffset(2)]
        public Byte b2;
        [FieldOffset(3)] //高位
        public Byte b3;

        [FieldOffset(0)]
        public Int16 s0;
        [FieldOffset(2)]
        public Int16 s1;

        [FieldOffset(0)]
        public UInt16 us0;
        [FieldOffset(2)]
        public UInt16 us1;

        [FieldOffset(0)]
        public Int32 i;

        [FieldOffset(0)]
        public UInt32 ui;

        [FieldOffset(0)]
        public float f;
    }
}

//end

