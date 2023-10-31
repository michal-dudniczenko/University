#ifndef CNUMBER_H
#define CNUMBER_H

#include <string>

const int default_length = 10;

class CNumber {
private:
	int* digits_array;
	int length;

public:
	CNumber();
	CNumber(int number);
	CNumber(const CNumber& other);
	~CNumber();

	void operator=(const int value);
	void operator=(const CNumber& other);

	CNumber operator+(const CNumber& other);
	void operator-(const CNumber& other);
	void operator*(const CNumber& other);
	void operator/(const CNumber& other);

	std::string to_string();
};

#endif