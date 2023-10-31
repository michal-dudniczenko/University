#include <iostream>
#include <string>
#include "CNumber.h"

CNumber::CNumber() {
	this->length = default_length;
	this->digits_array = new int[default_length];
	std::cout << "konstruktor bezp.\n";
}

CNumber::CNumber(int number) {
	std::cout << "konstruktor param.\n";
	int temp = number;
	int length_count = 0;
	while (temp > 0) {
		length_count++;
		temp /= 10;
	}
	this->length = length_count;
	this->digits_array = new int[length_count];
	for (int i = length_count-1; i >= 0; i--) {
		digits_array[i] = number % 10;
		number /= 10;
	}
	
}

CNumber::CNumber(const CNumber& other) {
	std::cout << "konstruktor kop.\n";
	if (this == &other) {
		return;
	}
	this->length = other.length;
	this->digits_array = new int[this->length];
	for (int i = 0; i < this->length; i++) {
		this->digits_array[i] = other.digits_array[i];
	}
}

CNumber::~CNumber() {
	std::cout << "destruktor\n";
	delete[] this->digits_array;
}


void CNumber::operator=(const int value) {
	delete[] this->digits_array;
	int temp = value;
	int length_count = 0;
	while (temp > 0) {
		length_count++;
		temp /= 10;
	}
	this->length = length_count;
	this->digits_array = new int[length_count];
	temp = value;
	for (int i = length_count - 1; i >= 0; i--) {
		digits_array[i] = temp % 10;
		temp /= 10;
	}

}

void CNumber::operator=(const CNumber& other) {
	if (this == &other) {
		return;
	}
	delete[] this->digits_array;

	this->length = other.length;
	this->digits_array = new int[this->length];
	for (int i = 0; i < this->length; i++) {
		this->digits_array[i] = other.digits_array[i];
	}
}

CNumber CNumber::operator+(const CNumber& other) {
	int result_length = std::max(this->length, other.length);
	
	int this_array_index = this->length - 1;
	int other_array_index = other.length - 1;
	int result_array_index = result_length - 1;
	
	int* result_array = new int[result_length];

	while (this_array_index >= 0 && other_array_index >= 0) {
		result_array[result_array_index] = this->digits_array[this_array_index] + other.digits_array[other_array_index];
		result_array_index--;
		this_array_index--;
		other_array_index--;
	}
	
	while (this_array_index >= 0) {
		result_array[result_array_index] = this->digits_array[this_array_index];
		result_array_index--;
		this_array_index--;
	}

	while (other_array_index >= 0) {
		result_array[result_array_index] = other.digits_array[other_array_index];
		result_array_index--;
		other_array_index--;
	}
	for (int i = 0; i < result_length; i++) {
		std::cout << result_array[i] << "\n";
	}

	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] > 9) {
			result_array[result_array_index] -= 10;
			result_array[result_array_index-1]++;
		}
		result_array_index--;
	}
	for (int i = 0; i < result_length; i++) {
		std::cout << result_array[i] << "\n";
	}
	
	this->length = result_length;

	if (result_array[0] > 9) {
		result_array[0] -= 10;
		int* temp = new int[result_length + 1];
		for (int i = 0; i < result_length; i++) {
			temp[i + 1] = result_array[i];
		}
		temp[0] = 1;
		delete[] result_array;
		result_array = temp;
		this->length++;
	}

	delete[] this->digits_array;
	this->digits_array = result_array;
	return *this;
}

void CNumber::operator-(const CNumber& other) {
	int result_length = std::max(this->length, other.length);

	int this_array_index = this->length - 1;
	int other_array_index = other.length - 1;
	int result_array_index = result_length - 1;

	int* result_array = new int[result_length];

	while (this_array_index >= 0 && other_array_index >= 0) {
		result_array[result_array_index] = this->digits_array[this_array_index] + other.digits_array[other_array_index];
		result_array_index--;
		this_array_index--;
		other_array_index--;
	}

	while (this_array_index >= 0) {
		result_array[result_array_index] = this->digits_array[this_array_index];
		result_array_index--;
		this_array_index--;
	}

	while (other_array_index >= 0) {
		result_array[result_array_index] = other.digits_array[other_array_index];
		result_array_index--;
		other_array_index--;
	}
	for (int i = 0; i < result_length; i++) {
		std::cout << result_array[i] << "\n";
	}

	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] > 9) {
			result_array[result_array_index] -= 10;
			result_array[result_array_index - 1]++;
		}
		result_array_index--;
	}
	for (int i = 0; i < result_length; i++) {
		std::cout << result_array[i] << "\n";
	}

	this->length = result_length;

	if (result_array[0] > 9) {
		result_array[0] -= 10;
		int* temp = new int[result_length + 1];
		for (int i = 0; i < result_length; i++) {
			temp[i + 1] = result_array[i];
		}
		temp[0] = 1;
		delete[] result_array;
		result_array = temp;
		this->length++;
	}

	delete[] this->digits_array;
	this->digits_array = result_array;
	return *this;
}

void CNumber::operator*(const CNumber& other)
{
}

void CNumber::operator/(const CNumber& other)
{
}

std::string CNumber::to_string() {
	std::string result = "number: ";
	for (int i = 0; i < this->length; i++) {
		result += std::to_string(this->digits_array[i]);
	}
	result += " length: " + std::to_string(this->length)+"\n";
	return result;
}