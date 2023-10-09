import java.util.Random;
import java.util.ArrayList;

public class Symulacja {
    public static void main(String[] args){

        int kwant=35;
        int czasSymulacji=10000;

        double[] wynikiFCFS = new double[7];
        double[] wynikiSJFbez = new double[7];
        double[] wynikiSJFz = new double[7];
        double[] wynikiRR = new double[7];

        for (int j=0; j<20; j++) {
            ArrayList<Proces> procesy = new ArrayList<>();
            int dlugoscFazy, czasZgloszenia;
            Random generator = new Random();
            for (int i = 0; i < 1000; i++) {
                dlugoscFazy = generator.nextInt(100) + 1;
                czasZgloszenia = generator.nextInt(czasSymulacji);
                procesy.add(new Proces(i + 1, dlugoscFazy, czasZgloszenia));
            }
            Algorytmy.FCFS(procesy, wynikiFCFS, czasSymulacji);
            Algorytmy.SJFbez(procesy, wynikiSJFbez, czasSymulacji);
            Algorytmy.SJFz(procesy, wynikiSJFz, czasSymulacji);
            Algorytmy.RR(procesy, wynikiRR, czasSymulacji, kwant);
        }

        for (int i=0; i<7; i++){
            wynikiFCFS[i]/=20;
        }
        for (int i=0; i<7; i++){
            wynikiSJFbez[i]/=20;
        }
        for (int i=0; i<7; i++){
            wynikiSJFz[i]/=20;
        }
        for (int i=0; i<7; i++){
            wynikiRR[i]/=20;
        }

        System.out.println("----------- Statystyki -----------");
        System.out.println("20 ciągów po 1000 procesów w czasie 10000ms ");

        System.out.println("FCFS:");
        System.out.println("średni czas oczekiwania: "+Math.round(wynikiFCFS[0]*100.0)/100.0);
        System.out.println("najdłuższy czas oczekiwania procesu: "+wynikiFCFS[1]);
        System.out.println("średni czas od rozpoczęcia procesu do zakończenia: "+Math.round(wynikiFCFS[2]*100.0)/100.0);
        System.out.println("liczba przełączeń: "+wynikiFCFS[3]);
        System.out.println("liczba procesów, które utknęły na ponad 8000ms i nie zostały wykonane(zagłodzone?): "+wynikiFCFS[4]);
        System.out.println("liczba procesów rozpoczętych, ale nie ukończonych: "+wynikiFCFS[5]);
        System.out.println("procent ukończonych: "+Math.round(wynikiFCFS[6]*100.0)/100.0+"%");
        System.out.println("--------------------------------");

        System.out.println("SJF bez wywłaszczenia:");
        System.out.println("średni czas oczekiwania: "+Math.round(wynikiSJFbez[0]*100.0)/100.0);
        System.out.println("najdłuższy czas oczekiwania procesu: "+wynikiSJFbez[1]);
        System.out.println("średni czas od rozpoczęcia procesu do zakończenia: "+Math.round(wynikiSJFbez[2]*100.0)/100.0);
        System.out.println("liczba przełączeń: "+wynikiSJFbez[3]);
        System.out.println("liczba procesów, które utknęły na ponad 8000ms i nie zostały wykonane(zagłodzone?): "+wynikiSJFbez[4]);
        System.out.println("liczba procesów rozpoczętych, ale nie ukończonych: "+wynikiSJFbez[5]);
        System.out.println("procent ukończonych: "+Math.round(wynikiSJFbez[6]*100.0)/100.0+"%");
        System.out.println("--------------------------------");

        System.out.println("SJF z wywłaszczeniem:");
        System.out.println("średni czas oczekiwania: "+Math.round(wynikiSJFz[0]*100.0)/100.0);
        System.out.println("najdłuższy czas oczekiwania procesu: "+wynikiSJFz[1]);
        System.out.println("średni czas od rozpoczęcia procesu do zakończenia: "+Math.round(wynikiSJFz[2]*100.0)/100.0);
        System.out.println("liczba przełączeń: "+wynikiSJFz[3]);
        System.out.println("liczba procesów, które utknęły na ponad 8000ms i nie zostały wykonane(zagłodzone?): "+wynikiSJFz[4]);
        System.out.println("liczba procesów rozpoczętych, ale nie ukończonych: "+wynikiSJFz[5]);
        System.out.println("procent ukończonych: "+Math.round(wynikiSJFz[6]*100.0)/100.0+"%");
        System.out.println("--------------------------------");

        System.out.println("RR dla wybranego kwantu czasu ("+kwant+") :");
        System.out.println("średni czas oczekiwania: "+Math.round(wynikiRR[0]*100.0)/100.0);
        System.out.println("najdłuższy czas oczekiwania procesu: "+wynikiRR[1]);
        System.out.println("średni czas od rozpoczęcia procesu do zakończenia: "+Math.round(wynikiRR[2]*100.0)/100.0);
        System.out.println("liczba przełączeń: "+wynikiRR[3]);
        System.out.println("liczba procesów, które utknęły na ponad 8000ms i nie zostały wykonane(zagłodzone?): "+wynikiRR[4]);
        System.out.println("liczba procesów rozpoczętych, ale nie ukończonych: "+wynikiRR[5]);
        System.out.println("procent ukończonych: "+Math.round(wynikiRR[6]*100.0)/100.0+"%");
        System.out.println("--------------------------------");
    }
}
