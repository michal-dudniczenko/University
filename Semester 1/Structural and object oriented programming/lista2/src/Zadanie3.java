
public class Zadanie3 {

    public static double E_x_MD(double x, double n){
        double suma=1;
        double skladnik = x;
        for (int i=2; i<=n+1; i++){
            suma += skladnik;
            skladnik *= (x/i);
        }
        return suma;
    }

    public static double Sin_x_MD(double x, double n){
        x %= (2*Math.PI);
        double suma=0;
        double skladnik = x;
        for (int i=1; i<=n+1; i++){
            suma += skladnik;
            skladnik *= (-1)*((x*x)/((2*i)*(2*i+1)));
        }
        return suma;
    }

    public static double Cos_x_MD(double x, double n){
        x %= (2*Math.PI);
        double suma=0;
        double skladnik = 1;
        for (int i=1; i<=n+1; i++){
            suma += skladnik;
            skladnik *= (-1)*((x*x)/((2*i)*(2*i-1)));
        }
        return suma;
    }

    public static void main(String[] args){
        System.out.println(E_x_MD(15, 10000));
        System.out.println(Sin_x_MD(-100, 100));
        System.out.println(Cos_x_MD(-110, 100));
    }

}
