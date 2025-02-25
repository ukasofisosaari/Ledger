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
        public string Internal_Account;

        public bool InitBill(string billRow)
        {
            try
            {

                Debug.WriteLine(billRow);
                string[] fields = billRow.Split(',');
                ReferenceNumber = fields[4].Replace(".00", "");
                Customer = fields[3];
                Description = fields[9];
                //Internal_Account = fields[10];
                Amount = Convert.ToDouble( fields[7], System.Globalization.CultureInfo.InvariantCulture);
                if(Amount < 0)
                {
                    Amount *= -1.0;
                }
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
