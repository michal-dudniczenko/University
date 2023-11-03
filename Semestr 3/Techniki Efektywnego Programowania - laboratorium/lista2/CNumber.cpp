#include <iostream>
#include <string>
#include "CNumber.h"

CNumber::CNumber() {
	this->length = default_length;
	this->is_negative = false;
	this->digits_array = new int[this->length];
}

CNumber::CNumber(int number) {
	if (number == 0) {
		this->length = 1;
		this->digits_array = new int[1];
		this->digits_array[0] = 0;
		this->is_negative = false;
		return;
	}

	if (number < 0) {
		this->is_negative = true;
		number *= (-1);
	}
	else {
		this->is_negative = false;
	}

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
	this->length = other.length;
	this->is_negative = other.is_negative;
	this->digits_array = new int[this->length];
	for (int i = 0; i < this->length; i++) {
		this->digits_array[i] = other.digits_array[i];
	}
}

CNumber::CNumber(int*& array, int length, bool is_negative) {
	this->digits_array = new int[length];
	for (int i = 0; i < length; i++) {
		this->digits_array[i] = array[i];
	}
	this->length = length;
	this->is_negative = is_negative;
}

CNumber::~CNumber() {
	delete[] this->digits_array;
}

void CNumber::operator=(const int value) {
	delete[] this->digits_array;

	if (value == 0) {
		this->length = 1;
		this->digits_array = new int[1];
		this->digits_array[0] = 0;
		this->is_negative = false;
		return;
	}

	int temp = value;
	if (temp < 0) {
		this->is_negative = true;
		temp *= (-1);
	}
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
	this->is_negative = other.is_negative;
}

CNumber CNumber::operator+(const CNumber& other) const {
	CNumber result;
	int result_length;
	int* result_array;

	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}
	
	if (this->is_negative) {
		//ujemna + ujemna
		if (other.is_negative) {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result.is_negative = true;
		}
		//ujemna + dodatnia czyli other - this
		else {
			if (absolute_is_1_less_than_2(other, *this)) {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result.is_negative = true;
			}
			else {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result.is_negative = false;
			}
		}
	}
	else {
		//dodatnia + ujemna czyli this - other
		if (other.is_negative) {
			if (absolute_is_1_less_than_2(*this, other)) {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result.is_negative = true;
			}
			else {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result.is_negative = false;
			}
		}
		//dodatnia + dodatnia
		else {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result.is_negative = false;
		}
	}
	
	delete[] array_1;
	delete[] array_2;

	result.digits_array = result_array;
	result.length = result_length;

	return result;
}

CNumber CNumber::operator-(const CNumber& other) const {
	CNumber result;
	int result_length;
	int* result_array;

	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}

	if (this->is_negative) {
		//ujemna1 - ujemna2   czyli other - this
		if (other.is_negative) {
			if (absolute_is_1_less_than_2(other, *this)) {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result.is_negative = true;
			}
			else {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result.is_negative = false;
			}
		}
		//ujemna - dodatnia
		else {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result.is_negative = true;
		}
	}
	else {
		//dodatnia - ujemna czyli this+other
		if (other.is_negative) {
			result_array = sum(array_1, array_2, this->length, other.length, result_length);
			result.is_negative = false;
		}
		//dodatnia - dodatnia czyli this-other
		else {
			if (absolute_is_1_less_than_2(*this, other)) {
				result_array = substract(array_2, array_1, other.length, this->length, result_length);
				result.is_negative = true;
			}
			else {
				result_array = substract(array_1, array_2, this->length, other.length, result_length);
				result.is_negative = false;
			}
		}
	}

	delete[] array_1;
	delete[] array_2;

	result.digits_array = result_array;
	result.length = result_length;

	return result;
}

CNumber CNumber::operator*(const CNumber& other) const {
	CNumber result;
	int result_length;
	int* result_array;

	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}

	result_array = multiply(array_1, array_2, this->length, other.length, result_length);
	if (this->is_negative && other.is_negative) {
		result.is_negative = false;
	}
	else if (this->is_negative || other.is_negative) {
		result.is_negative = true;
	}
	else {
		result.is_negative = false;
	}

	delete[] array_1;
	delete[] array_2;

	result.digits_array = result_array;
	result.length = result_length;

	return result;
}

CNumber CNumber::operator/(const CNumber& other) const {
	if (other.digits_array[0] == 0) {
		std::cout<< "ERROR: Division by zero!\n";
		return CNumber(0);
	}
	if (this->digits_array[0] == 0 || absolute_is_1_less_than_2(*this, other)) {
		return CNumber(0);
	}

	CNumber result;
	int result_length;
	int* result_array;

	int* array_1 = new int[this->length];
	int* array_2 = new int[other.length];
	for (int i = 0; i < this->length; i++) {
		array_1[i] = this->digits_array[i];
	}
	for (int i = 0; i < other.length; i++) {
		array_2[i] = other.digits_array[i];
	}

	result_array = divide(array_1, array_2, this->length, other.length, result_length);

	if (this->is_negative && other.is_negative) {
		result.is_negative = false;
	}
	else if (this->is_negative || other.is_negative) {
		result.is_negative = true;
	}
	else {
		result.is_negative = false;
	}

	delete[] array_1;
	delete[] array_2;

	result.digits_array = result_array;
	result.length = result_length;

	return result;
}

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

		//porownanie jezeli maja takie same dlugosci
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

		//porownanie jezeli maja takie same dlugosci
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return true;
			else if (this->digits_array[i] < other.digits_array[i]) return false;
		}
		//okazaly sie rowne
		return false;
	}
}

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

		//porownanie jezeli maja takie same dlugosci
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

		//porownanie jezeli maja takie same dlugosci
		for (int i = 0; i < this->length; i++) {
			if (this->digits_array[i] > other.digits_array[i]) return false;
			else if (this->digits_array[i] < other.digits_array[i]) return true;
		}
		//okazaly sie rowne
		return false;
	}
}

