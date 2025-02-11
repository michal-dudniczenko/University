//implementacja klasy CMyString

#include "CMyString.h"

#include <stdexcept>


//konstruktor bezparametrowy, podstawowo pusty string, jedynie z null terminatorem
CMyString::CMyString() {
	_length = 0;
	text = new char[1];
	text[0] = '\0';
}

//konstruktor kopiujacy, kopiuje zawartosc tablicy podanego obiektu, na koncu null terminator
CMyString::CMyString(const CMyString& other) {
	_length = other._length;
	text = new char[_length + 1];
	for (int i = 0; i < _length; i++) {
		text[i] = other.text[i];
	}
	text[_length] = '\0';
}

//kostruktor pomocniczy, alokuje pusta tablice o podanej dlugosci
CMyString::CMyString(int length) {
	if (length >= 0) {
		_length = length;
		text = new char[_length + 1];
		text[_length] = '\0';
	}
	else {
		throw new std::runtime_error("String length cant be negative!");
	}
}

//konstruktor parametryzowany przyjmujacy string
CMyString::CMyString(const char* otherText) {
	//obliczam dlugosc podanego stringa
	int tempLength = 0;
	while (otherText[tempLength] != '\0') {
		tempLength++;
	}
	_length = tempLength;
	//alokujemy tablice i kopiujemy do niej kolejne chary w podanym stringu
	text = new char[_length + 1];
	for (int i = 0; i < _length; i++) {
		text[i] = otherText[i];
	}
	//na koncu null terminator
	text[_length] = '\0';
}

//konstruktor przenoszacy, przejmuje tablice z podanego obiektu i ustawia jego wskaznik jako nullptr
CMyString::CMyString(CMyString&& other) noexcept
	: text(other.text), _length(other._length)
{
	other.text = nullptr;
}

//destruktor, dealokuje dynamiczna tablice
CMyString::~CMyString() {
	delete[] text;
}


void CMyString::operator=(const CMyString& other) {
	//jezeli nie przypisujemy tego samego obiektu
	if (this != &other) {
		//jezeli druga tablica ma taka sama dlugosc to nie trzeba dealokowac starej
		if (other._length != _length) {
			//dealokacja starej tablicy
			delete[] text;
			_length = other._length;
			text = new char[_length + 1];
		}
		//kopiowanie tablicy
		for (int i = 0; i < _length; i++) {
			text[i] = other.text[i];
		}
		text[_length] = '\0';
	}
}


void CMyString::operator=(const char* otherText) {
	//obliczenie dlugosci podanego stringa
	int tempLength = 0;
	while (otherText[tempLength] != '\0') {
		tempLength++;
	}
	//jezeli druga tablica ma taka sama dlugosc to nie trzeba dealokowac starej
	if (tempLength != _length) {
		delete[] text;
		_length = tempLength;
		text = new char[_length + 1];
	}
	//kopiowanie tablicy
	for (int i = 0; i < _length; i++) {
		text[i] = otherText[i];
	}
	text[_length] = '\0';
}


void CMyString::operator+=(const CMyString& other) {
	//alokacja nowej tablicy
	int newLength = _length + other._length;
	char* newText = new char[newLength + 1];
	int newTextIndex = 0;
	//kopiowanie starego stringa na poczatek nowej tablicy
	for (int i = 0; i < _length; i++) {
		newText[newTextIndex] = text[i];
		newTextIndex++;
	}
	//kopiowanie, doczepianie dodawanego stringa
	for (int i = 0; i < other._length; i++) {
		newText[newTextIndex] = other.text[i];
		newTextIndex++;
	}
	//dodanie null terminatora
	newText[newLength] = '\0';
	//zastapienie obecnej tablicy nowo utworzona
	delete[] text;
	_length = newLength;
	text = newText;
}


void CMyString::operator+=(const char* otherText) {
	//obliczenie dlugosci dodawanego stringa
	int tempLength = 0;
	while (otherText[tempLength] != '\0') {
		tempLength++;
	}
	//alokacja nowej tablicy
	int newLength = _length + tempLength;
	char* newText = new char[newLength + 1];
	int newTextIndex = 0;
	//kopiowanie starego stringa na poczatek nowej tablicy
	for (int i = 0; i < _length; i++) {
		newText[newTextIndex] = text[i];
		newTextIndex++;
	}
	//kopiowanie, doczepianie dodawanego stringa
	for (int i = 0; i < tempLength; i++) {
		newText[newTextIndex] = otherText[i];
		newTextIndex++;
	}
	//dodanie null terminatora
	newText[newLength] = '\0';
	//zastapienie obecnej tablicy nowo utworzona
	delete[] text;
	_length = newLength;
	text = newText;
}

CMyString CMyString::operator+(const CMyString& other) {
	//utworzenie nowego obiektu, ktory zostanie zwrocony
	CMyString result(_length + other._length);
	int newTextIndex = 0;
	//kopiowanie starego stringa na poczatek nowej tablicy
	for (int i = 0; i < _length; i++) {
		result.text[newTextIndex] = text[i];
		newTextIndex++;
	}
	//kopiowanie, doczepianie dodawanego stringa
	for (int i = 0; i < other._length; i++) {
		result.text[newTextIndex] = other.text[i];
		newTextIndex++;
	}
	return result;
}

CMyString CMyString::operator+(const char* otherText) {
	//obliczenie dlugosci dodawanego stringa
	int tempLength = 0;
	while (otherText[tempLength] != '\0') {
		tempLength++;
	}
	//utworzenie nowego obiektu, ktory zostanie zwrocony
	CMyString result(_length + tempLength);
	int newTextIndex = 0;
	//kopiowanie starego stringa na poczatek nowej tablicy
	for (int i = 0; i < _length; i++) {
		result.text[newTextIndex] = text[i];
		newTextIndex++;
	}
	//kopiowanie, doczepianie dodawanego stringa
	for (int i = 0; i < tempLength; i++) {
		result.text[newTextIndex] = otherText[i];
		newTextIndex++;
	}
	return result;
}

//rzutowanie do std::string
std::string CMyString::toString() {
	return std::string(text);
}

//rzutowanie do bool
CMyString::operator bool() const {
	return (_length != 0);
}

//zwrocenie dlugosci
int CMyString::length() {
	return _length;
}

//zwrocenie znaku na podanej pozycji
char CMyString::at(int index) {
	//jezeli podany index jest prawidlowy
	if (index < _length && index >= 0) {
		return text[index];
	}
	//jak nie to blad
	else {
		throw new std::runtime_error("Index out of bound for length: " + _length);
	}
}

//porownywanie z innym obiektem, zwraca true jezeli przechowywana tablica char'ów jest taka sama jak w podanym obiekcie
bool CMyString::operator==(const CMyString& other) {
	//jezeli rozne dlugosci to false
	if (_length != other._length) {
		return false;
	}
	//sprawdz po kolei wszystkie znaki 
	for (int i = 0; i < _length; i++) {
		//jezeli jakis znak jest rozny to false
		if (text[i] != other.text[i]) {
			return false;
		}
	}
	//wszystkie znaki byly takie same, zwroc true
	return true;
}