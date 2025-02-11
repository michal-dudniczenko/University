.data

coefs: .float 2.3 3.45 7.67 5.32
degree: .word 3
text0: .asciiz "\n\n1 - policz wartosc wielomianu\n0 - zakoncz\n"
text1: .asciiz "\nPodaj wartosc x: "
text2: .asciiz "\nWartosc wielomianu = "

.text

petla:
li $v0, 4					#wypisanie komunikatu o wyborze akcji
la $a0, text0
syscall

li $v0, 5					#pobranie wyboru akcji
syscall

beq $v0, $zero, koniec				#jezeli wybrano 0 to koniec

li $v0, 4					#wypisanie komunikatu podaj liczbe
la $a0, text1
syscall

li $v0, 7					#pobranie liczby
syscall 

la $a0, coefs					#a0 - adres poczatku wspolczynnikow
lw $a1, degree					#a1 - stopien
mov.d $f12, $f0					#f12 - x

l.s $f0, ($a0)					#f0(wynik) = wspolczynniki[0]
cvt.d.s $f0, $f0

petla2:
beqz $a1, koniecPetli2				#dopóki stopien>0

mul.d $f0, $f0, $f12				#wynik *= x
addi $a0, $a0, 4				#i++
l.s $f4, ($a0)					#f4 = wspolczynniki[i]
cvt.d.s $f4, $f4
add.d $f0, $f0, $f4				#wynik+=wspolczynniki[i]

addi $a1, $a1, -1				#stopien--
j petla2

koniecPetli2:
li $v0, 4					
la $a0, text2					#wypisanie wyniku
syscall
li $v0, 3					
mov.d $f12, $f0
syscall
j petla


koniec:
li $v0, 10					#zakonczenie programu
syscall

