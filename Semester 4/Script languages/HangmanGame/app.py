import random
import sys

from PyQt5.QtWidgets import *
from PyQt5.QtCore import Qt
from PyQt5.QtGui import QIcon, QFont, QPixmap


class Menu(QWidget):
    def __init__(self):
        super().__init__()

        self.uiScale = 2

        self.gameWindow = None
        self.dodajHasloWindow = None

        self.hasla = []
        self.kategorie = []
        self.plikKategorie = "data\\kategorie.csv"
        self.plikHasla = "data\\hasla.csv"
        self.zaladujKategorie()
        self.zaladujHasla()

        self.poprzednieHaslo = None

        self.setWindowTitle("Menu")
        self.setFixedSize(200 * self.uiScale, 200 * self.uiScale)

        self.setWindowFlag(Qt.WindowMinimizeButtonHint, False)
        self.setWindowFlag(Qt.WindowMaximizeButtonHint, False)

        self.setWindowIcon(QIcon("images\\icon.png"))

        buttonGraj = QPushButton("GRAJ", self)
        buttonDodajHaslo = QPushButton("DODAJ HASŁO", self)
        buttonWyjdz = QPushButton("WYJDŹ", self)

        buttonGraj.clicked.connect(self.buttonGrajAction)
        buttonDodajHaslo.clicked.connect(self.buttonDodajHasloAction)
        buttonWyjdz.clicked.connect(QApplication.quit)
        
        
        buttonGraj.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonDodajHaslo.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonWyjdz.setFixedSize(150 * self.uiScale, 50 * self.uiScale)

        buttonGraj.setStyleSheet(f"background-color: #2BC459; color: white;font-weight: bold;border-radius: 8px; font-size: {11 * self.uiScale}px;")
        buttonDodajHaslo.setStyleSheet(f"background-color: #008CBA; color: white;font-weight: bold;border-radius: 8px; font-size: {11 * self.uiScale}px;")
        buttonWyjdz.setStyleSheet(f"background-color: #C42B5E; color: white;font-weight: bold;border-radius: 8px; font-size: {11 * self.uiScale}px;")

        layout = QVBoxLayout()

        layout.addWidget(buttonGraj)
        layout.addWidget(buttonDodajHaslo)
        layout.addWidget(buttonWyjdz)

        layout.setAlignment(Qt.AlignCenter)
        self.setLayout(layout)
    

    def buttonGrajAction(self):
        czyBlad = self.rozpocznijGre()
        if not czyBlad:
            self.hide()
    

    def buttonDodajHasloAction(self):
        self.dodajHasloWindow = DodajHaslo(self)
        self.dodajHasloWindow.show()
        self.hide()


    def zaladujKategorie(self):
        try:
            with open(self.plikKategorie, "r", encoding='utf-8') as file:
                for line in file.readlines():
                    try:
                        kategoria = line.strip().upper()
                        if kategoria not in self.kategorie:
                            self.kategorie.append(kategoria)
                    except:
                        pass
        except FileNotFoundError:
            with open(self.plikKategorie, "w", encoding='utf-8') as file:
                pass
    

    def zaladujHasla(self):
        try:
            with open(self.plikHasla, "r", encoding='utf-8') as file:
                for line in file.readlines():
                    try:
                        hasloKat = line.strip().split(";")
                        hasloKatTuple = (hasloKat[0].upper(), hasloKat[1].upper())
                        if not (hasloKatTuple in self.hasla):
                            self.hasla.append(hasloKatTuple)
                        if not (hasloKatTuple[1] in self.kategorie):
                            self.kategorie.append(hasloKatTuple[1])
                    except:
                        pass
        except FileNotFoundError:
            with open(self.plikHasla, "w", encoding='utf-8') as file:
                pass


    def dodajHaslo(self, haslo, kategoria):
        haslo = haslo.strip()
        if len(haslo) == 0 or ";" in haslo or len(haslo) > 22:
            self.dialog = self.Potwierdzenie(czyBlad=True, tresc="Nieprawidłowe hasło.")
        elif (haslo,kategoria) in self.hasla:
            self.dialog = self.Potwierdzenie(czyBlad=True, tresc="Hasło już istnieje.")
        else:
            self.hasla.append((haslo,kategoria))
            with open(self.plikHasla, "a", encoding='utf-8') as file:
                file.write(haslo + ";" + kategoria + "\n")
            self.dialog = self.Potwierdzenie(czyBlad=False, tresc="Dodano hasło.")

        self.dialog.show()


    def dodajKategorie(self, kategoria):
        kategoria = kategoria.strip()
        if len(kategoria) == 0 or ";" in kategoria or len(kategoria) > 22:
            self.dialog = self.Potwierdzenie(czyBlad=True, tresc="Nieprawidłowa kategoria.")
        elif kategoria in self.kategorie:
            self.dialog = self.Potwierdzenie(czyBlad=True, tresc="Kategoria już istnieje.")
        else:
            self.kategorie.append(kategoria)
            with open(self.plikKategorie, "a", encoding='utf-8') as file:
                file.write(kategoria + "\n")
            self.dialog = self.Potwierdzenie(czyBlad=False, tresc="Dodano kategorię.")

        self.dialog.show()


    def losujHaslo(self):
        if len(self.hasla) == 0:
            return None
        
        return random.choice(self.hasla)


    def rozpocznijGre(self):
        losowanie = self.losujHaslo()

        if losowanie == None:
            self.dialog = self.Potwierdzenie(czyBlad=True, tresc="Brak haseł do wylosowania.")
            self.dialog.show()
            return True

        while losowanie == self.poprzednieHaslo:
            losowanie = self.losujHaslo()
        
        self.poprzednieHaslo = losowanie
        
        haslo, kategoria = losowanie[0], losowanie[1]

        self.gameWindow = Gra(kategoria, haslo, self)
        self.gameWindow.show()
        return False
    

    def getKategorie(self):
        return self.kategorie


    def getHasla(self):
        return self.hasla


    class Potwierdzenie(QWidget):
        def __init__(self, czyBlad, tresc):
            super().__init__()

            self.uiScale = 2

            self.setWindowTitle(" ")
            self.setFixedSize(110 * self.uiScale, 60 * self.uiScale)

            if czyBlad:
                self.setFixedSize(160 * self.uiScale, 60 * self.uiScale)

            self.setWindowIcon(QIcon("images\\icon.png"))
            self.setWindowFlag(Qt.WindowMinimizeButtonHint, False)

            self.setWindowFlag(Qt.WindowMaximizeButtonHint, False)

            layout = QVBoxLayout()

            font = QFont()
            font.setPointSize(11)

            label = QLabel(tresc, self)
            label.setFont(font)
            layout.addWidget(label, alignment=Qt.AlignCenter)

            buttonOK = QPushButton('OK', self)
            buttonOK.clicked.connect(self.close)
            layout.addWidget(buttonOK, alignment=Qt.AlignCenter)

            self.setLayout(layout)
             


