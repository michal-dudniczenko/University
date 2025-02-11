import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Muzycy[] postacie = new Muzycy[10];
        Wiolonczelistka p1 = new Wiolonczelistka("Nadia", 19);
        Wiolonczelistka p2 = new Wiolonczelistka("Ania", 25);
        Gitarzysta p3 = new Gitarzysta("Michał", 21);
        Gitarzysta p4 = new Gitarzysta("Janusz", 35);

        postacie[0] = p1;
        postacie[1] = p2;
        postacie[2] = p3;
        postacie[3] = p4;

        Scanner input = new Scanner(System.in);
        System.out.println("\n\n\n\n\n\n\n\n\n\n/////////////////////   START   /////////////////////");
        System.out.println("\nWybierz, którą postacią chcesz grać:\n");
        int i=0;
        while (postacie[i]!=null){
            System.out.print("Postac "+(i+1)+":     ");
            postacie[i].getStanShort();
            System.out.println("\n");
            i++;
        }
        String pom = input.nextLine();
        Muzycy postac = postacie[Integer.parseInt(pom)-1];

        System.out.println("\n\n\n\n\n\n");
        System.out.println("                             Celem gry jest zarobienie miliona. Powodzenia!");

        String wybor="";

        while (postac.getPortfel() < 1000000 && !wybor.equals("6")) {
            System.out.println("--------------------------------------------------------------------------------------------------------");
            System.out.println("Twoja postać:");
            postac.get_stan();
            System.out.println("\nTwój instrument:");
            postac.getInstrument();
            System.out.println("--------------------------------------------------------------------------------------------------------\n");
            System.out.println("                                    --- Co chcesz teraz zrobić? ---");
            System.out.println("(1-daj koncert, 2-idź na lekcję, 3-ćwicz grę, 4-ulepsz instrument, 5-napraw instrument, 6-koniec)\n");
            System.out.println("                           --- Wprowadź opcję i zatwierdź enterem  ---");
            wybor = input.nextLine();
            System.out.println("\n\n\n");
            switch (wybor) {
                case "1" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    postac.daj_koncert();
                }
                case "2" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    postac.idz_na_lekcje();
                }
                case "3" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    postac.cwicz();
                }
                case "4" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    postac.ulepsz_instrument();
                }
                case "5" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    postac.napraw_instrument();
                }
                case "6" -> {
                    System.out.println("\n\n\n\n\n\n\n\n\n");
                    System.out.println("\n\n\n\n\n\n\n\n\n/////////////////////   KONIEC   /////////////////////");
                }
                default -> System.out.println("\n\n\n\n\n\n\n\n\nNieprawidłowy wybór :(");
            }
        }
        if(!wybor.equals("6")){
            System.out.println("Koniec gry! Zostałaś milionerką whaaaat, dobra robota");
        }
    }
}
