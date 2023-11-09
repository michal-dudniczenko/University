#include <iostream>
#include <string>
#include "CNumber.h"
#include <sstream>

//konstruktor bezparametrowy
CNumber::CNumber() {
	//domyslna dlugosc okreslona w pliku naglowkowym i pusta lista
	this->length = default_length;
	this->is_negative = false;
	this->digits_array = new int[this->length];
}

//konstruktor przyjmujacy liczbe
CNumber::CNumber(int number) {
	//zero to specyficzny przypadek, rozpatruje go oddzielnie
	if (number == 0) {
		this->length = 1;
		this->digits_array = new int[1];
		this->digits_array[0] = 0;
		this->is_negative = false;
		return;
	}
	//jezeli liczba jest ujemna to ustawiam flage ujemnosci na true i przechodze na dodatnia, jak nie to flaga na false
	if (number < 0) {
		this->is_negative = true;
		number *= (-1);
	}
	else {
		this->is_negative = false;
	}

	//obliczam liczbe cyfr (dlugosc liczby) 
	int temp = number;
	int length_count = 0;
	while (temp > 0) {
		length_count++;
		temp /= 10;
	}
	this->length = length_count;
	//tworze tablice o obliczonej dlugosci
	this->digits_array = new int[length_count];
	//umieszczam w tablicy od tylu poszczegolne cyfry liczby otrzymujac je dzielac modulo przez 10
	for (int i = length_count-1; i >= 0; i--) {
		digits_array[i] = number % 10;
		number /= 10;
	}
}

//konstruktor kopiujacy przyjmujacy inny cnumber
CNumber::CNumber(const CNumber& other) {
	//przypisuje wartosci pol drugiego obiektu length i is_negative bezposrednio kopiuje wartosci
	this->length = other.length;
	this->is_negative = other.is_negative;
	//tablice odwzorowuje tworzac nowa tablice i kopiujac poszczegolne wartosci z tablicy drugiego obiektu
	this->digits_array = new int[this->length];
	for (int i = 0; i < this->length; i++) {
		this->digits_array[i] = other.digits_array[i];
	}
}

//konstuktor przyjmujacy reprezentacje liczby w postaci tablicy oraz pozostale pola
CNumber::CNumber(int*& array, int length, bool is_negative) {
	//analogicznie jak wyzej w konstruktorze kopiujacym, length oraz is_negative przypisuje przez wartosc, tablice kopiuje poszczegolne elementy
	this->digits_array = new int[length];
	for (int i = 0; i < length; i++) {
		this->digits_array[i] = array[i];
	}
	this->length = length;
	this->is_negative = is_negative;
}

//destruktor 
CNumber::~CNumber() {
	//usuwam dynamicznie zaalokowana tablice zeby uniknac wycieku pamieci
	delete[] this->digits_array;
}

//operator przypisania wartosci w postaci inta
void CNumber::operator=(const int value) {
	//usuwam obecn¹ dynamicznie zaalokowana tablice zeby uniknac wycieku pamieci
	delete[] this->digits_array;

	//analogicznie jak w konstruktorze przyjmujacym wartosc int
	
	//zero to specyficzny przypadek, rozpatruje go oddzielnie
	if (value == 0) {
		this->length = 1;
		this->digits_array = new int[1];
		this->digits_array[0] = 0;
		this->is_negative = false;
		return;
	}

	//okreslam znak liczby i przechodze na dodatnia 
	int temp = value;
	if (temp < 0) {
		this->is_negative = true;
		temp *= (-1);
	}
	else {
		this->is_negative = false;
	}

	//obliczam dlugosc liczby, liczbe cyfr
	int length_count = 0;
	while (temp > 0) {
		length_count++;
		temp /= 10;
	}
	this->length = length_count;

	this->digits_array = new int[length_count];
	temp = value;
	if (temp < 0) {
		temp *= (-1);
	}
	//wypelniam tablice cyfr od tylu poszczegolnymi cyframi liczby uzyskanymi z wykorzystaniem dzielenia modulo przez 10
	for (int i = length_count - 1; i >= 0; i--) {
		digits_array[i] = temp % 10;
		temp /= 10;
	}
}

