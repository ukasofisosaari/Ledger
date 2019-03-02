import csv
from PySide2.QtWidgets import QApplication, QFileDialog, QWidget

from bookkeeping_entry import BookkeepingEntry
ENTRY_FIELDS = { "date": 0,
  "from_to": 1,
  "type_msg": 2,
  "description_msg": 3,
  "amount": 4
}



def parse_tiliote(file, main_w):
    bill_list = QFileDialog.getOpenFileName(main_w, "Valitse laskulista",
                                                  "",
                                                  "Text files (*.csv)")[0]

    billlist_reader = csv.reader(open(bill_list, newline=''), delimiter=";")
    next(billlist_reader, None)
    #Key is viitenumero, values it tuple(descr, bill_n)
    bills = {}
    for row in billlist_reader:
        print(row)
        print(row[4])
        print(row[0], row[3] + ' ' + row[9])
        bills[row[4]] = (row[0], row[3] + ' ' + row[9])
    tilioteReader = csv.reader(open(file, newline=''), delimiter=";")
    headers = next(tilioteReader, None)
    list_of_entries= []
    for row in tilioteReader:
        print("***********")
        print(row)
        desc = row[ENTRY_FIELDS["description_msg"]].replace("'","")
        if desc in bills:
            desc = bills[desc][1]

        row[ENTRY_FIELDS["description_msg"]] = desc
        entry = BookkeepingEntry(row, ENTRY_FIELDS)
        list_of_entries.append(entry)
    list_of_entries.sort()
    return list_of_entries