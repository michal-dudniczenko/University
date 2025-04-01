import java.util.Random;

public class Tablica1D {

    private int dlugosc;
    private int[] tablica;
    private int zakres=10;

    public Tablica1D(int dlugosc) {
        this.dlugosc = dlugosc;
        tablica = new int[dlugosc];
    }
    public void wypelnij() {
        Random generator = new Random();
        for (int i = 0; i < dlugosc; i++) {
            tablica[i] = generator.nextInt(zakres+1);
        }
    }
    public void wyswietl(int[] tablica){
        System.out.println("zawartość tablicy:");
        for (int i : tablica) System.out.print(i+" ");
        System.out.println("\n");
    }
    public void wyswietl() {
        System.out.println("zawartość tablicy:");
        for (int i : tablica) System.out.print(i + " ");
        System.out.println("\n");
    }
    public void wyswietl_od_tylu(){
        System.out.println("zawartość tablicy odwrotnie:");
        for (int i=dlugosc-1; i>=0; i--) System.out.print(tablica[i]+" ");
        System.out.println("\n");
    }
    public int maksimum(){
        int max = 0;
        for (int i : tablica){
            if (i>max)max = i;
        }
        return max;
    }
    public int minimum(){
        int min = zakres;
        for (int i : tablica){
            if (i<min)min = i;
        }
        return min;
    }

    public int[] parzyste(){
        int licznik=0;
        for (int i : tablica)if (i % 2 == 0)licznik++;
        int[] tablica_parzyste = new int[licznik];
        int pom=0;
        for (int i : tablica) {
            if (i % 2 == 0) {
                tablica_parzyste[pom] = i;
                pom++;
            }
        }
        return tablica_parzyste;
    }
    public int[] nieparzyste(){
        int licznik=0;
        for (int i : tablica)if (i % 2 == 1)licznik++;
        int[] tablica_nieparzyste = new int[licznik];
        int pom=0;
        for (int i : tablica) {
            if (i % 2 == 1) {
                tablica_nieparzyste[pom] = i;
                pom++;
            }
        }
        return tablica_nieparzyste;
    }
}

