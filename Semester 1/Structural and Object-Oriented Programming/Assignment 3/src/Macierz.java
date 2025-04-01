import java.util.Random;

public class Macierz {
    private double[][] macierz;

    public Macierz(int M){
        int zakres = 5;
        Random pom = new Random();
        this.macierz = new double[M][M];
        for (int i=0; i<M; i++){
            for (int j=0; j<M; j++){
                macierz[i][j] = pom.nextDouble(zakres+1);
            }
        }
    }
     public void wypisz(){
        for (double[] i : macierz){
            for (double j : i) System.out.print(j + " ");
            System.out.println();
        }
        System.out.println();
     }

    public void wypisz(Macierz m1){
        for (double[] i : m1.macierz){
            for (double j : i) System.out.print(j + " ");
            System.out.println();
        }
        System.out.println();
    }

     public Macierz suma(Macierz m2){
        int rozmiar = this.macierz.length;
        Macierz wynik = new Macierz(rozmiar);
        for (int i=0; i<rozmiar; i++){
            for (int j=0; j<rozmiar; j++){
                wynik.macierz[i][j] = this.macierz[i][j] + m2.macierz[i][j];
             }
         }
        return wynik;
     }

     public Macierz iloczyn(Macierz m2){
         int rozmiar = this.macierz.length;
         Macierz wynik = new Macierz(rozmiar);
         double suma;
         for (int i=0; i<rozmiar; i++){
             for (int j=0; j<rozmiar; j++){
                 suma=0;
                 for (int k=0; k<rozmiar; k++){
                     suma += this.macierz[i][k] * m2.macierz[k][j];
                 }
                 wynik.macierz[i][j] = suma;
             }
         }
         return wynik;
     }
}
