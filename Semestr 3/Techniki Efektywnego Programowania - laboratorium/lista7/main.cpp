#include <iostream>

#include "CMyString.h"

using namespace std;


int main() {
	CMyString c_str;
	c_str = "Ala ma ";
	c_str += "kota ";
	c_str = (c_str + "i psa");

	CMyString c_str2 = "Ala ma kota i psa";
	CMyString c_str3("some text");

	cout << c_str.toString() << endl;
	cout << "char at index 1: " << c_str.at(1) << endl;
	cout << "string length: " << c_str.length() << endl;
	cout << "c_str == c_str2 ? " << (c_str == c_str2) << endl;
	cout << "c_str == c_str3 ? " << (c_str == c_str3) << endl;
}