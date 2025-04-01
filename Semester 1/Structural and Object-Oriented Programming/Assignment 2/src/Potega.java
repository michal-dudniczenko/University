public class Potega {

    public static void potega(double x, int n){
        if (n<0) System.out.println("Błędne dane! Wykładnik ma być naturalny.");
        else {
            double wynik = 1;
            while (n > 0) {
                if (n % 2 == 1) wynik *= x;
                x *= x;
                n /= 2;
            }
            System.out.println(wynik);
        }
    }


    public static void main(String[] args){

        potega(-3.41, 7);
    }
}
