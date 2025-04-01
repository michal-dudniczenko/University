.data 

zakres: .word 100				#zakres liczb pierwszych
nprimes: .word 0				#licznik pierwszych
numall: .space 396				#tablica liczb 2-zakres
primes: .space 396				#tablica liczb pierwszych
newline: .asciiz "\n"

	
.text

lw $s0, zakres					#zakres w s0
lw $s1, zakres					#polowa zakresu w s1
srl $s1, $s1, 1

li $t0 2					#t0 - temp, zaczynam od  2, bede wypelnial az do zakresu
li $t1 0					#znacznik pomocniczny do wstawiania na pozycje w tabli

wypelnienieNumall:				#petla do wypelnienia numall
bgt $t0, $s0, koniecPetli1			#wyjscie z petli
sw $t0, numall($t1)				#wpisuje do tablicy temp
addi $t0, $t0, 1				#zwiekszenie temp o 1
addi $t1, $t1, 4				#przejscie na kolejny indeks tablicy
j wypelnienieNumall				#powrot na poczatek petli

koniecPetli1:					#numall jest wypelniona 2-zakres

li $t0, 0					#t0 - temp
li $t1, 0					#znacznik pomocniczy - indeks w numall	
li $t2, 0					#obecny dzielnik
li $t3, 0					#obecna wielokrotnosc

usuwanieLiczb:					#petla do usuwania wielokrotnosci w numall
bge $t0, $s1, koniecPetli2			#jak dojdziemy do polowy zakresu, wyjscie
lw $t2, numall($t1)				#pobieram liczbe z numall jako dzielnik
beq $t2, 0, dzielnikZero			#jezeli dzielnik=0 to przejdz na koniec petli
add $t3, $t2, $t2				#wielokrotnosc do dzielnik*2

usuwanieWielokrotnosci:				#wewnetrzna petla do usuwania wielokrotnosci
bgt $t3, $s0, dzielnikZero			#jezeli wielokrotnosc jest wieksza od zakres, wyjdz z petli
sub $t4, $t3, 2					#t4-indeks w tabeli(dzielnik-2)
mul $t4, $t4, 4					#pozycja w tablicy indeksu
sw $zero, numall($t4)				#zeruje wielokrotnosc w numall
add $t3, $t3, $t2				#zwiekszam wielokrotnosc o dzielnik
j usuwanieWielokrotnosci			#powrot na poczatek petli

dzielnikZero:					#koniec petli wewnetrznej, lub dzielnik=0
addi $t0, $t0, 1				#zwiekszam temp o 1
addi $t1, $t1, 4				#zwiekszam indeks o 4
j usuwanieLiczb					#usuwanie kolejnej liczby

koniecPetli2:					#w numall sa same pierwsze i zera

li $t0, 0					#temp - iterator
li $t1, 0					#4*temp - pozycja w tablicy numall
li $t2, 0					#licznik nprimes
subi $t4, $s0, 1				#pomocnicza - zakres-1
li $t6, 0					#pozycja w tablicy primes

wypelnianieZliczanie:				#petla wypelniajaca primes i zliczajaca
bge $t0, $t4, koniecPetli3			#wyjscie z petli jak temp=zakres-1
lw $t5, numall($t1)				#pomocnicze t5 - liczba w numall
beq $t5, $zero, liczbaZero			#jezeli liczba w numall=0 to pomin dodawanie
addi $t2, $t2, 1				#zwieksz licznik o 1
sw $t5, primes($t6)				#przepisz liczbe z numall do primes
addi $t6, $t6, 4				#zwieksz znacznik tablicy primes

liczbaZero:					#liczba=0
addi $t0, $t0, 1				#zwieksz temp o 1
addi $t1, $t1, 4				#zwieksz znacznik tablicy o 4

j wypelnianieZliczanie				#powrot na poczatek petli

koniecPetli3:					#koniec zliczania

sw $t2, nprimes					#zapisuje liczbe pierwszych do nprimes


li $t0, 0					#adres liczby w primes

wypisywanie:					#petla do wypisywania
lw $t1, primes($t0)				#laduje liczbe z primes do t1
beq $t1, $zero, koniec				#jezeli trafilismy na zero to koniec
li $v0, 1					#ustawiam syscall na wypisanie inta
la $a0, ($t1)					#podaje syscall liczbe do wypisania
syscall						#wypisuje liczbe
li $v0, 4					#ustawiam syscall na wypisanie napisu
la $a0, newline					#podaje syscall nowa linie do wypisania
syscall						#wypisuje nowa linie
addi $t0, $t0, 4				#przechodze na nastepna liczbe w primes
j wypisywanie 					#wypisuje kolejna liczbe



koniec:						#koniec programu

li $v0, 10					#ustawiam syscall na koniec
syscall						#koncze program

#dla n=25 jest około 1000 instrukcji
#dla n=50 jest około 2000 instrukcji
#dla n=100 jest około 4000 instrukcji
#zatem wyglada na to ze liczba instrukcji zachowuje sie liniowo i wynosi 40*n
#szacuje ze dla n = 100 000 bedzie 4 000 000 instrukcji
#program dla n = 100 000 wykonal sie w okolo 8s co daje predkosc okolo 500 000 operacji na sekunde

#czasy wykonania java:
#n = 1 000 000 : 	java: 0,02s		asm: ok. 80s
#n = 10 000 000 : 	java: 0,25s		asm: ok. 13 min
#n = 100 000 000 : 	java: 2,58s		asm: ok. 2h




	
	
