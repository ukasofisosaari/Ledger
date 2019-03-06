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
        private string _ledgerName;
        private int _eventN;
        private int _statementN;
        private string _ledgerEntry;
        private List<String> _internalAccounts = new List<string>();
        public String SelectedAccount { get; set; }
        public List<String> InternalAccounts
        {
            get
            {
                return _internalAccounts;
            }

            set
            {
                _internalAccounts = value;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _ledgerName = "";
            Debug.WriteLine("Kirjuri started");
            LoadInternalAccounts();
            DataContext = this;
        }

        private void LoadInternalAccounts()
        {
            foreach (string line in File.ReadLines("tilit.csv", Encoding.GetEncoding("iso-8859-1")).Skip(1))
            {
                _internalAccounts.Add(line);
            }
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
            _ledgerName = GetFilename("Lataa tilikirja");
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
            textBlckBankStatement.Text = _ledgerName;
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

        private void CheckPreRequisites()
        {
            //Tämä ei toimi, tee bitti operaatioilla! TODO
            int flag = 0;
            if(_ledgerName != "")
            {
                flag += 1;
            }
            if (bankStatements.Count() > 0)
            {

                flag += 1;
            }
            if (flag == 2)
            {
                btnStart.IsEnabled = true;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _eventN = 1;
            _statementN = 0;
            LoadNextStatement();
            btnOK.IsEnabled = true;
            btnStart.IsEnabled = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            //Save current statement info as event
            _eventN += 1;
            _statementN += 1;

            BankStatementEntry entry = bankStatements[_statementN];
            LoadNextStatement();
            string internalAccount = SelectedAccount.Split(' ')[0];
            string ledgerEntryTemplate = "\n\t\t\tevent {0}\n\t\t\t\tdate {1}\n\t\t\t\t\"{2}\"\n\t\t\t\t\n\t\t\t\t\t{3}\n\t\t\t\t\t\tmoney {4}\n\t\t\t\t\t{5}\n\t\t\t\t\t\tmoney {6}";
            _ledgerEntry = string.Format(ledgerEntryTemplate, 
                _eventN,
                entry.Date,
                textBoxDescriptor.Text,
                "10003",
                entry.Amount,
                internalAccount,
                entry.Amount * -1);
            Debug.WriteLine(_ledgerEntry);
            File.AppendAllText(_ledgerName, _ledgerEntry);
            SelectedAccount = "";
        }

        private void LoadNextStatement()
        {
            BankStatementEntry entry = bankStatements[_statementN];
            labelEntryNumber.Content = _statementN.ToString();
            if (bills.ContainsKey(entry.DescriptionMSG))
            {
                entry.InternalAccount = _internalAccounts.FirstOrDefault(s => s.Contains(bills[entry.DescriptionMSG].Internal_Account));
                entry.DescriptionMSG = string.Format("{0} {1}",
                    bills[entry.DescriptionMSG].Customer,
                    bills[entry.DescriptionMSG].Description);
                SelectedAccount = entry.InternalAccount;

            }
            textBoxDescriptor.Text = entry.DescriptionMSG;
            textBlckBankStatement.Text = string.Format("Bank statement info:\nMessage: {0}\nSender: \n{1}\nSum: {2}\nDate: {3}\n", 
                entry.DescriptionMSG,
                entry.FromTo,
                entry.Amount,
                entry.Date);
        }
    }
}
