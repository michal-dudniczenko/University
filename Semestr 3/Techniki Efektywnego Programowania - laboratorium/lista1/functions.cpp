#include <iostream>
#include "functions.h"

void alloc_array_fill_34(int array_size) {
	if (array_size <= 0) {
		std::cout << err_wrong_size;
		return;
	}

	int* array = new int[array_size];

	for (int i = 0; i < array_size; i++) {
		array[i] = array_fill;
	}
	std::cout << array_content;
	for (int i = 0; i < array_size; i++) {
		std::cout << array[i] << " ";
	}
	std::cout << newline;
	delete[] array;
}

bool alloc_array_2d(int*** array_2d, int size_x, int size_y) {
	if (size_x <= 0 || size_y <= 0) {
		std::cout << err_wrong_size;
		return false;
	}

	*array_2d = new int*[size_x];
	if (*array_2d == NULL) {
		return false;
	}

	for (int i = 0; i < size_x; i++) {
		(*array_2d)[i] = new int[size_y];
		if ((*array_2d)[i] == NULL) {
			for (int j = 0; j < i; j++) {
				delete[](*array_2d)[i];
			}
			delete[] *array_2d;
			return false;
		}
	}
	return true;
}

bool dealloc_array_2d(int*** array_2d, int size_x) {
	if (size_x <= 0) {
		std::cout << err_wrong_size;
		return false;
	}

	if (*array_2d == NULL) {
		return false;
	}
	
	for (int i = 0; i < size_x; i++) {
		delete[] (*array_2d)[i];
	}
	delete[] *array_2d;
	return true;
}