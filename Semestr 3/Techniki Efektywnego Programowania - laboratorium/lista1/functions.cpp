#include <iostream>
#include "functions.h"

//funkcja alokuje dynamiczna tablice o podanym rozmiarze, wypelnia ja wartosciami 34, wypisuje stan tablicy i dealokuje tablice
void alloc_array_fill_34(int array_size) {
	//jezeli podana dlugosc jest <=0 wypisz informacje o bledzie i zakoncz 
	if (array_size <= 0) {
		std::cout << err_wrong_size;
		return;
	}

	//alokacja dynamicznej tablicy o podanym rozmiarze
	int* array = new int[array_size];

	//wypelnienie tablicy wartosciami 34
	for (int i = 0; i < array_size; i++) {
		array[i] = array_fill;
	}
	//wypisywanie zawartosci tablicy
	std::cout << array_content;
	for (int i = 0; i < array_size; i++) {
		std::cout << array[i] << " ";
	}
	std::cout << newline;

	//dealokacja tablicy
	delete[] array;
}

//funkcja alokuje dynamiczna dwuwymiarowa tablice(macierz), zwraca informacje czy alokacja sie udala
//jezeli zwroci true to podany wskaznik w parametrze bedzie wskazywal na nowo zaalokowana tablice 
bool alloc_array_2d(int*** array_2d, int size_x, int size_y) {
	//jezeli ktorys z podanych wymiarow jest <=0 wypisz informacje o bledzie i zakoncz
	if (size_x <= 0 || size_y <= 0) {
		std::cout << err_wrong_size;
		return false;
	}

	//dynamiczna alokacja pierwszego wymiaru tablicy, tablicy wskaznikow na dynamiczne tablice bedace wierszami 
	*array_2d = new int*[size_x];
	//jezeli alokacja sie nie powiodla zwroc falsz
	if (*array_2d == NULL) {
		return false;
	}

	//dynamiczna alokacja poszczegolnych wierszy tablicy
	for (int i = 0; i < size_x; i++) {
		(*array_2d)[i] = new int[size_y];
		//jezeli alokacja wiersza sie nie udala
		if ((*array_2d)[i] == NULL) {
			//dealokuj wszystkie do tej pory zaalokowane wiersze
			for (int j = 0; j < i; j++) {
				delete[](*array_2d)[i];
			}
			//dealokacja tablicy wskaznikow (pierwszego wymiaru)
			delete[] *array_2d;
			//zwroc falsz
			return false;
		}
	}
	//wszystko po drodze sie udalo, zwroc prawda
	return true;
}


//funkcja dealokuje dynamicznie zaalokowana tablice dwuwymiarowa na ktora wskaznik podajemy w parametrze, zwraca informacje czy operacja sie udala
bool dealloc_array_2d(int** array_2d, int size_x) {
	//blednie podany rozmiar, wypisz informacje o bledzie i zwroc falsz
	if (size_x <= 0) {
		std::cout << err_wrong_size;
		return false;
	}

	//jezeli nie ma czego dealokowac zwroc falsz
	if (array_2d == NULL) {
		return false;
	}
	
	//dla kazdej dynamicznej tablicy wewnetrznej (dla kazdego wiersza) nastepuje jej dealokacja
	for (int i = 0; i < size_x; i++) {
		//dodatkowo sprawdzamy czy wiersz nie zostal przypadkiem nieprawidlowo zaalokowany
		if (array_2d[i] != NULL) {
			delete[] array_2d[i];
		}
	}
	//dealokacja tablicy pierwszego wymiaru, tablicy wskaznikow
	delete[] array_2d;
	//wszystko zdealokowano, zwroc prawda
	return true;
}