class DodajHaslo(QWidget):
    def __init__(self, menuWindow):
        super().__init__()

        self.uiScale = 2

        self.menuWindow = menuWindow

        self.setWindowTitle("Dodaj hasło")
        self.setFixedSize(300 * self.uiScale, 200 * self.uiScale)

        self.setWindowIcon(QIcon("images\\icon.png"))

        self.setWindowFlag(Qt.WindowMinimizeButtonHint, False)
        self.setWindowFlag(Qt.WindowMaximizeButtonHint, False)

        outerLayout = QVBoxLayout()

        font = QFont()
        font.setPointSize(10)

        topLayout = QFormLayout()
        self.hasloEdit = QLineEdit()
        self.hasloEdit.setFixedHeight(30)
        hasloLabel = QLabel("Wpisz hasło:")
        hasloLabel.setFixedHeight(30)
        hasloLabel.setFont(font)
        topLayout.addRow(hasloLabel, self.hasloEdit)

        self.listaKategorii = QComboBox()
        self.listaKategorii.addItems(self.menuWindow.getKategorie())
        self.listaKategorii.setFixedHeight(30)
        self.listaKategorii.setFont(font)

        listaKategoriiLabel = QLabel("Wybierz kategorię:")
        listaKategoriiLabel.setFixedHeight(30)
        listaKategoriiLabel.setFont(font)

        topLayout.addRow(listaKategoriiLabel, self.listaKategorii)

        outerLayout.addLayout(topLayout)

        outerLayout.addSpacing(10)

        buttonDodajHaslo = QPushButton("DODAJ HASŁO", self)
        buttonDodajHaslo.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonDodajHaslo.setStyleSheet(f"background-color: #2BC459; color: white;font-weight: bold;border-radius: 8px; font-size: {11 * self.uiScale}px;")

        buttonDodajHaslo.clicked.connect(self.buttonDodajHasloAction)

        outerLayout.addWidget(buttonDodajHaslo, alignment=Qt.AlignCenter)

        outerLayout.addSpacing(30)

        bottomLayout = QFormLayout()
        self.kategoriaEdit = QLineEdit()
        self.kategoriaEdit.setFixedHeight(30)
        kategoriaLabel = QLabel("Wpisz kategorię:")
        kategoriaLabel.setFixedHeight(30)
        kategoriaLabel.setFont(font)
        bottomLayout.addRow(kategoriaLabel, self.kategoriaEdit)

        outerLayout.addLayout(bottomLayout)

        outerLayout.addSpacing(10)

        buttonDodajKategorie = QPushButton("DODAJ KATEGORIĘ", self)
        buttonDodajKategorie.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonDodajKategorie.setStyleSheet(f"background-color: #20C6D4; color: white;font-weight: bold;border-radius: 8px; font-size: {11 * self.uiScale}px;")

        buttonDodajKategorie.clicked.connect(self.buttonDodajKategorieAction)

        outerLayout.addWidget(buttonDodajKategorie, alignment=Qt.AlignCenter)

        outerLayout.addSpacing(10)

        self.setLayout(outerLayout)


    def buttonDodajHasloAction(self):
        haslo = self.hasloEdit.text().upper()
        kategoria = self.listaKategorii.currentText().upper()
        self.hasloEdit.clear()
        self.menuWindow.dodajHaslo(haslo, kategoria)
    

    def buttonDodajKategorieAction(self):
        kategoria = self.kategoriaEdit.text().upper()
        self.menuWindow.dodajKategorie(kategoria)
        self.kategoriaEdit.clear()
        self.listaKategorii.clear()
        self.listaKategorii.addItems(self.menuWindow.getKategorie())
        
    
    def closeEvent(self, event):
        self.menuWindow.show()
        event.accept()



