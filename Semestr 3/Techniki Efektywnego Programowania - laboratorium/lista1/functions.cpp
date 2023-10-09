#include <iostream>
#include "functions.h"

void alloc_array_fill_34(int array_size) {
	if (array_size <= 0) {
		std::cout << "Error! Array size must be greater than 0.";
		return;
	}

	int* array = new int[array_size];

	for (int i = 0; i < array_size; i++) {
		array[i] = array_fill;
	}
	std::cout << "Array content: ";
	for (int i = 0; i < array_size; i++) {
		std::cout << array[i] << " ";
	}
	std::cout << "\n";
	delete[] array;
}

bool alloc_array_2d(int*** array_2d, int size_x, int size_y) {
	if (size_x <= 0 || size_y <= 0) {
		std::cout << "Incorrect array size.\n";
		return false;
	}

	*array_2d = new int* [size_x];

	for (int i = 0; i < size_x; i++) {
		(*array_2d)[i] = new int[size_y];
		if ((*array_2d)[i] == nullptr) {
			for (int j = 0; j < i; j++) {
				delete[](*array_2d)[j];
			}
			delete[] * array_2d;
			return false;
		}
	}
	return true;
}

bool dealloc_array_2d(int** array_2d, int size_x, int size_y) {
	if (array_2d == nullptr) {
		return false;
	}
	for (int i = 0; i < size_x; i++) {
		delete[] array_2d[i];
	}
	delete[] array_2d;
	return true;
}

bool dealloc_array_2d_better(int** array_2d) {
	if (array_2d == nullptr) {
		return false;
	}
	for (int i = 0; i < sizeof(array_2d); i++) {
		delete[] array_2d[i];
	}
	delete[] array_2d;
	return true;
}