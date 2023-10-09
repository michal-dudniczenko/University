import java.util.ArrayList;
import java.util.Random;
import java.util.Scanner;

public class Symulacja {
    public static void main(String[] args){
        System.out.println("--------------------------------------------------");

        Scanner input = new Scanner(System.in);
        Random rnd = new Random();

        System.out.println("Podaj liczbę procesorów (N): ");
        int N = Integer.parseInt(input.nextLine());
        System.out.println("Podaj minimalny próg obciążenia (r): ");
        int r = Integer.parseInt(input.nextLine());
        System.out.println("Podaj maksymalny próg obciążenia (p): ");
        int p = Integer.parseInt(input.nextLine());
        System.out.println("Podaj maksymalną liczbę losowań dla strategii 1 (z): ");
        int z = Integer.parseInt(input.nextLine());

        ArrayList<Proces> procesy = new ArrayList<>();

        int czasWykonania, obciazenieProcesora, docelowyProcesor;
        for (int i=0; i<10000; i++){
            czasWykonania = rnd.nextInt(200)+1;
            obciazenieProcesora = rnd.nextInt(20)+1;
            docelowyProcesor = rnd.nextInt(N);
            procesy.add(new Proces(czasWykonania, obciazenieProcesora, docelowyProcesor));
        }

        int[] pom = new int[1000];
        for (int i=0; i<1000; i++){
            pom[i]=rnd.nextInt(11);
        }

        Algorytmy.Strategia1(procesy, N, p, z, pom);
        Algorytmy.Strategia2(procesy, N, p, pom);
        Algorytmy.Strategia3(procesy, N, r, p, pom);
    }
}