class Gra(QWidget):
    def __init__(self, kategoria, haslo, menuWindow):
        super().__init__()

        self.menuWindow = menuWindow
        self.endGameWindow = None

        self.uiScale = 1

        self.kategoria = kategoria

        hasloSpacje = ""
        for znak in haslo:
            hasloSpacje += znak + " "
        
        self.haslo = hasloSpacje.rstrip()

        self.hasloZakryte = ""
        for i in range(len(self.haslo)):
            if not self.haslo[i].isalnum():
                self.hasloZakryte += self.haslo[i]
            else:
                self.hasloZakryte += "_"

        self.zycia = 8

        self.setWindowTitle("Wisielec")
        self.setFixedSize(800 * self.uiScale, 800 * self.uiScale)

        self.setWindowIcon(QIcon("images\\icon.png"))

        self.setWindowFlag(Qt.WindowMinimizeButtonHint, False)
        self.setWindowFlag(Qt.WindowMaximizeButtonHint, False)

        outerLayout = QVBoxLayout()

        outerLayout.addSpacing(10)

        font = QFont()
        font.setPointSize(20)

        biggerFont = QFont()
        biggerFont.setPointSize(30)
        biggerFont.setBold(True)
        

        kategoriaLabel1 = QLabel("Kategoria:")
        kategoriaLabel1.setFont(font)
        outerLayout.addWidget(kategoriaLabel1, alignment=Qt.AlignCenter)

        kategoriaLabel2 = QLabel(self.kategoria)
        kategoriaLabel2.setFont(biggerFont)
        outerLayout.addWidget(kategoriaLabel2, alignment=Qt.AlignCenter)

        outerLayout.addSpacing(40)

        self.hangmanImage = QLabel()
        image = QPixmap("images\\hangman8.png")
        self.hangmanImage.setPixmap(image)
        self.hangmanImage.setScaledContents(True)

        outerLayout.addWidget(self.hangmanImage, alignment=Qt.AlignCenter)

        outerLayout.addSpacing(20)

        hasloLabel1 = QLabel("Hasło:")
        hasloLabel1.setFont(font)
        outerLayout.addWidget(hasloLabel1, alignment=Qt.AlignCenter)

        self.hasloZakryteLabel = QLabel(self.hasloZakryte)
        self.hasloZakryteLabel.setFont(biggerFont)
        outerLayout.addWidget(self.hasloZakryteLabel, alignment=Qt.AlignCenter)

        outerLayout.addSpacing(50)

        bottomLayout = QGridLayout()

        alphabet = "AĄBCĆDEĘFGHIJKLŁMNŃOÓPQRSŚTUVWXYZŹŻ"

        self.buttons = []
        
        for i in range(len(alphabet)):
            letter = alphabet[i]
            button = QPushButton(letter)
            row = i // 7
            col = i % 7
            
            button.setStyleSheet("background-color: #F0E3CE; color: black;font-weight: bold;border-radius: 6px; font-size: 16px;")
            button.clicked.connect(lambda checked, b=button: self.zgadnijLitere(b))
            bottomLayout.addWidget(button, row, col)

            self.buttons.append(button)
        
        outerLayout.addLayout(bottomLayout)

        self.setLayout(outerLayout)


    def zgadnijLitere(self, button):
        litera = button.text()
        if litera in self.haslo:
            self.hasloZakryte = self.odkryjLitery(self.haslo, self.hasloZakryte, litera)
            button.setStyleSheet("background-color: #2DF767; color: black;font-weight: bold;border-radius: 6px; font-size: 16px;")
        else:
            self.zycia -= 1
            self.updateHangmanImage()
            button.setStyleSheet("background-color: #EE5278; color: black;font-weight: bold;border-radius: 6px; font-size: 16px;")
        
        button.setEnabled(False)
        self.hasloZakryteLabel.setText(self.hasloZakryte)
        
        if self.zycia == 0:
            for button in self.buttons:
                button.setEnabled(False)
            self.hasloZakryteLabel.setText(self.haslo)
            self.endGameWindow = KoniecGry(self, False)
            self.endGameWindow.show()
        elif not '_' in self.hasloZakryte:
            for button in self.buttons:
                button.setEnabled(False)
            self.endGameWindow = KoniecGry( self, True)
            self.endGameWindow.show()
    

    def updateHangmanImage(self):
        image_path = f"images\\hangman{self.zycia}.png"
        image = QPixmap(image_path)
        self.hangmanImage.setPixmap(image)
    

    def odkryjLitery(self, hasloOdkryte, hasloZakryte, litera):
        noweHasloZakryte = ""
        for i in range(len(hasloOdkryte)):
            if hasloOdkryte[i] == litera:
                noweHasloZakryte += litera
            else:
                noweHasloZakryte += hasloZakryte[i]
        return noweHasloZakryte
    

    def getMenuWindow(self):
        return self.menuWindow
    

    def closeEvent(self, event):
        if self.endGameWindow:
            self.endGameWindow.close()
        self.menuWindow.show()
        event.accept()