bool CNumber::operator==(const CNumber& other) const {
	if (this == &other) return true;
	if (this->length != other.length || this->is_negative != other.is_negative) return false;
	for (int i = 0; i < this->length; i++) {
		if (this->digits_array[i] != other.digits_array[i]) return false;
	}
	return true;
}

std::string CNumber::to_string() const {
	std::string result = "number: ";
	if (this->is_negative) result += "-";
	for (int i = 0; i < this->length; i++) {
		result += std::to_string(this->digits_array[i]);
	}
	result += " length: " + std::to_string(this->length);
	return result;
}

int* CNumber::sum(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//dodawanie dwoch liczb dodatnich
	
	result_length = std::max(length_1, length_2) + 1;

	int array_1_index = length_1 - 1;
	int array_2_index = length_2 - 1;
	int result_array_index = result_length - 1;

	int* result_array = new int[result_length];
	result_array[0] = 0;

	while ((array_1_index >= 0) && (array_2_index >= 0)) {
		result_array[result_array_index] = array_1[array_1_index] + array_2[array_2_index];
		result_array_index--;
		array_1_index--;
		array_2_index--;
	}

	while (array_1_index >= 0) {
		result_array[result_array_index] = array_1[array_1_index];
		result_array_index--;
		array_1_index--;
	}

	while (array_2_index >= 0) {
		result_array[result_array_index] = array_2[array_2_index];
		result_array_index--;
		array_2_index--;
	}

	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] > 9) {
			result_array[result_array_index] -= 10;
			result_array[result_array_index - 1]++;
		}
		result_array_index--;
	}

	int leading_zeros = 0;
	while ((leading_zeros < result_length-1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	if (leading_zeros > 0) {
		result_length -= leading_zeros;
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		delete[] result_array;
		result_array = temp;
	}
	return result_array;
}

int* CNumber::substract(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//odejmowanie dwoch liczb dodatnich, pierwsza wieksza od drugiej
	
	result_length = std::max(length_1, length_2) + 1;

	int array_1_index = length_1 - 1;
	int array_2_index = length_2 - 1;
	int result_array_index = result_length - 1;

	int* result_array = new int[result_length];
	result_array[0] = 0;

	while ((array_1_index >= 0) && (array_2_index >= 0)) {
		result_array[result_array_index] = array_1[array_1_index] - array_2[array_2_index];
		result_array_index--;
		array_1_index--;
		array_2_index--;
	}

	while (array_1_index >= 0) {
		result_array[result_array_index] = array_1[array_1_index];
		result_array_index--;
		array_1_index--;
	}

	while (array_2_index >= 0) {
		result_array[result_array_index] = array_2[array_2_index] * (-1);
		result_array_index--;
		array_2_index--;
	}

	result_array_index = result_length - 1;
	while (result_array_index > 0) {
		if (result_array[result_array_index] < 0) {
			result_array[result_array_index] += 10;
			result_array[result_array_index - 1]--;
		}
		result_array_index--;
	}

	int leading_zeros = 0;
	while ((leading_zeros < result_length-1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	if (leading_zeros > 0) {
		result_length -= leading_zeros;
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		delete[] result_array;
		result_array = temp;
	}
	return result_array;
}

int* CNumber::multiply(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//mnozenie dwoch liczb
	
	result_length = length_1 + length_2;
	int* result_array = new int[result_length];
	for (int i = 0; i < result_length; i++) {
		result_array[i] = 0;
	}

	for (int multiplier = 0; multiplier < length_2; multiplier++) {
		for (int multiplicand = 0; multiplicand < length_1; multiplicand++) {
			result_array[result_length - 1 - multiplier - multiplicand] += array_1[length_1 - 1 - multiplicand] * array_2[length_2 - 1 - multiplier];
		}
	}

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

	int leading_zeros = 0;
	while ((leading_zeros < result_length - 1) && (result_array[leading_zeros] == 0)) {
		leading_zeros++;
	}

	if (leading_zeros > 0) {
		result_length -= leading_zeros;
		int* temp = new int[result_length];
		for (int i = result_length - 1; i >= 0; i--) {
			temp[i] = result_array[i + leading_zeros];
		}
		delete[] result_array;
		result_array = temp;
	}
	return result_array;
}

int* CNumber::divide(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length) {
	//dzielenie dwoch liczb dodatnich, pierwsza wieksza od drugiej
	
	CNumber result(0);
	CNumber dividend(array_1, length_1, false);
	CNumber divisor(array_2, length_2, false);
	CNumber acc_divisor(divisor);
	
	while (!(acc_divisor > dividend)) {
		result = result + 1;
		acc_divisor = acc_divisor + divisor;
	}

	int* result_array = new int[result.length];
	for (int i = 0; i < result.length; i++) {
		result_array[i] = result.digits_array[i];
	}

	result_length = result.length;

	return result_array;
}

bool CNumber::absolute_is_1_less_than_2(const CNumber& cnum1, const CNumber& cnum2) {
	CNumber abs1(cnum1);
	CNumber abs2(cnum2);
	abs1.is_negative = false;
	abs2.is_negative = false;
	return (abs1 < abs2);
}