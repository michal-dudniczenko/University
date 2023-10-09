import java.util.ArrayList;
import java.util.Scanner;

public class Symulacja {
    public static void main(String[] args){
        System.out.println("-----------------------------");
        Scanner input = new Scanner(System.in);

        System.out.print("Podaj liczbę ramek (rozmiar pamięci fizycznej): ");
        int liczbaRamek = Integer.parseInt(input.nextLine());

        System.out.print("Podaj liczbę stron (rozmiar pamięci wirtualnej): ");
        int liczbaStron = Integer.parseInt(input.nextLine());

        System.out.print("Podaj liczbę wygenerowanych procesów: ");
        int liczbaProcesow = Integer.parseInt(input.nextLine());

        System.out.print("Podaj promień lokalności procesu (zasięg rozrzutu dla danego procesu): ");
        int promien = Integer.parseInt(input.nextLine());

        for (int z=0; z<5; z++) {

            ArrayList<Proces> procesy = new ArrayList<>();

            for (int i = 0; i < liczbaProcesow; i++) {
                procesy.add(new Proces(liczbaStron, promien));
            }

            ArrayList<Integer> requests = new ArrayList<>();

            for (Proces p : procesy) {
                for (int odwolanie : p.odwolania) {
                    requests.add(odwolanie);
                }
            }

            System.out.println("\nWygenerowana liczba odwołań do stron: " + requests.size());

            System.out.println("\nLiczba błędów stron dla poszczególnych algorytmów:");
            System.out.println("FIFO: " + Algorytmy.FIFO(liczbaRamek, requests));
            System.out.println("OPT: " + Algorytmy.OPT(liczbaRamek, requests));
            System.out.println("LRU: " + Algorytmy.LRU(liczbaRamek, requests));
            System.out.println("LRU_APR: " + Algorytmy.LRU_APR(liczbaRamek, requests));
            System.out.println("RAND: " + Algorytmy.RAND(liczbaRamek, requests));
        }
    }
}
