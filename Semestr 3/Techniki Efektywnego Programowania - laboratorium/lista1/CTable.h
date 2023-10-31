#ifndef CTABLE_H
#define CTABLE_H

#include <string>

const std::string default_name = "default_name";
const int default_length = 10;

class CTable {
private:
	std::string name;
	int length;
	int* array;
	
public:
	CTable();
	CTable(std::string name, int length);
	CTable(CTable &other);
	~CTable();

	void set_name(std::string name);
	bool set_new_size(int new_size);
	CTable* clone();

	void display();

	static void modify_tab(CTable* ctable, int new_size);
	static void modify_tab(CTable ctable, int new_size);
};

#endif