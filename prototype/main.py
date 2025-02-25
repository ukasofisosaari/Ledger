import sys

from PySide2.QtWidgets import QApplication, QFileDialog, QWidget

from parse_tiliote import parse_tiliote

ACCOUNTS=[]
TEST_TILIOTE_FILE="ex_tiliote.csv"
def main():
    app = QApplication([])
    main_w = QWidget()
    main_w.show()

    tiliote_file = QFileDialog.getOpenFileName(main_w, "Valitse tiliote",
                                               "",
                                               "Text files (*.csv)")[0]
    kirjanpito_file = QFileDialog.getOpenFileName(main_w, "Valitse kirjanpito",
                                                  "",
                                                  "Text files (*.csv)")[0]




    event_n = 5
    with open(kirjanpito_file, 'a') as f:
        for tiliote in parse_tiliote(tiliote_file, main_w):
            tiliote_entry = tiliote.form_bookkeeping_msg_tappio(event_n)
            f.write(tiliote_entry)
            print(tiliote_entry)
            event_n +=1

    sys.exit(app.exec_())


if __name__ == "__main__":
    main()