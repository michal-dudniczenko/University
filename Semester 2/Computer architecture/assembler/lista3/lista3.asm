.data

RAM: .space 4096
text1: .asciiz "\nPodaj liczbe wierszy macierzy: "
text2: .asciiz "\nPodaj liczbe kolumn macierzy: "
text3: .asciiz "\nWybierz co chcesz zrobic: 1-zapis, 2-odczyt, 3-koniec: "
text4: .asciiz "\nPodaj numer wiersza: "
text5: .asciiz "\nPodaj numer kolumny: "
text6: .asciiz "\nSzukana liczba w macierzy: "
text7: .asciiz "\nPodaj liczbe do wpisania: "

.text

#pobieranie rozmiarow od uzytkownika

li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text1				#podaje "podaj liczbe wierszy"
syscall					#wypisz "podaj liczbe wierszy"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $s1, $v0				#zapisuje pobrana liczbe wierszy w s1

li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text2				#podaje "podaj liczbe kolumn"
syscall					#wypisz "podaj liczbe kolumn"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $s2, $v0				#zapisuje pobrane liczbe kolumn w s2

#zmienne pomocnicze

#s0-poczatek tablicy RAM
#s1-liczba wierszy
#s2-liczba kolumn
#s3-liczba wierszy * 4
#s4-liczba kolumn * 4

la $s0, RAM
sll $s3, $s1, 2
sll $s4, $s2, 2


#wypelnienie tablicy adresow wierszy
#t0 - index w tablicy wierszy
#t1 - licznik wierszy
#t2 - index poczatku wiersza

move $t0, $s0				#t0 = s0
li $t1, 0				#t1 = 0
add $t2, $s0, $s3			#t2 = s0 + s3


tablicaWierszy:				#petla do wypelniania tablicy wierszy
bge $t1, $s1, koniecTablicyWierszy	#dopóki t1 < s1
sw $t2, ($t0)				#RAM(t0)=index poczatku wiersza
add $t2, $t2, $s4			#t2 += s4
addi $t0, $t0, 4			#t0 += 4
addi $t1, $t1, 1			#t1 += 1
j tablicaWierszy			#powrot na poczatek petli

koniecTablicyWierszy:			#tablica wierszy jest juz uzupelniona

#wypelnienie macierzy wartosciami
#t0 - index poczatku  pierwszego wiersza
#t1 - licznik wierszy
#t2 - licznik kolumn
#t3 - wypelnienie

li $t1, 0				#t1 = 0
lw $t0, ($s0)				#t0 = (s0)

wypelnianieMacierzy:			#petla do wypelniania macierzy
bge $t1, $s1, koniecWypelniania	#dopóki t1<s1
mul $t3, $t1, 100			#t3 = t1 * 100
addi $t3, $t3, 1			#t3 += 1
li $t2, 0				#t2 = 0
wypelnianieWiersza:			#wypelnianie wiersza, petla wewnetrzna
bge $t2, $s2, koniecPetli		#dopóki t2<s2
sw $t3, ($t0)				#(t0) = t3	
addi $t3, $t3, 1			#t3 += 1
addi $t0, $t0, 4			#t0 += 4
addi $t2, $t2, 1			#t2 += 1
				
j wypelnianieWiersza
koniecPetli:
add $t1, $t1, 1				#t1 += 1

j wypelnianieMacierzy
koniecWypelniania:			#macierz jest wypelniona

#zapis / odczyt

#t0 - wybor uzytkownika
#t1 - wybor wiersza
#t2 - wybor kolumny
#t3 - wybor wiersza * 4
#t4 - wybor kolumny * 4
#t5 - index poczatku adresu wiersza wybranego
#t6 - index poczatku wiersza wybranego, potem poczatek wybranej kolumny
#t7 - wartosc do wpisania

zapisOdczyt:
li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text3				#podaje "wybierz co chcesz zrobic"
syscall					#wypisz "wybierz co chcesz zrobic"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $t0, $v0				#zapisuje wybor uzytkownika w t0

beq $t0, 3, koniecProgramu		#jezeli t0 = 3 zakoncz program

li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text4				#podaje "podaj numer wiersza
syscall					#wypisz "podaj numer wiersza"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $t1, $v0				#zapisuje wybor wiersza w t1

li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text5				#podaje "podaj numer kolumny
syscall					#wypisz "podaj numer kolumny"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $t2, $v0				#zapisuje wybor kolumny w t2

sll $t3, $t1, 2				#t3 = wybor wiersza *4
sll $t4, $t2, 2				#t4 - wybor kolumny *4

add $t5, $s0, $t3			#t5 = s0 + s3
lw $t6, ($t5)				#t6 = (t5)
add $t6, $t6, $t4			#t6 += t4

beq $t0, 2, odczyt			#jezeli t0 = 2 - odczyt
#zapis liczby
li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text7				#podaje "podaj liczbe do wpisania"
syscall					#wypisz "podaj liczbe do wpisania"
li $v0, 5				#ustawiam syscall na pobranie liczby
syscall					#pobieram liczbe
move $t7, $v0				#zapisuje pobrana liczbe do wpisania w t7
sw $t7, ($t6)				# (t6) = t7
j zapisOdczyt				#powrot na poczatek

odczyt:					#odczyt liczby
li $v0, 4				#ustawiam syscall na wypisanie
la $a0, text6				#podaje "szukana liczba w macierzy:"
syscall					#wypisz "szukana liczba w macierzy:"
li $v0, 1				#ustawiam syscall na wypisanie liczby
lw $a0, ($t6)				#podaje syscall liczbe spod adresu t6
syscall					#wypisuje liczbe
j zapisOdczyt				#powrot na poczatek

koniecProgramu:				#koniec programu
li $v0, 10				#ustawiam syscall na koniec
syscall					#koncze program





