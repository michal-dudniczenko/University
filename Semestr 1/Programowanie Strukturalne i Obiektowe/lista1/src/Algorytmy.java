public class Algorytmy {
    public static void NWD(int M, int N){
        if (M<0) M*=-1;
        if (N<0) N*=-1;

        if (M!=0 && N!=0){
            while (M!=N){
                if (M>N) M-=N;
                else N-=M;
            }
            System.out.print("NWD liczb:\n"+M);
        }
        else System.out.print("Błędne dane!");
    }

    public static void pierwiastki(double a, double b, double c){
        if (a==0){
            if (b==0){
                if (c==0) System.out.print("Tożsamość");
                else System.out.print("Sprzeczne");
            }
            else System.out.print("Pierwiastek równania:\n"+(-c/b));
        }
        else{
            double delta = b*b - 4*a*c;
            if (delta < 0) System.out.print("Brak rozwiązań");
            else if (delta==0) System.out.print(-b/(2*a));
            else{
                double x1=(-b + Math.sqrt(delta))/(2*a);
                double x2=(-b - Math.sqrt(delta))/(2*a);
                System.out.print("Pierwiastki równania:\n"+x1+"\n"+x2);
            }
        }
    }

    public static void sortowanie(double A, double B, double C){
        if (A>B){
            double pom = A;
            A = B;
            B = pom;
        }
        if (B>C){
            double pom = B;
            B = C;
            C = pom;
        }
        if (A>B){
            double pom = A;
            A = B;
            B = pom;
        }
        System.out.print("Niemalejący ciąg:\n"+A+" "+B+" "+C);
    }
}