//operator przypisania wartosci w postaci innego obiektu cnumber
void CNumber::operator=(const CNumber& other) {
	//jezeli to ten sam obiekt to nie trzeba nic robic
	if (this == &other) {
		return;
	}
	//usuwam dynamicznie zaalokowana tablice zeby uniknac wycieku pamieci
	delete[] this->digits_array;

	//analogicznie do konstruktora kopiujacego, length i is_negative kopiuje bezposrednio, tablice po kolei przepisuje poszczegolne wartosci
	this->length = other.length;
	this->digits_array = new int[this->length];
	for (int i = 0; i < this->length; i++) {
		this->digits_array[i] = other.digits_array[i];
	}
	this->is_negative = other.is_negative;
}

//operator dodawania zwraca nowy cnumber rowny obecnemu obiektowi powieksoznemu o wartosc cnumber w parametrze
CNumber CNumber::operator+(const CNumber& other) const {
	
	//inicjuje pola obiektu wynikowego
	int result_length;
	int* result_array;
	bool result_is_negative;

	//kopiuje tablice liczb w obu obiektach, bede ich uzywal w wywolaniach funkcji pomocnicznych, chce uniknac operowania na oryginalnych danych
	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}
	
	//rozpatruje wszystkie mozliwe przypadki zwiazane z roznymi znakami liczb w operacji dodawania, operuje tylko na modulach liczb

	if (this->is_negative) {
		//ujemna + ujemna
		//dodaje modduly liczb i okreslam wynik jako ujemny
		if (other.is_negative) {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result_is_negative = true;
		}
		//ujemna + dodatnia czyli other - this
		//odejmuje pierwsza liczbe od drugiej
		else {
			//jezeli odejmowana liczba jest wieksza to zamiast tego odejmuje wieksza - mniejsza(zeby otrzymac dodatni wynik)
			//  i ustawiam wynik jako ujemny
			if (absolute_is_1_less_than_2(other, *this)) {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result_is_negative = true;
			}
			//jezeli odejmowana liczba jest mniejsza lub rowna to po prostu wykonuje odejmownaie
			//  i ustawiam wynik jako nieujemny
			else {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result_is_negative = false;
			}
		}
	}
	else {
		//dodatnia + ujemna czyli this - other
		if (other.is_negative) {
			//analogicznie jak wyzej, odejmuje tak zeby uniknac ujemnego wyniku i okreslam znak wyniku
			if (absolute_is_1_less_than_2(*this, other)) {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result_is_negative = true;
			}
			else {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result_is_negative = false;
			}
		}
		//dodatnia + dodatnia
		//wykonuje dodawanie i ustawiam znak na nieujemny
		else {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result_is_negative = false;
		}
	}
	
	//usuwam dynamicznie zaalokowane tymczasowe tablice zwalniajac pamiec
	delete[] array_1;
	delete[] array_2;

	//tworze obiekt wynikowy, na podstawie wczesniej otrzymanych tablicy, dlugosci i znaku
	CNumber result(result_array, result_length, result_is_negative);


	//zwracam kompletny obiekt bedacy wynikiem
	return result;
}

//operator dodawania zwraca nowy cnumber rowny obecnemu obiektowi powiekszonemu o wartosc w parametrze
CNumber CNumber::operator+(const int value) const{
	//operator pomocniczy wykorzystywany w implementacji dzielenia

	//tworze tymczasowy obiekt reprezentujacy wartosc do dodania
	CNumber temp = CNumber(value);
	//wykorzystuje wczesniejsza implementacje dodawania dwoch obiektow cnumber
	return *this + temp;
}

