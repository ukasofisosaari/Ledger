import sys
import csv

from PySide2.QtWidgets import QApplication, QFileDialog, QWidget

from parse_tiliote import ENTRY_FIELDS

ACCOUNTS=[]
TEST_TILIOTE_FILE="ex_tiliote.csv"
def main():
    app = QApplication([])
    main_w = QWidget()
    main_w.show()

    tiliote_file = QFileDialog.getOpenFileName(main_w, "Valitse tiliote",
                                               "",
                                               "Text files (*.csv)")[0]
    bill_list = QFileDialog.getOpenFileName(main_w, "Valitse laskulista",
                                            "",
                                            "Text files (*.csv)")[0]
    billlist_reader = csv.reader(open(bill_list, newline=''), delimiter=";")
    next(billlist_reader, None)
    # Key is viite, values it tuple(value, bill_n)
    bills = {}
    for row in billlist_reader:
        bills[row[4]] = (float(row[7]), row[0] )
    tilioteReader = csv.reader(open(tiliote_file, newline=''), delimiter=";")
    headers = next(tilioteReader, None)
    list_of_entries = []
    for row in tilioteReader:
        desc = row[ENTRY_FIELDS["description_msg"]].replace("'", "").lstrip('0')
        if desc in bills and float(row[ENTRY_FIELDS["amount"]].replace(",", ".")) == bills[desc][0]:
            print("Lasku n:{0} maksettu, summa {1}".format(bills[desc][1], bills[desc][0]))
        elif desc in bills and float(row[ENTRY_FIELDS["amount"]].replace(",", ".")) != bills[desc][0]:
            print("Maksu ei täsmännyt: {0}, summa: {1}\n".format(bills[desc][1], bills[desc][0]))
            print(float(row[ENTRY_FIELDS["amount"]].replace(",", ".")))



    sys.exit(app.exec_())


if __name__ == "__main__":
    main()