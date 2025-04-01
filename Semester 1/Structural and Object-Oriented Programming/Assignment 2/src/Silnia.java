public class Silnia {

    public static void main(String[] args) {
        int n=7;
        long wynik = 1;
        if (n<0) {System.out.println("błędne dane, silnia może być " +
                "obliczona tylko z wartości dodatnich!");}
        else {
            for (int i = 1; i <= n; i++) wynik *= i;
            System.out.println(wynik);
        }
    }
}