//operator odejmowania zwraca nowy cnumber rowny obecnemu obiektowi pomniejszonemu o wartosc cnumber w parametrze
CNumber CNumber::operator-(const CNumber& other) const {
	//logika analogiczna do operatora dodawania
	
	//inicjuje pola obiektu wynikowego
	int result_length;
	int* result_array;
	bool result_is_negative;

	//kopiuje tablice liczb w obu obiektach, bede ich uzywal w wywolaniach funkcji pomocnicznych, chce uniknac operowania na oryginalnych danych
	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}


	//rozpatruje wszystkie mozliwe przypadki zwiazane z roznymi znakami liczb w operacji dodawania, operuje tylko na modulach liczb

	if (this->is_negative) {
		//ujemna1 - ujemna2   czyli other - this
		//w tym przypadku od drugiej liczby odejmuje pierwsza
		if (other.is_negative) {
			//jezeli odejmowana liczba jest wieksza to zamiast tego odejmuje wieksza - mniejsza(zeby otrzymac dodatni wynik)
			//i ustawiam wynik jako ujemny
			if (absolute_is_1_less_than_2(other, *this)) {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result_is_negative = true;
			}
			//jezeli odejmowana liczba jest mniejsza lub rowna to po prostu wykonuje odejmownaie
			//i ustawiam wynik jako nieujemny
			else {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result_is_negative = false;
			}
		}
		//ujemna - dodatnia
		//dodaje modu³y i oznaczam wynik jako ujemmy
		else {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result_is_negative = true;
		}
	}
	else {
		//dodatnia - ujemna czyli this+other
		//dodaje modu³y i ustawiam wynik jako nieujemny
		if (other.is_negative) {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result_is_negative = false;
		}
		//dodatnia - dodatnia czyli this-other
		else {
			//analogicznie jak wyzej, odejmuje tak zeby uniknac ujemnego wyniku i okreslam znak wyniku
			if (absolute_is_1_less_than_2(*this, other)) {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result_is_negative = true;
			}
			else {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result_is_negative = false;
			}
		}
	}

	//usuwam dynamicznie zaalokowane tymczasowe tablice zwalniajac pamiec
	delete[] array_1;
	delete[] array_2;

	//tworze obiekt wynikowy, na podstawie wczesniej otrzymanych tablicy, dlugosci i znaku
	CNumber result(result_array, result_length, result_is_negative);

	//zwracam kompletny obiekt bedacy wynikiem
	return result;
}

//operator mnozenia zwraca nowy cnumber rowny obecnemu obiektowi pomnozonemu razy wartosc cnumber w parametrze
CNumber CNumber::operator*(const CNumber& other) const {
	//inicjuje pola obiektu wynikowego
	int result_length;
	int* result_array;
	bool result_is_negative;

	//kopiuje tablice liczb w obu obiektach, bede ich uzywal w wywolaniach funkcji pomocnicznych, chce uniknac operowania na oryginalnych danych
	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}

	//korzystajac z funkcji pomocniczej multiply wykonuje mnozenie uzyskujac reprezentacje wyniku w postaci tablicy cyfr
	result_array = multiply(array_1, array_2, this->length, other.length, result_length);

	//nastepnie okreslam i ustawiam znak wyniku
	if (this->is_negative && other.is_negative) {
		result_is_negative = false;
	}
	else if (this->is_negative || other.is_negative) {
		result_is_negative = true;
	}
	else {
		result_is_negative = false;
	}

	//usuwam dynamicznie zaalokowane tymczasowe tablice zwalniajac pamiec
	delete[] array_1;
	delete[] array_2;

	//tworze obiekt wynikowy, na podstawie wczesniej otrzymanych tablicy, dlugosci i znaku
	CNumber result(result_array, result_length, result_is_negative);

	//zwracam kompletny obiekt bedacy wynikiem
	return result;
}

//operator dzielenia zwraca nowy cnumber rowny obecnemu obiektowi podzielonemu przez wartosc cnumber w parametrze
CNumber CNumber::operator/(const CNumber& other) const {
	
	//w przypadku dzielenia przez zero wypisuje informacje o bledzie i zwracam 0
	if (other.digits_array[0] == 0) {
		std::cout<< "ERROR: Division by zero!\n";
		return CNumber(0);
	}
	//jezeli dzielna to zero lub dzielnik>dzielnej to zwracam wynik zero
	if (this->digits_array[0] == 0 || absolute_is_1_less_than_2(*this, other)) {
		return CNumber(0);
	}

	//inicjuje pola obiektu wynikowego
	int result_length;
	int* result_array;
	bool result_is_negative;

	//kopiuje tablice liczb w obu obiektach, bede ich uzywal w wywolaniach funkcji pomocnicznych, chce uniknac operowania na oryginalnych danych
	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}

	//korzystajac z funkcji pomocniczej divide wykonuje dzielenie uzyskujac reprezentacje wyniku w postaci tablicy cyfr
	result_array = divide(array_1, array_2, this->length, other.length, result_length);

	//okreslam i ustawiam znak wyniku
	if (this->is_negative && other.is_negative) {
		result_is_negative = false;
	}
	else if (this->is_negative || other.is_negative) {
		result_is_negative = true;
	}
	else {
		result_is_negative = false;
	}

	//usuwam dynamicznie zaalokowane tymczasowe tablice zwalniajac pamiec
	delete[] array_1;
	delete[] array_2;

	//tworze obiekt wynikowy, na podstawie wczesniej otrzymanych tablicy, dlugosci i znaku
	CNumber result(result_array, result_length, result_is_negative);

	//zwracam kompletny obiekt bedacy wynikiem
	return result;
}

