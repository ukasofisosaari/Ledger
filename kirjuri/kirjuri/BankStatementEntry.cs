using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace kirjuri
{
    class BankStatementEntry
    {
        public string Date;
        public string FromTo;
        public string TypeMSG;
        public string DescriptionMSG;
        public double Amount;
        public string InternalAccount;

        public bool initBankStatementEntry(string bankStatementEntry)
        {
            try
            {
                string[] fields = bankStatementEntry.Split(';');
                Debug.WriteLine(fields[0].Trim('"'));
                //Date = DateTime.ParseExact(fields[0].Trim('"'),
                //                  "dd.MM.yyyy",
                //                  System.Globalization.CultureInfo.InvariantCulture);
                Date = FormatDate(fields[0].Trim('"'));
                Debug.WriteLine(Date);
                FromTo = fields[1].Trim('"');
                TypeMSG = fields[2].Trim('"');
                DescriptionMSG = fields[3].Trim('"').Trim('\'').TrimStart('0');
                Amount = Convert.ToDouble( fields[4].Trim('"'));
                Debug.WriteLine(Amount);
                Debug.WriteLine(Amount.ToString("N2"));
                return true;
            }
            catch (System.IndexOutOfRangeException)
            {
                return false;
            }
        }
        private string FormatDate(string date)
        {
            string[] fields = date.Split('.');
            return string.Format("{0} {1} {2}", fields[2], fields[1], fields[0]);
        }
    }
}
