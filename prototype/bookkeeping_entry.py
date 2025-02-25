from datetime import datetime

from PySide2.QtWidgets import QWidget, QInputDialog

BOOKKEEPING_BOOK_ENTRY_FORMAT = "\n\t\t\tevent {0}\n\t\t\t\tdate {1}\n\t\t\t\t\"{2}\"\n\t\t\t\t\n\t\t\t\t\t{3}\n\t\t\t\t\t\tmoney {4}\n\t\t\t\t\t{5}\n\t\t\t\t\t\tmoney {6}"
class BookkeepingEntry(QWidget):

    def __init__(self, row, header_dict):
        QWidget.__init__(self)

        self._accounts = []
        self._date = datetime.strptime(row[header_dict["date"]], '%d.%m.%Y')
        self._amount = int(float(row[header_dict["amount"]].replace(',','.'))*100)
        self._msg = row[header_dict["description_msg"]].replace("'","")
        self._type = row[header_dict["type_msg"]]
        self._from_to = row[header_dict["from_to"]]
        self._read_account_numbers()

    def startGUI(self):
        pass

    def _read_account_numbers(self):
        with open("tilit.csv", 'r') as f:
            for account in f.readlines():
                self._accounts.append(account)

    def form_bookkeeping_msg_tappio(self, event_n):
        while True:
            item, ok = QInputDialog.getItem(self, "Choose account number", "Please input internal account for bank entry with message: {0}, " \
                  "recipient/payer: {1} with amount {2}:\n".format(self._msg, self._from_to, str(int(self._amount)/100)), self._accounts, 0, False)
            #internal_account = input("Please input internal account for bank entry with message: {0}, " \
            #      "recipient/payer: {1} with amount {2}:\n".format(self._msg, self._from_to, str(int(self._amount)/100)))
            print('\n')
            if ok:
                internal_account = item.split(" ")[0]
                return BOOKKEEPING_BOOK_ENTRY_FORMAT.format(event_n,
                                                            self._date.strftime('%Y %m %d'),
                                                            self._msg,
                                                            "10003",
                                                            self._amount,
                                                            internal_account,
                                                            self._amount * -1)


    def get_date(self):
        return self._date

    def __lt__(self, other_entry):
        return self._date < other_entry.get_date()

    def __repr__(self):
        return "No representation available"

    def __str__(self):
        return "{0} {1} {2} {3}".format(self._date, self._from, self._to, self._msg)
