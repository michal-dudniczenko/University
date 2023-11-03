#ifndef CNUMBER_H
#define CNUMBER_H

#include <string>

const int default_length = 1;

class CNumber {
private:
	int* digits_array;
	int length;
	bool is_negative;

public:
	CNumber();
	CNumber(int number);
	CNumber(const CNumber& other);
	CNumber(int*& array, int length, bool is_negative);
	~CNumber();

	void operator=(const int value);
	void operator=(const CNumber& other);
	CNumber operator+(const CNumber& other) const;
	CNumber operator-(const CNumber& other) const;
	CNumber operator*(const CNumber& other) const;
	CNumber operator/(const CNumber& other) const;
	bool operator>(const CNumber& other) const;
	bool operator<(const CNumber& other) const;
	bool operator==(const CNumber& other) const;

	std::string to_string() const;

private:
	static int* sum(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length);
	static int* substract(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length);
	static int* multiply(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length);
	static int* divide(int*& array_1, int*& array_2, int length_1, int length_2, int& result_length);
	static bool absolute_is_1_less_than_2(const CNumber& cnum1, const CNumber& cnum2);
};

#endif