using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace kirjuri
{
    class BillEntry
    {
        public string ReferenceNumber;
        public double Amount;
        public string Bill_n;
        public string Customer;
        public string Description;

        public bool InitBill(string billRow)
        {
            try
            {
                string[] fields = billRow.Split(';');
                ReferenceNumber = fields[4];
                Customer = fields[3];
                Description = fields[9];
                //Debug.WriteLine(fields[7]);
                Amount = Convert.ToDouble( fields[7], System.Globalization.CultureInfo.InvariantCulture);
                Bill_n = fields[0];
                return true;
            }
            catch (System.IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}
