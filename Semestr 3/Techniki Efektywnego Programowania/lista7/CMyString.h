//klasa sluzy do obslugi zmiennych typu string, ktore przechowywane sa w postaci tablicy typu char
//opracowano:
//konstruktory: bezparametrowy, kopiujacy, parametryzowany bezposrednio podanym stringiem, przenoszacy
//destruktor, operatory =, +, +=, rzutowania do bool
//metode toString zwracajaca przechowywany string w formie std::string
//metode length zwracajaca dlugosc stringa
//metode at(index) zwracajaca znak na podanej pozycji
//operator == sluzacy do porownywania obiektow CMyString


#pragma once

#include <string>


class CMyString {
private:
	char* text;
	int _length;

	CMyString(int length);

public:
	CMyString();
	CMyString(const CMyString& other);
	CMyString(const char* text);
	CMyString(CMyString&& other) noexcept;
	~CMyString();

	void operator=(const CMyString& other);
	void operator=(const char* text);

	void operator+=(const CMyString& other);
	void operator+=(const char* text);

	CMyString operator+(const CMyString& other);
	CMyString operator+(const char* text);

	std::string toString();

	operator bool() const;

	int length();

	char at(int index);

	bool operator==(const CMyString& other);
};