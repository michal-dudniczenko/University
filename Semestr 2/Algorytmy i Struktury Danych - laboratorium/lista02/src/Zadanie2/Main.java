package Zadanie2;

import java.util.Scanner;

public class Main {
    public static void main(String[] args){
        Scanner input = new Scanner(System.in);
        System.out.println("Podaj z jakiego zakresu chcesz wygenerować liczby pierwsze: ");
        int zakres=Integer.parseInt(input.nextLine());

        IteratorFiltrujacy<Integer> iter = new IteratorFiltrujacy<Integer>(new IteratorLiczbowy(zakres), new PierwszaPredicate());
        while(iter.hasNext()){
            System.out.println(iter.next());
        }
    }
}
