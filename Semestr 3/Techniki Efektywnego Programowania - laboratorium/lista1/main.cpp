#include <iostream>
#include "functions.h"
#include "CTable.h"

int main() {
	alloc_array_fill_34(10);

	int** array_2d;

	alloc_array_2d(&array_2d, 3, 4);
	dealloc_array_2d(array_2d, 3);

	CTable ctab1;
	CTable* ctab2 = new CTable("dynamic", 10);
	delete ctab2;

	std::cout << "\ncreating ctable[] array\n";
	CTable* ctab_array = new CTable[5];
	std::cout << "\ndeleting ctable[] array\n";
	delete[] ctab_array;
	std::cout << std::endl;

	CTable mod_test_1("test_1", 10);
	mod_test_1.display();
	CTable::modify_tab(mod_test_1, 20);
	mod_test_1.display();

	CTable mod_test_2("test_2", 10);
	mod_test_2.display();
	CTable::modify_tab(&mod_test_2, 20);
	mod_test_2.display();

	return 0;
}