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
using System.Text.RegularExpressions;

namespace kirjuri
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, BillEntry> bills = new Dictionary<string, BillEntry>();
        private List<BankStatementEntry> bankStatements = new List<BankStatementEntry>();

        private List<string> paidBills = new List<string>();
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

        private InformationFlags informationFlags;
        public event EventHandler NextLedgerStatement;
        private bool CsvLedger;
        public MainWindow()
        {
            InitializeComponent();
            _ledgerName = "";
            Debug.WriteLine("Kirjuri started");
            LoadInternalAccounts();
            DataContext = this;

            NextLedgerStatement += LoadNextStatement;
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
            string ext = System.IO.Path.GetExtension(_ledgerName);
            if(ext == ".csv")
            {
                CsvLedger = true;
            }
            if( ext == ".kitupiikki")
            {
                CsvLedger = false;
            }
            else
            {
                //TODO: Popup alerting not supported
            }
            //Load accounts from ledger
            CheckPreRequisites(InformationFlags.Ledger);

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
                CheckPreRequisites(InformationFlags.Bills);
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
                CheckPreRequisites(InformationFlags.Bankstatements);
            }
            else
            {
                MessageBox.Show("Lasku listaa ei saatu auki");
            }
        }

        private void CheckPreRequisites(InformationFlags flags)
        {
            informationFlags |= flags;
            if (informationFlags.HasFlag(InformationFlags.All))
            {
                btnStart.IsEnabled = true;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _eventN = Convert.ToInt32(textBoxEvent.Text) + 1;
            textBoxEvent.IsEnabled = false;
            Debug.WriteLine(_eventN);
            _statementN = 0;
            NextLedgerStatement(this, EventArgs.Empty);
            btnOK.IsEnabled = true;
            btnStart.IsEnabled = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (CsvLedger)
            {
                SaveOntoLedgerFile();
            }
            else
            {
                SaveOntoDB();
            }
            _eventN += 1;
            _statementN += 1;
            NextLedgerStatement(this, EventArgs.Empty);
            btnSavePaidBillsFile.IsEnabled = true;
        }

        private void SaveOntoLedgerFile()
        {
            BankStatementEntry entry = bankStatements[_statementN];
            string internalAccount = SelectedAccount.Split(' ')[0];
            Debug.WriteLine(internalAccount);
            Debug.WriteLine(SelectedAccount);
            string ledgerEntryTemplate = "\n\t\t\tevent {0}\n\t\t\t\tdate {1}\n\t\t\t\t\"{2}\"\n\t\t\t\t\n\t\t\t\t\t{3}\n\t\t\t\t\t\tmoney {4}00\n\t\t\t\t\t{5}\n\t\t\t\t\t\tmoney {6}00";
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
        }

        private void SaveOntoDB()
        {

        }

        private void LoadNextStatement(object sender, EventArgs e)
        {
            //Save current statement info as event
            BankStatementEntry entry = bankStatements[_statementN];
            labelEntryNumber.Content = _statementN.ToString();
            if (bills.ContainsKey(entry.DescriptionMSG))
            {
                string billReference = entry.DescriptionMSG;
                //entry.InternalAccount = _internalAccounts.FirstOrDefault(s => s.Contains(bills[entry.DescriptionMSG].Internal_Account));
                entry.DescriptionMSG = string.Format("{0} {1}",
                    bills[billReference].Customer,
                    bills[billReference].Description);
                //SelectedAccount = entry.InternalAccount;
                paidBills.Add(bills[billReference].Bill_n);

            }
            Debug.WriteLine(entry.Amount.ToString("N2"));
            Debug.WriteLine(entry.Amount);
            textBoxDescriptor.Text = entry.DescriptionMSG;
            textBlckBankStatement.Text = string.Format("Bank statement info:\nMessage: {0}\nSender: \n{1}\nSum: {2}\nDate: {3}\n", 
                entry.DescriptionMSG,
                entry.FromTo,
                entry.Amount.ToString("N2"),
                entry.Date);
        }

        private void BtnSavePaidBillsFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savePaidsBillsFile = new SaveFileDialog();

            savePaidsBillsFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            savePaidsBillsFile.FilterIndex = 2;
            savePaidsBillsFile.RestoreDirectory = true;

            if (savePaidsBillsFile.ShowDialog() == true)
            {
                    // Code to write the stream goes here.
                    StreamWriter writer = new StreamWriter(savePaidsBillsFile.FileName);
                    foreach (String s in paidBills)
                    {
                        writer.WriteLine(s);
                    }
                writer.Close();
            }
        }

        private void TextBoxEvent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxEvent_KeyUp(object sender, KeyEventArgs e)
        {

            CheckPreRequisites(InformationFlags.Event_N);
        }
    }
}
