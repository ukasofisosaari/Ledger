using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kirjuri
{
    [Flags]
    public enum InformationFlags
    {
        None = 0,
        //Ledger has been given
        Ledger = 1,
        //Bill list has been given
        Bills = 2,
        //Bank statements have been given
        Bankstatements = 4,
        //Event number
        Event_N = 8,
        BillInfo = 6,
        All = 15
    }
}
