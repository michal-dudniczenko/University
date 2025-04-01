import os
from log_utility import *

from PyQt5.QtWidgets import *
from PyQt5 import uic
from PyQt5.QtGui import QIcon



class AppGUI(QMainWindow):
    def __init__(self):
        super(AppGUI, self).__init__()
        uic.loadUi("gui.ui", self)
        self.show()

        self.log_list = []
        self.selected_log = ""
        self.icon_path = "icon.png"

        self.open_button.clicked.connect(self.open_button_action)
        self.filter_button.clicked.connect(self.filter_button_action)
        self.listWidget.itemClicked.connect(self.list_item_click_action)
        self.next_button.clicked.connect(self.next_button_action)
        self.previous_button.clicked.connect(self.previous_button_action)
        self.setWindowIcon(QIcon(self.icon_path))
    
    
    def open_button_action(self):
        path = self.path_text_edit.toPlainText()

        try:
            new_log_list = []
            with open(path, encoding="utf-8") as log_file:
                for log in log_file:
                    if check_if_valid(log):
                        new_log_list.append(log)
            
            self.log_list = new_log_list 
            self.listWidget.clear()
            self.listWidget.addItems(self.log_list)
            self.clear_text_edits()
        except:
            self.log_list = []
            message = QMessageBox()
            message.setText("<center>Invalid path to log file!</center>")
            message.setWindowTitle("Error!")
            message.setWindowIcon(QIcon(self.icon_path))

            message.exec_()
    

    def filter_button_action(self):
        date_from = self.date_from.date().toString("dd.MM.yyyy")
        date_to = self.date_to.date().toString("dd.MM.yyyy")
        
        filtered_logs = [log for log in self.log_list if filter_date(log, date_from, date_to)]
        self.listWidget.clear()
        self.listWidget.addItems(filtered_logs)
        self.previous_button.setEnabled(False)
        self.next_button.setEnabled(False)
        self.clear_text_edits()
    

    def list_item_click_action(self):
        self.previous_button.setEnabled(not self.listWidget.currentRow() == 0)
        self.next_button.setEnabled(not self.listWidget.currentRow() == (self.listWidget.count()-1))
        self.selected_log = self.listWidget.currentItem().text().rstrip()
        self.fill_text_edits()
    

    def next_button_action(self):
        current_index = self.listWidget.currentRow()
        self.listWidget.setCurrentRow(current_index + 1)

        self.selected_log = self.listWidget.currentItem().text().rstrip()
        self.next_button.setEnabled(not (current_index + 1) == (self.listWidget.count()-1))
        self.previous_button.setEnabled(True)
        self.fill_text_edits()

    
    def previous_button_action(self):
        current_index = self.listWidget.currentRow()
        self.listWidget.setCurrentRow(current_index - 1)

        self.selected_log = self.listWidget.currentItem().text().rstrip()
        self.previous_button.setEnabled(not (current_index - 1) == 0)
        self.next_button.setEnabled(True)
        self.fill_text_edits()
    

    def fill_text_edits(self):
        log_dict = get_log_dict(self.selected_log)
        self.host_text_edit.setPlainText(log_dict["host"])
        self.date_text_edit.setPlainText(log_dict["date"])
        self.time_text_edit.setPlainText(log_dict["time"])
        self.timezone_text_edit.setPlainText(log_dict["timezone"])
        self.status_text_edit.setPlainText(log_dict["status"])
        self.method_text_edit.setPlainText(log_dict["method"])
        self.resource_text_edit.setPlainText(log_dict["resource"])
        self.size_text_edit.setPlainText(log_dict["size"])

        status_code = int(log_dict["status"])
        if status_code >= 400:
            self.status_text_edit.setStyleSheet("background-color: rgb(215, 63, 83);")
        elif status_code >= 300:
            self.status_text_edit.setStyleSheet("background-color: rgb(230, 211, 87);")
        else:
            self.status_text_edit.setStyleSheet("background-color: rgb(100, 225, 75);")
    

    def clear_text_edits(self):
        self.host_text_edit.clear()
        self.date_text_edit.clear()
        self.time_text_edit.clear()
        self.timezone_text_edit.clear()
        self.status_text_edit.clear()
        self.method_text_edit.clear()
        self.resource_text_edit.clear()
        self.size_text_edit.clear()

        self.status_text_edit.setStyleSheet("background-color: rgb(255, 255, 255);")
    


def main():
    app = QApplication([])
    windows = AppGUI()
    app.exec_()



if __name__ == "__main__":
    os.environ["QT_QPA_PLATFORM_PLUGIN_PATH"] = r"C:\Users\Michal\Documents\Studia\Semestr 4\Języki skryptowe - laboratorium\lista8\.venv\Lib\site-packages\PyQt5\Qt5\plugins\platforms"
    main()