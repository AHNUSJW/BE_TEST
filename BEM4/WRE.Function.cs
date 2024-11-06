using System.Collections.Generic;

namespace BEM4
{
    public partial class Wrench
    {
        public Wrench()
        {
            DEV = new Device();
            REC = new List<Record>();
            REC.Clear();
            DPT = new List<DataProcessTable>();
            DPT.Clear();
        }
    }
}

//end