//operator porownania - wiekoszci, true jezeli wartosc this > wartosc other, false wpp
bool CNumber::operator>(const CNumber& other) const {
	//jezeli to to samo
	if (this == &other) return false;
	
	//jezeli sa roznych znakow
	if (this->is_negative != other.is_negative) {
		if (this->is_negative) return false;
		else return true;
	}
	
	//porownywanie ujemnych
	if (this->is_negative) {
		//rozne dlugosci
		if (this->length < other.length) return true;
		if (this->length > other.length) return false;

		//porownanie jezeli maja takie same dlugosci, porownywanie kolejnych par cyfr
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return false;
			else if (this->digits_array[i] < other.digits_array[i]) return true;
		}
		//okazaly sie rowne
		return false;
	}
	//porownywanie dodatnich
	else {
		//rozne dlugosci
		if (this->length < other.length) return false;
		if (this->length > other.length) return true;

		//porownanie jezeli maja takie same dlugosci, porownywanie kolejnych par cyfr
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return true;
			else if (this->digits_array[i] < other.digits_array[i]) return false;
		}
		//okazaly sie rowne
		return false;
	}
}

//operator porownania - mniejszosci, true jezeli wartosc this < wartosc other, false wpp
bool CNumber::operator<(const CNumber& other) const {
	//jezeli to to samo
	if (this == &other) return false;

	//jezeli sa roznych znakow
	if (this->is_negative != other.is_negative) {
		if (this->is_negative) return true;
		else return false;
	}

	//porownywanie ujemnych
	if (this->is_negative) {
		//rozne dlugosci
		if (this->length < other.length) return false;
		if (this->length > other.length) return true;

		//porownanie jezeli maja takie same dlugosci, porownywanie kolejnych par cyfr
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return true;
			else if (this->digits_array[i] < other.digits_array[i]) return false;
		}
		//okazaly sie rowne
		return false;
	}
	//porownywanie dodatnich
	else {
		//rozne dlugosci
		if (this->length < other.length) return true;
		if (this->length > other.length) return false;

		//porownanie jezeli maja takie same dlugosci, porownywanie kolejnych par cyfr
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return false;
			else if (this->digits_array[i] < other.digits_array[i]) return true;
		}
		//okazaly sie rowne
		return false;
	}
}

//operator porownania - rownosci, true jezeli wartosc this jest rowna wartosci other, false wpp
bool CNumber::operator==(const CNumber& other) const {
	//jezeli to ten sam obiekt
	if (this == &other) return true;

	//jezeli roznia sie dlugoscia/znakiem to nie sa rowne
	if (this->length != other.length || this->is_negative != other.is_negative) return false;

	//porownuje kolejne pary cyfr, jezeli jakas nie jest rowna to liczby nie sa rowne
	for (int i = 0; i < this->length; i++) {
		if (this->digits_array[i] != other.digits_array[i]) return false;
	}
	//jezeli nie znaleziono zadnej niezgodnej pary to liczby sa rowne
	return true;
}

//funkcja zwraca reprezentacje obiektu cnumber w postaci stringa
std::string CNumber::to_string() const {
	//tworze string wynikowy
	std::stringstream ss;
	ss << "number: ";
	//jezeli liczba jest ujemna to dodaje minus
	if (this->is_negative) ss << "-";
	//iterujac po tablicy dodaje do napisu liczbe ktor¹ przechowuje obiekt
	for (int i = 0; i < this->length; i++) {
		ss << this->digits_array[i];
	}
	//dodaje informacje o dlugsci liczby
	ss << " length: " << this->length;
	//zwracam string wynikowy
	return ss.str();
}

