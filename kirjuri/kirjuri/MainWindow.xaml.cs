using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace kirjuri
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, BillEntry> bills = new Dictionary<string, BillEntry>();
        private List<BankStatementEntry> bankStatements = new List<BankStatementEntry>();
        private string ledgerName;
        public MainWindow()
        {
            InitializeComponent();
            ledgerName = "";
            Debug.WriteLine("Kirjuri started");
        }
        private string GetFilename(string title)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void BtnLoadLedger_Click(object sender, RoutedEventArgs e)
        {
            ledgerName = GetFilename("Lataa tilikirja");
            //Load accounts from ledger
            CheckPreRequisites();

        }

        private void BtnLoadBillList_Click(object sender, RoutedEventArgs e)
        {

            string billListFileName = GetFilename("Lataa laskulista");
            if (billListFileName != "")
            {
                foreach (string line in File.ReadLines(billListFileName, Encoding.GetEncoding("iso-8859-1")).Skip(1))
                {
                    Debug.WriteLine(line);
                    BillEntry billEntry = new BillEntry();
                    billEntry.InitBill(line);
                    bills.Add(billEntry.ReferenceNumber, billEntry);
                    Debug.WriteLine(billEntry.Amount);

                }
                CheckPreRequisites();
            }
            else
            {
                MessageBox.Show("Lasku listaa ei saatu auki");
            }
        }

        private void BtnLoadBankStatements_Click(object sender, RoutedEventArgs e)
        {
            textBlckBankStatement.Text = ledgerName;
            string bankStatementsFileName = GetFilename("Lataa tiliote");
            if (bankStatementsFileName != "")
            {
                foreach (string line in File.ReadLines(bankStatementsFileName, Encoding.GetEncoding("iso-8859-1")).Skip(1))
                {
                    Debug.WriteLine(line);
                    BankStatementEntry bankStatementEntry = new BankStatementEntry();
                    bankStatementEntry.initBankStatementEntry(line);
                    bankStatements.Add(bankStatementEntry);

                }
                bankStatements.Reverse();
                /*foreach(BankStatementEntry entry in bankStatements)
                {
                    Debug.WriteLine(String.Format("{0} {1} {2} {3}", entry.Date, entry.DescriptionMSG, entry.Amount, entry.FromTo));
                }*/
                CheckPreRequisites();
            }
            else
            {
                MessageBox.Show("Lasku listaa ei saatu auki");
            }
        }


        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            int eventN = 1;
            string ledgerEntryTemplate = "\n\t\t\tevent {0}\n\t\t\t\tdate {1}\n\t\t\t\t\"{2}\"\n\t\t\t\t\n\t\t\t\t\t{3}\n\t\t\t\t\t\tmoney {4}\n\t\t\t\t\t{5}\n\t\t\t\t\t\tmoney {6}";
            foreach (BankStatementEntry entry in bankStatements)
            {
                string internalAccount = "";
                if (bills.ContainsKey(entry.DescriptionMSG))
                {
                    entry.DescriptionMSG = string.Format("{0} {1}",
                        bills[entry.DescriptionMSG].Customer,
                        bills[entry.DescriptionMSG].Description);
                }
                string ledgerEntry = string.Format(ledgerEntryTemplate, 
                    eventN, 
                    entry.Date, entry.DescriptionMSG,
                    "10003",
                    entry.Amount,
                    internalAccount,
                    entry.Amount*-1);
                Debug.WriteLine(ledgerEntry);

                //File.AppendAllText(@ledgerName, "text content");
                eventN += 1;
            }
        }

        private void CheckPreRequisites()
        {
            int flag = 0;
            if(ledgerName != "")
            {
                flag += 1;
            }
            if (bankStatements.Count() > 0)
            {

                flag += 1;
            }
            if (flag == 2)
            {
                btnOK.IsEnabled = true;
            }
        }
    }
}
