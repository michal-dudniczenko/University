#include <iostream>
#include "CNumber.h"

int main() {
	CNumber c_num_0, c_num_1;
	c_num_0 = 768;
	c_num_1 = 9567;
	c_num_0 = c_num_0 + c_num_1;
	std::cout<< c_num_0.to_string();
	return 0;
}