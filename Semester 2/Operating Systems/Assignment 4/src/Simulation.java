import java.util.ArrayList;
import java.util.Random;
//import java.util.Collections;
//import java.util.Scanner;


public class Simulation {
    public static void main(String[] args) {
        Random rnd = new Random();

        int liczbaRamek = 50;
        int liczbaStron = 200;
        int liczbaProcesow = 10;
        int liczbaOdwolan = 200;
        int promien = 5;
        int pff_l = 2;
        int pff_h = 4;
        int pff_t = 30;

        /*
        Scanner input = new Scanner(System.in);

        System.out.print("Podaj liczbę ramek (rozmiar pamięci fizycznej): ");
        int liczbaRamek = Integer.parseInt(input.nextLine());


        System.out.print("Podaj liczbę stron (rozmiar pamięci wirtualnej): ");
        int liczbaStron = Integer.parseInt(input.nextLine());


        System.out.print("Podaj liczbę działających procesów: ");
        int liczbaProcesow = Integer.parseInt(input.nextLine());

        System.out.print("Podaj liczbę wygenerowanych odwołań: ");
        int liczbaOdwolan = Integer.parseInt(input.nextLine());


        System.out.print("Podaj promień lokalności procesu (zasięg rozrzutu dla danego procesu): ");
        int promien = Integer.parseInt(input.nextLine());

        System.out.print("Podaj okno pomiaru czasu dla alg. dynamicznych: ");
        int pff_t = Integer.parseInt(input.nextLine());

        System.out.print("Podaj dolny prog l dla alg. ster. czest.: ");
        int pff_l = Integer.parseInt(input.nextLine());

        System.out.print("Podaj gorny prog h dla alg. ster. czest.: ");
        int pff_h = Integer.parseInt(input.nextLine());
         */
        System.out.println("-------------------------------------------------------------");
        System.out.println("Przyjete zalozenia symulacji:\n");
        System.out.println("liczba ramek: "+liczbaRamek);
        System.out.println("liczba stron: "+liczbaStron);
        System.out.println("liczba procesów: "+liczbaProcesow);
        System.out.println("liczba odwołań: "+liczbaOdwolan);
        System.out.println("promień lokalności: "+promien);
        System.out.println("przyjete okno czasu do pomiarow dla alg. dynamicznych: "+pff_t);
        System.out.println("dla alg. ster. czest.: prog l: "+pff_l+" ,prog h: "+pff_h+"\n");

        ArrayList<Request> requests = new ArrayList<>();

        for (int j = 0; j < liczbaOdwolan; j++) {
            int i = rnd.nextInt(liczbaProcesow);
            int odwolanieCentralne = liczbaStron / liczbaProcesow * (i) + liczbaStron / liczbaProcesow / 2;
            int odwolanie = rnd.nextInt(odwolanieCentralne - promien, odwolanieCentralne + promien + 1);
            if (odwolanie < 0) odwolanie = 0;
            else if (odwolanie >= liczbaStron) odwolanie = liczbaStron - 1;
            requests.add(new Request(i, odwolanie));
        }

        Algorithms.przydzialProporcjonalny(requests, liczbaRamek, liczbaProcesow);
        Algorithms.przydzialRowny(requests, liczbaRamek, liczbaProcesow);
        Algorithms.sterowanieCzestoscia(requests, liczbaRamek, liczbaProcesow, pff_l, pff_h, pff_t);
        Algorithms.modelStrefowy(requests, liczbaRamek, liczbaProcesow, pff_t);
    }
}
