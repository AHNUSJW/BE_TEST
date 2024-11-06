using System;
using System.Collections.Generic;

namespace BEM4
{
    public class DataTable
    {
        public const UInt16 TABLESIZE = 5000;

        public Int32 lines;
        public String stamp;
        public String opsn;
        public float torque;
        public String unit;
        public float angle;
        public String info;

        public DataTable(Int32 mline, String mstamp, String mopsn, UInt32 tor, UNIT munit, Int16 ang, String minfo)
        {
            lines = mline;
            stamp = mstamp;
            opsn = mopsn;
            torque = tor / 100.0f;
            switch (munit)
            {
                case UNIT.UNIT_nm: unit = "N·m"; break;
                case UNIT.UNIT_lbfin: unit = "lbf·in"; break;
                case UNIT.UNIT_lbfft: unit = "lbf·ft"; break;
                case UNIT.UNIT_kgcm: unit = "kgf·cm"; break;
            }
            angle = ang / 10.0f;
            info = minfo;
        }
    }
}
