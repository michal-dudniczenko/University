import java.util.Scanner;

public class Zadanie2 {
    public static boolean czyPierwsza(int n){
        if (n==2)return true;
        if (n<2 || n%2==0)return false;
        for (int i=3; i<=Math.sqrt(n);i+=2){
            if(n % i == 0)return false;
        }
        return true;
    }

    public static void main(String[] args){
        System.out.println("--------------");
        System.out.println("Zadanie 2:");
        Scanner input = new Scanner(System.in);
        int poczatek, koniec;
        System.out.println("Podaj początek zakresu z którego chcesz wygenerować liczby pierwsze: ");
        poczatek=Integer.parseInt(input.nextLine());
        System.out.println("Podaj koniec zakresu z którego chcesz wygenerować liczby pierwsze: ");
        koniec=Integer.parseInt(input.nextLine());
        System.out.println("Liczby pierwsze z przedziału: "+"["+poczatek+", "+koniec+"]");
        IteratorFiltrujacy<Integer> iter = new IteratorFiltrujacy<>(new IteratorLiczbowy(poczatek, koniec), n -> czyPierwsza(n));
        while(iter.hasNext()){
            System.out.println(iter.next());
        }
    }
}