//funkcja pomocnicza implementujaca dodawanie pisemne dwoch liczb dodatnich
int* CNumber::sum(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//funkcja pomocnicza realizujaca dodawanie dwoch liczb nieujemnych
	//jest to implementacja odwzorowujaca algorytm dodawania pisemnego "w slupku"
	//do funkcji przekazuje referencje na zmienna przechowujaca dlugosc wyniku w obiekcie wynikowym z operatora
	//dzieki czemu moge modyfikowac ja wewnatrz tej funkcji pomocniczej
	
	//dlugosc wyniku moze byc maksymalnie o jeden wieksza niz dlugosc dluzszej liczby
	result_length = std::max(length_1, length_2) + 1;

	//bede przechodzil po liczbach od tylu, tak jak w dodawaniu pisemnym
	int array_1_index = length_1 - 1;
	int array_2_index = length_2 - 1;
	int result_array_index = result_length - 1;

	//tworze pusta tablice na wynik
	int* result_array = new int[result_length];
	//inicuje pierwszy element zeby w sytuacji braku przeniesienia z dodawania nie bylo tam "smieciowej" wartosci
	result_array[0] = 0;

	//przechodze od tylu po dodawanych liczbach dodajac je i sume zapisujac do tablicy wynikowej do czasu az nie dojde do konca(tutaj poczatku)
	//ktorejs z nich
	while ((array_1_index >= 0) && (array_2_index >= 0)) {
		result_array[result_array_index] = array_1[array_1_index] + array_2[array_2_index];
		result_array_index--;
		array_1_index--;
		array_2_index--;
	}

	//jezeli w liczbie1 zostaly jakies cyfry (liczba2 byla krotsza) to przepisuje je do wyniku
	while (array_1_index >= 0) {
		result_array[result_array_index] = array_1[array_1_index];
		result_array_index--;
		array_1_index--;
	}

	//jezeli w liczbie2 zostaly jakies cyfry (liczba1 byla krotsza) to przepisuja je do wyniku
	while (array_2_index >= 0) {
		result_array[result_array_index] = array_2[array_2_index];
		result_array_index--;
		array_2_index--;
	}

	//teraz przechodze po tablicy wynikowej od tylu i w miejscach gdzie suma z wczesniejszego dodawania jest wieksza od 9 (wystepuje przeniesienie)
	//to wtedy odejmuje od danej liczby 10 i przenosze to na liczbe przed ni¹ zwiekszajac j¹ o jeden
	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] > 9) {
			result_array[result_array_index] -= 10;
			result_array[result_array_index - 1]++;
		}
		result_array_index--;
	}

	//zliczam ile wystepuje wiodacych zer w wyniku, niebedacych jedyn¹ cyfr¹ w liczbie (dlatego result_length-1) 
	int leading_zeros = 0;
	while ((leading_zeros < result_length-1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	//jezeli sa jakies wiodace zera do usuniecia
	if (leading_zeros > 0) {
		//aktualizuje dlugosc wyniku z uwzglednieniem wiodacych zer
		result_length -= leading_zeros;
		//kopiuje cyfry niebedace zerami do usuniecia do nowej tablicy o poprawnej dlugosci (pomijam je dzieki przesunieciu [i+leading_zeros])
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		//dealokuje star¹ tablice
		delete[] result_array;
		//nowa tablice przypisujê jako tablice wynikow¹
		result_array = temp;
	}
	//zwracam wynik w postaci tablicy cyfr
	return result_array;
}

//funkcja pomocnicza implementujaca odejmowanie pisemne dwoch liczb dodatnich
int* CNumber::substract(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//funkcja pomocnicza realizujaca odejmowanie dwoch liczb nieujemnych
	//jest to implementacja odwzorowujaca algorytm odejmowania pisemnego "w slupku"
	//do funkcji przekazuje referencje na zmienna przechowujaca dlugosc wyniku w obiekcie wynikowym z operatora
	//dzieki czemu moge modyfikowac ja wewnatrz tej funkcji pomocniczej
	
	//dlugosc wyniku moze byc maksymalnie rowna dlugosci dluzszej z liczb
	result_length = std::max(length_1, length_2);

	//bede przechodzil po liczbach od tylu, tak jak w odejmowaniu pisemnym
	int array_1_index = length_1 - 1;
	int array_2_index = length_2 - 1;
	int result_array_index = result_length - 1;

	//tworze pusta tablice na wynik
	int* result_array = new int[result_length];


	//przechodze od tylu po odejmowanych liczbach dodajac je i roznice zapisujac do tablicy wynikowej do czasu az nie dojde do konca(tutaj poczatku)
	//ktorejs z nich
	while ((array_1_index >= 0) && (array_2_index >= 0)) {
		result_array[result_array_index] = array_1[array_1_index] - array_2[array_2_index];
		result_array_index--;
		array_1_index--;
		array_2_index--;
	}

	//jezeli w liczbie1 zostaly jakies cyfry (liczba2 byla krotsza) to przepisuje je do wyniku
	while (array_1_index >= 0) {
		result_array[result_array_index] = array_1[array_1_index];
		result_array_index--;
		array_1_index--;
	}

	//jezeli w liczbie2 zostaly jakies cyfry (liczba1 byla krotsza) to przepisuje je do wyniku ale z ujemnym znakiem poniewaz jest to odjemnik
	while (array_2_index >= 0) {
		result_array[result_array_index] = array_2[array_2_index] * (-1);
		result_array_index--;
		array_2_index--;
	}

	//teraz przechodze po tablicy wynikowej od tylu i w miejscach gdzie roznica z wczesniejszego odejmowania jest mniejsza od 0
	//(wystepuje potrzeba pozyczenia) to wtedy dodaje do danej liczby 10 i zabieram to z cyfry przed nia, pomniejszajac ja o 1
	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] < 0) {
			result_array[result_array_index] += 10;
			result_array[result_array_index - 1]--;
		}
		result_array_index--;
	}

	//zliczam ile wystepuje wiodacych zer w wyniku, niebedacych jedyn¹ cyfr¹ w liczbie (dlatego result_length-1) 
	int leading_zeros = 0;
	while ((leading_zeros < result_length-1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	//jezeli sa jakies wiodace zera do usuniecia
	if (leading_zeros > 0) {
		//aktualizuje dlugosc wyniku z uwzglednieniem wiodacych zer
		result_length -= leading_zeros;

		//kopiuje cyfry niebedace zerami do usuniecia do nowej tablicy o poprawnej dlugosci (pomijam je dzieki przesunieciu [i+leading_zeros])
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		//dealokuje star¹ tablice
		delete[] result_array;

		//nowa tablice przypisujê jako tablice wynikow¹
		result_array = temp;
	}
	//zwracam wynik w postaci tablicy cyfr
	return result_array;
}

//funkcja pomocnicza implementujaca mnozenie pisemne dwoch liczb dodatnich
int* CNumber::multiply(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//funkcja pomocnicza realizujaca mnozenie dwoch liczb nieujemnych
	//jest to implementacja odwzorowujaca algorytm mnozenia pisemnego "w slupku"
	//do funkcji przekazuje referencje na zmienna przechowujaca dlugosc wyniku w obiekcie wynikowym z operatora
	//dzieki czemu moge modyfikowac ja wewnatrz tej funkcji pomocniczej
	
	//dlugosc wyniku moze byc maksymalnie rowna sumie dlugosci mnozonych liczb
	result_length = length_1 + length_2;

	//inicjuje tablice wynikowa i wypelniam ja wartosciami poczatkowymi=0 poniewaz w odroznieniu od dodawania i odejmowania tutaj bede
	//dodawal do pozycji w tablicy wynikowej zamiast ich nadpisywac
	int* result_array = new int[result_length];
	for (int i = 0; i < result_length; i++) {
		result_array[i] = 0;
	}

	//analogicznie do mnozenia pisemnego biore cyfre z mnoznika i mnoze j¹ razy kazda cyfre mnoznej dodajac otrzymana liczbe do odpowiedniej
	//pozycji w tablicy wynikowej, nastepnie to samo z kolejna od tylu cyfra mnoznika, az do konca
	//zmienne multiplier i multiplicand wewnatrz petli sluza do uzyskania po¿¹danego przesuniecia odpowiadajace zapisywaniu liczb o jedna pozycje
	//w lewo podczas mnozenia pisemnego

	//dla kazdej cyfry mnoznika
	for (int multiplier = 0; multiplier < length_2; multiplier++) {

		//dla kazdej cyfry mno¿nej
		for (int multiplicand = 0; multiplicand < length_1; multiplicand++) {
			result_array[result_length - 1 - multiplier - multiplicand] += array_1[length_1 - 1 - multiplicand] * array_2[length_2 - 1 - multiplier];
		}
	}


	//przechodze po tablicy wynikowej od tylu, wykrywam potrzebe przeniesienia tam gdzie suma uzyskana z poszczegolnych wczesniejszych mno¿eñ
	//jest wieksza niz 9, wartosc przeniesienie uzyskuje z uzyciem dzielenia calkowitego przez 10, na wskazanej pozycji zostawiam wynik
	//dzielenia modulo przez 10 (cyfre jednosci analizowanej liczby) i otrzymane przeniesienie dodaje do liczby ktora jest bezposrednio przed obecnie
	//analizowan¹ liczb¹
	int carry;
	int result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] > 9) {
			carry = result_array[result_array_index] / 10;
			result_array[result_array_index] %= 10;
			result_array[result_array_index - 1] += carry;
		}
		result_array_index--;
	}

	//zliczam ile wystepuje wiodacych zer w wyniku, niebedacych jedyn¹ cyfr¹ w liczbie (dlatego result_length-1) 
	int leading_zeros = 0;
	while ((leading_zeros < result_length - 1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	//jezeli sa jakies wiodace zera do usuniecia
	if (leading_zeros > 0) {
		//aktualizuje dlugosc wyniku z uwzglednieniem wiodacych zer
		result_length -= leading_zeros;
		//kopiuje cyfry niebedace zerami do usuniecia do nowej tablicy o poprawnej dlugosci (pomijam je dzieki przesunieciu [i+leading_zeros])
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		//dealokuje star¹ tablice
		delete[] result_array;
		//nowa tablice przypisujê jako tablice wynikow¹
		result_array = temp;
	}
	//zwracam wynik w postaci tablicy cyfr
	return result_array;
}

//funkcja pomocnicza implementujaca dzielenie dwoch liczb dodatnich, gdzie pierwsza jest wieksza od drugiej
int* CNumber::divide(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//funkcja pomocnicza implementujaca dzielenie calkowite wykorzystujac algorytm odejmowania dzielnika od dzielnej
	//tak dlugo az dzielna bedzie mniejsza od dzielnika, w ten sposob sprawdzamy ile razy dzielnik zmiesci sie caly w dzielnej
	//otrzymujac tym samym pozadany wynik dzielenia calkowitego
	//tu rowniez wewnatrz funkcji aktualizuje zmienna result_length ktora bedzie dlugoscia obiektu wynikowego w operatorze
	
	//inicjuje wynik jako obiekt cnumber=0 (wynik dzielenia moze byc wiekszy niz zakres int)
	CNumber result(0);
	//inicjuje tymczasowa dzielna oraz dzielnik jako modu³y tych liczb
	CNumber dividend(array_1, length_1, false);
	CNumber divisor(array_2, length_2, false);
	
	//dopoki dzielna nie jest mniejsza od dzielnika
	while (!(dividend < divisor)) {
		//zwiekszam wynik o jeden i odejmuje dzielnik
		result = result + 1;
		dividend = dividend - divisor;
	}

	//inicjuje tablice ktora bedzie wynikiem, bedzie ona kopi¹ tablicy znajdujacej sie w obiekcie result 
	// (obiekt result wraz ze wspomniana tablica zostanie usuniety w momencie zakonczenia wywolania funkcji, stad potrzeba kopii)
	int* result_array = new int[result.length];
	for (int i = 0; i < result.length; i++) {
		result_array[i] = result.digits_array[i];
	}
	
	//aktualizuje dlugosc obiektu wynikowego
	result_length = result.length;

	//zwracam wynik w postaci otrzymanej tablicy cyfr
	return result_array;
}

//funkcja pomocnicza wykorzystywana do okreslenia czy modul jednej liczby jest wiekszy od modulu drugiej liczby
bool CNumber::absolute_is_1_less_than_2(const CNumber& cnum1, const CNumber& cnum2) {
	//aby porownac obie liczby w modulach tworze dwa tymczasowe obiekty bedace ich kopia i ustawiam je jako nieujemne dzieki czemu moge uzyc
	//zaimplementowany wczesniej operator porownania

	CNumber abs1(cnum1);
	CNumber abs2(cnum2);
	abs1.is_negative = false;
	abs2.is_negative = false;
	return (abs1 < abs2);
}