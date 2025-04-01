.eqv STACK_SIZE 2048

.data
sys_stack_addr: .word 0
stack: .space STACK_SIZE
global_array: .word 1, 2, 3, 4, 5, 6, 7, 8, 9, 10

.text
sw $sp, sys_stack_addr		#sys_stack_addr - adres systemowego stosu
la $sp, stack+STACK_SIZE	#$sp ustawiony na poczatek stosu, bedzie sie rozrastal w lewo

jal main			#wywoluje main()

lw $sp, sys_stack_addr		#zapisuje w sp poczatkowy adres systemowego stosu

li $v0, 10			#ustawiam syscall na koniec
syscall				#koncze program

#===========================================================================================

sum:
subi $sp, $sp, 8		#przesuwam wierzcholek stosu o 2 miejsca(wartosc zwracana i adres powrotu)
sw $ra, ($sp)			#na wierzcholku stosu adres powrotu
subi $sp, $sp, 8		#miejsce na stosie na: int i, int s
li $t1, 0			#s = 0
sw $t1, 4($sp)			#odkladam s na stos
lw $t1, 20($sp)			#i = 10 (sciagam argument ze stosu)
subi $t1, $t1, 1		#i = array_size - 1
sw $t1, ($sp)			#odkladam i na stos

petla:
lw $t1, ($sp)			#t1 - i (sciagam ze stosu)
lw $t2, 4($sp)			#t2 - s	(sciagam ze stosu)
blt $t1, $zero, koniecPetli	#wykonuj dopoki i>=0
lw $t3, 16($sp)			#t3 - *global_array
sll $t4, $t1, 2			#t4 - i * 4 (przesuniecie w global_array)
add $t3, $t3, $t4		#t3 - adres i-tego elementu w global_array
lw $t3, ($t3)			#t3 - i-ty element z global_array
add $t2, $t2, $t3		#t2 = s + array[i]
subi $t1, $t1, 1		#t3 = i - 1
sw $t1, ($sp)			#odkladam nowe i na stos
sw $t2, 4($sp)			#odkladam nowe s na stos
j petla

koniecPetli:
lw $t1, 4($sp)			#t1 - s (sciagam ze stosu)
sw $t1, 12($sp)			#zapisuje na stosie ( w miejscu poprzednio zarezerwowanym do tego celu) wartość zwracaną
addi $sp, $sp, 8		# przesuwa wskaźnik stosu tak aby usunąć ze stosu zmienne lokalne
lw $ra, ($sp)			#pobiera ze stosu adres powrotu i umieszcza go w rejestrze $ra
addi $sp, $sp, 4		#przesuwa wskaźnik stosu tak aby na wierzchołku stosu znalazła się wartość zwracana
jr $ra				#powrot z podprogramu sum()



main:
subi $sp, $sp 4			#robie miejsce na stosie na odlozenie powrotu z maina
sw $ra, ($sp)			#odkladam na stos adres powrotu z main()
subi $sp, $sp 4			#robie miejsce na stosie na odlozenie int s z maina

subi $sp, $sp, 8		#robie miejsce na stosie na 2 argumenty sum()
la $t1, global_array		#t1 = adres global_array
sw $t1, ($sp)			#odkladam pierwszy argument (*global_array) na stos
li $t1, 10			#t1 = 10
sw $t1, 4($sp)			#odkladam drugi argument (10) na stos
jal sum				#wywoluje funkcje sum()
lw $t1, ($sp)			#pobiera ze stosu wartość zwróconą przez wywołany podprogram
addi $sp, $sp, 12		#przesuwa wskaźnik stosu tak aby usunąć z niego wartość zwracaną oraz argumenty podprogramu położone na stos przed jego wywołaniem

sw $t1, ($sp)			#s = sum( global_array, 10 );

li $v0, 1			#ustawiam syscall na wypisanie inta
lw $a0, ($sp)			#podaje syscall wartosc s do wypisania
syscall				#wypisuje s

addi $sp, $sp, 4		#przesuwa wskaźnik stosu tak aby usunąć ze stosu zmienne lokalne
lw $ra, ($sp)			#pobiera ze stosu adres powrotu i umieszcza go w rejestrze $ra
addi $sp, $sp, 4		#przesuwa wskaźnik stosu tak aby na wierzchołku stosu znalazła się wartość zwracana
jr $ra				#powrot z podprogramu main()