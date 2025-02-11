import java.util.Random;
import java.util.ArrayList;
import java.util.Comparator;

public class Symulacja {
    public static void main(String[] args) {
        Random generator = new Random();
        int rozmiarDysku = 1024;
        int liczbaProcesow = 100;
        int liczbaRealtime = 10;
        int czasSymulacji = 10000;

        ArrayList<Proces> procesy = new ArrayList<>();

        double[] wyniki = new double[8];



        for (int j = 0; j < 100; j++) {
            procesy.clear();
            int czasZgloszenia, blok, deadline;
            Proces proces;
            for (int i = 0; i < liczbaProcesow; i++) {
                czasZgloszenia = generator.nextInt(czasSymulacji) + 1;
                blok = generator.nextInt(rozmiarDysku) + 1;
                proces = new Proces(i + 1, blok, czasZgloszenia);
                procesy.add(proces);
            }
            for (int i = 0; i < liczbaRealtime; i++) {
                czasZgloszenia = generator.nextInt(czasSymulacji) + 1;
                blok = generator.nextInt(rozmiarDysku) + 1;
                proces = new Proces(i + 1, blok, czasZgloszenia);
                proces.setRealtime(true);
                deadline = generator.nextInt(czasZgloszenia + 512, czasZgloszenia + 1024);
                proces.setDeadline(deadline);
                procesy.add(proces);
            }
            procesy.sort(new Comparator<>() {
                public int compare(Proces p1, Proces p2) {
                    return p1.getCzasZgloszenia() - p2.getCzasZgloszenia();
                }
            });

            Algorytmy.FCFS(procesy, wyniki);
            Algorytmy.SSTF(procesy, wyniki);
            Algorytmy.SCAN(procesy, wyniki);
            Algorytmy.CSCAN(procesy, wyniki);
            Algorytmy.SSTF_EDF(procesy, wyniki);
            Algorytmy.SSTF_FDSCAN(procesy, wyniki);
        }
        for (int i=0; i<wyniki.length; i++){
            wyniki[i]/=100;
        }


        System.out.println("------------------");
        System.out.println("Statystyki:");
        System.out.println("średnie ruchy głowicy ze 100 symulacji:\n");
        System.out.println("FCFS: "+wyniki[0]);
        System.out.println("SSTF: "+wyniki[1]);
        System.out.println("SCAN: "+wyniki[2]);
        System.out.println("C-SCAN: "+wyniki[3]);
        System.out.println("\nSSTF + EDF: "+wyniki[4]);
        System.out.println("Ukończone procesy realtime: "+Math.round(wyniki[5]*100)+"%");
        System.out.println("\nSSTF + FD-SCAN: "+wyniki[6]);
        System.out.println("Ukończone procesy realtime: "+Math.round(wyniki[7]*100)+"%");

        System.out.println("\n---------------------------");
    }
}