class KoniecGry(QWidget):
    def __init__(self, gameWindow, czyWygrana):
        super().__init__()

        self.uiScale = 1
        
        self.gameWindow = gameWindow
        self.czyWygrana = czyWygrana

        self.setWindowTitle("Koniec gry")
        self.setFixedSize(400 * self.uiScale, 300 * self.uiScale)

        self.setWindowIcon(QIcon("images\\icon.png"))

        self.setWindowFlag(Qt.WindowMinimizeButtonHint, False)
        self.setWindowFlag(Qt.WindowMaximizeButtonHint, False)
        self.setWindowFlag(Qt.WindowCloseButtonHint, False)

        if self.czyWygrana:
            image = QPixmap("images\\wygrana.png")
            tresc = "Gratulacje!"
        else:
            image = QPixmap("images\\przegrana.png")
            tresc = "Następnym razem się uda!"

        outerLayout = QVBoxLayout()

        font = QFont()
        font.setPointSize(15)

        endGameLabel = QLabel(tresc)
        endGameLabel.setFont(font)

        outerLayout.addWidget(endGameLabel, alignment=Qt.AlignCenter)

        endGameImage = QLabel()
        endGameImage.setPixmap(image)
        endGameImage.setScaledContents(True)

        outerLayout.addWidget(endGameImage, alignment=Qt.AlignCenter)

        bottomLayout = QHBoxLayout()

        buttonZagrajPonownie = QPushButton("ZAGRAJ PONOWNIE", self)
        buttonZagrajPonownie.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonZagrajPonownie.setStyleSheet(f"background-color: #86D149; color: white;font-weight: bold;border-radius: 8px; font-size: {13 * self.uiScale}px;")

        buttonZagrajPonownie.clicked.connect(self.buttonZagrajPonownieAction)

        bottomLayout.addWidget(buttonZagrajPonownie, alignment=Qt.AlignCenter)

        buttonMenu = QPushButton("MENU", self)
        buttonMenu.setFixedSize(150 * self.uiScale, 50 * self.uiScale)
        buttonMenu.setStyleSheet(f"background-color: #C42B5E; color: white;font-weight: bold;border-radius: 8px; font-size: {14 * self.uiScale}px;")

        buttonMenu.clicked.connect(self.buttonMenuAction)

        bottomLayout.addWidget(buttonMenu, alignment=Qt.AlignCenter)

        outerLayout.addLayout(bottomLayout)

        self.setLayout(outerLayout)
    

    def buttonZagrajPonownieAction(self):
        self.gameWindow.close()
        self.gameWindow.getMenuWindow().buttonGrajAction()
        self.close()
    

    def buttonMenuAction(self):
        self.gameWindow.close()
        self.close()        
    


def run():
    app = QApplication(sys.argv)
    menuWindow = Menu()
    menuWindow.show()
    sys.exit(app.exec_())


if __name__ == "__main__":
    run()
