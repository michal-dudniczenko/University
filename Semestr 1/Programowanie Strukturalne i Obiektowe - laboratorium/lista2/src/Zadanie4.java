public class Zadanie4 {

    public static long silnia(int n){
        long wynik = 1;
        for (int i = 1; i <= n; i++) wynik *= i;
        return wynik;
    }

    public static double potega(double x, int n){
        double wynik = 1;
        while (n > 0) {
            if (n % 2 == 1) wynik *= x;
            x *= x;
            n /= 2;
        }
        return wynik;
        }

    public static double E_x_MD(double x, double n){
        double suma=0;
        for (int i=0; i<=n; i++){
            suma += potega(x, i)/silnia(i);
        }
        return suma;
    }

    public static double Sin_x_MD(double x, double n){
        x %= (2*Math.PI);
        double suma=0;
        for (int i=0; i<=n; i++){
            suma += potega(-1, i) * potega(x, 2*i+1) / silnia(2*i+1);
        }
        return suma;
    }

    public static double Cos_x_MD(double x, double n){
        x %= (2*Math.PI);
        double suma=0;
        for (int i=0; i<=n; i++){
            suma += potega(-1, i) * potega(x, 2*i) / silnia(2*i);
        }
        return suma;
    }

    public static void main(String[] args){
        System.out.println(E_x_MD(15, 27));
        System.out.println(Sin_x_MD(-100, 30));
        System.out.println(Cos_x_MD(-110, 26));
        /* na + prostota i przejrzystosc kodu
        fatalnie niska mozliwosc aproksymacji z uwagi na niski mozliwy zakres
        funkcji silnia
        o wiele gorsza wydajnosc
        */
    }
}

