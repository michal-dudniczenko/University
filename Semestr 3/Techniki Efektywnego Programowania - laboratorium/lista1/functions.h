//w pliku functions znajdziemy 3 funkcje majace na celu prezentacje mechanizmow obecnych przy alokacji/dealokacji dynamicznych tablic
//wraz z deklaracjami wykorzystywanych stalych

#ifndef FUNCTIONS_H
#define FUNCTIONS_H

#include <string>

const int array_fill = 34;
const std::string err_wrong_size = "Incorrect array size.\n";
const std::string array_content = "Array content: ";
const std::string newline = "\n";

void alloc_array_fill_34(int array_size);

bool alloc_array_2d(int*** array_2d, int size_x, int size_y);

bool dealloc_array_2d(int*** array_2d, int size_x);

#endif