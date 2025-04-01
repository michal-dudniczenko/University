.data
	komunikat1: .asciiz "Podaj pierwsza liczbe: "
	komunikat2: .asciiz "Podaj druga liczbe: "
	komunikat3: .asciiz "\nWynik mnozenia: "
	komunikat4: .asciiz "\nWystapilo przepelnienie! Wyniku nie da sie zapisac na 32 bitach.\n"
	
	licz1:	.word 0
	licz2: .word 0
	wyn: .word 0
	status: .byte 0
	
.text
	li $v0, 4			#ustawiam syscall na wypisanie
	la $a0, komunikat1		#podaje syscall	 tekst do wypisania
	syscall				#wypisuje komunikat
	
	li $v0, 5			#ustawiam syscall na pobranie liczby
	syscall				#pobieram liczbe1
	
	move $t1, $v0			#zapisuje odczytana wartosc w $t1
	
	li $v0, 4			#ustawiam syscall na wypisanie
	la $a0, komunikat2		#podaje syscall tekst do wypisania
	syscall				#wypisuje komunikat
	
	li $v0, 5			#ustawiam syscall na pobranie liczby
	syscall				#pobieram liczbe2
	
	move $t2, $v0			#zapisuje odczytana wartosc w $t2
	
	sw $t1, licz1			#pobrana liczbe1 zapisuje do pola licz1
	sw $t2, licz2			#pobrana liczbe2 zapisuje do pola licz2
	
	
#wlasciwy program sie zaczyna, legenda rejestrow:
#t0 - wynik
#t1 - liczba1 (mnozna)
#t2 - liczba2 (mnoznik)
#t4 - flaga przepe	lnienia(jezeli jest 1 to znaczy ze bylo przepelnienie) jednoczesnie bedzie przepisana do pola status
#t5 - obecne przesuniecie w lewo(tak jak w mnozeniu pisemnym)
#t6 - wartosc temp, tu bedzie mnozna odpowiednio przesunieta
#t7 - ostatnia cyfra mnoznika
#wartosc do testowania przepelnienia 2147483647


	beq $t1, $zero, licz2Eq0	#jezeli licz1 jest rowna 0 to nie wykonuj

petla1:
	beq $t2, $zero, licz2Eq0	#jezeli licz2 jest rowna 0 to wyjdz z petli
	and $t7, $t2, 1			#w t7 jest zapisana ostatnia cyfra licz2
	beq $t7, $zero, koniecPetli1	#jezeli ostatnia cyfra mnoznika to 0 to nic nie rob i zakoncz petle
	move $t6, $t1			#w temp zapisz mnozna
	sllv $t6, $t6, $t5		#przesun mnozna w lewo o wartosc przesuniecia
	bltz $t6, przepelnienie		#wykryj przepelnienie i przejdz na koniec
	addu $t0, $t0, $t6		#dodaj do wyniku przesunieta mnozna
	bltz $t0, przepelnienie		#wykryj przepelnienie i przejdz na koniec
	
	
koniecPetli1:
	add $t5, $t5, 1			#zwieksz obecne przesuniecie o 1
	srl $t2, $t2, 1			#przesun mnoznik w prawo o 1(dzielenie przez 2)
	j petla1			#wroc na poczatek petli
	
	
licz2Eq0:				#mnoznik jest juz rowny 0, koniec dodawania
	li $v0, 4			#ustawiam syscall na wypisywanie
	la $a0, komunikat3		#podaje syscall tekst do wypisania o wyniku
	syscall				#wypisuje komunikat
	li $v0, 1			#ustawiam syscall na wypisanie liczby(wyniku)
	la $a0, ($t0)			#podaje syscall wynik do wypisania
	syscall				#wypisuje wynik
	j koniec			#przejdz do konca programu
	

przepelnienie:				#blok jezeli wystapilo przepelnienie
	add $t4, $t4, 1			#ustaw rejestr $t4 na wartosc 1 czyli flaga o przepelnieniu
	sw $t4, status			#flage o przepelnieniu zapisz do pola status
	li $v0, 4			#ustaw syscall na wypisywanie
	la $a0, komunikat4		#podaje syscall komunikat o przepelnieniu
	syscall				#wypisuje komunikat o przepelnieniu	


koniec:					#blok zakonczenia programu
	li $v0, 10			#ustawiam syscall na zakonczenie
	syscall				#koncze program

