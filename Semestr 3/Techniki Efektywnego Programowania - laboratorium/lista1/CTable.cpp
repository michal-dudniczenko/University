#include <iostream>
#include "CTable.h"


void CTable::modify_tab(CTable* ctable, int new_size) {
	ctable->set_new_size(new_size);
}

void CTable::modify_tab(CTable ctable, int new_size) {
	ctable.set_new_size(new_size);
}

CTable::CTable() {
	this->name = default_name;
	this->length = default_length;
	this->array = new int[default_length];
	std::cout << "bezp: '" << this->name << "'\n";
}

CTable::CTable(std::string name, int length){
	this->name = name;
	this->length = length;
	this->array = new int[length];
	std::cout << "parametr: '" << this->name << "'\n";
}

CTable::CTable(CTable& other) {
	this->name = other.name + "_copy";
	this->length = other.length;
	this->array = new int[length];
	for (int i = 0; i < length; i++) {
		this->array[i] = other.array[i];
	}
	std::cout << "kopiuj '" << name << '\n';
}

CTable::~CTable() {
	std::cout << "usuwam: '" << name << "'\n";
	delete[] array;
}

void CTable::set_name(std::string name) {
	this->name = name;
}

bool CTable::set_new_size(int new_size) {
	if (new_size <= 0) {
		return false;
	}

	int* new_array = new int[new_size];
	for (int i = 0; i < std::min(this->length, new_size); i++) {
		new_array[i] = this->array[i];
	}
	delete[] this->array;
	this->array = new_array;
	this->length = new_size;
	return true;
}

CTable* CTable::clone() {
	CTable* temp = new CTable(*this);
	return temp;
}

void CTable::display() {
	std::cout << "array: " << this->name << " length: " << this->length << "\n";
}