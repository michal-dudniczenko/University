#include <iostream>
#include <string>
#include "CTable.h"

//statyczna funkcja ktora dla podanego w parametrze obiektu ctable zmieni jego rozmiar (wywola funkcje sluzaca do zmiany romiaru na nowy)
//poniewaz podajemy wskaznik na podawany obiekt, modyfikacji ulegnie faktycznie podany obiekt
void CTable::modify_tab(CTable* ctable, int new_size) {
	ctable->set_new_size(new_size);
}

//bledna implementacja statycznej funkcji sluzacej do modyfikacji rozmiaru tablicy obiektu podanego w parametrze
//problem polega na tym, ze obiekt podajemy przez wartosc a nie referencje/wskaznik na niego, przez co obiekt na rzecz ktorego 
//pozniej wywolujemy metode zmieniajaca rozmiar tablicy jest obiektem utworzonym wewnatrz funkcji w wyniku wywolania konstruktora kopiujacego
//ponadto obiekt ten zostanie usuniety w momencie zakonczenia wywolania metody 
void CTable::modify_tab(CTable ctable, int new_size) {
	ctable.set_new_size(new_size);
}

//konstruktor bezparametrowy, polom klasy przypisuje domyslne wartosci, alokuje dynamiczna tablice o domyslnym rozmiarze
//sygnalizuje wywolanie wypisujac odpowiedni string
CTable::CTable() {
	this->name = default_name;
	this->length = default_length;
	this->array = new int[default_length];
	std::cout << "bezp: '" << this->name << "'\n";
}

//konstruktor parametryzowany, przypisuje podana nazwe oraz dlugosc tablicy, alokuje dynamiczna tablice o podanym rozmiarze
//sygnalizuje wywolanie wypisujac odpowiedni string
CTable::CTable(std::string name, int length){
	this->name = name;
	this->length = length;
	this->array = new int[length];
	std::cout << "parametr: '" << this->name << "'\n";
}

//konstruktor kopiujacy, kopiuje nazwe oraz dlugosc podanego obiektu ctable, alokuje dynamiczna tablice o tym samym rozmiarze i kopiuje wszystkie
//wartosci z tablicy w podanym obiekcie
//sygnalizuje wywolanie wypisujac odpowiedni string
CTable::CTable(CTable& other) {
	this->name = other.name + "_copy";
	this->length = other.length;
	this->array = new int[length];
	for (int i = 0; i < length; i++) {
		this->array[i] = other.array[i];
	}
	std::cout << "kopiuj '" << name << '\n';
}

//destruktor, wypisuje informacje o wywolaniu i nastepnie dealokuje dynamicznie zaalokowana tablice
CTable::~CTable() {
	std::cout << "usuwam: '" << name << "'\n";
	delete[] array;
}

//zmienia/ustawia nazwe tablicy na podana w parametrze
void CTable::set_name(std::string name) {
	this->name = name;
}


//zmienia dlugosc (rozmiar) tablicy na podany w parametrze
bool CTable::set_new_size(int new_size) {
	//jezeli podany nowy rozmiar <=0 zwroc falsz i zakoncz
	if (new_size <= 0) {
		return false;
	}
	//jezeli nowy rozmiar jest taki sam jak obecny, nic nie trzeba robic, zwroc prawda i zakoncz
	else if (new_size == this->length) {
		return true;
	}

	//zaalokuj nowa tablice o podanym rozmiarze
	int* new_array = new int[new_size];

	//przepisz albo wszystkie elementy starej tablicy (jezeli nowy rozmiar > stary) lub dana liczbe poczatkowych elementow (tyle ile sie zmiesci)
	for (int i = 0; i < std::min(this->length, new_size); i++) {
		new_array[i] = this->array[i];
	}
	//dealokacja starej tablicy
	delete[] this->array;
	//zmiana dlugosci tablicy na nowa oraz przypisanie wskaznikowi nowo zaalokowanej tablicy
	this->array = new_array;
	this->length = new_size;
	//zwroc informacje o powodzeniu
	return true;
}

//zwraca wskaznik na nowo utworzony obiekt bedacy kopi¹ obecnego obiektu, wykorzystuje wczesniejsza implementacje konstruktora kopiujacego
CTable* CTable::clone() {
	CTable* temp = new CTable(*this);
	return temp;
}

//wypisz informacje o nazwie i rozmiarze przechowywanej tablicy
void CTable::display() {
	std::cout << "array: " << this->name << " length: " << this->length << "\n";
}