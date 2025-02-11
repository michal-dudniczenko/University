import java.util.Scanner;
import java.io.*;
import java.util.ArrayList;

public class Main {
    public static void main(String[] args){
        Uczelnia pwr = new Uczelnia();
        File osobySer = new File("osoby.txt");
        File kursySer = new File("kursy.txt");
        try{
            if (osobySer.length()!=0) {
                ObjectInputStream ois = new ObjectInputStream(new FileInputStream(osobySer));
                pwr.setOsoby((ArrayList) ois.readObject());
                ois.close();
            }
            if (kursySer.length()!=0) {
                ObjectInputStream ois = new ObjectInputStream(new FileInputStream(kursySer));
                pwr.setKursy((ArrayList) ois.readObject());
                ois.close();
            }
        } catch (IOException | ClassNotFoundException exc) {
            exc.printStackTrace();
        }


        Scanner input = new Scanner(System.in);
        int wybor= -1;

        while (wybor != 0){
            System.out.println("\nWybierz co chcesz teraz zrobić:");
            System.out.println("1 - Dodaj osobę do bazy danych");
            System.out.println("2 - Dodaj kurs do bazy danych");
            System.out.println("3 - Wyszukaj pracownika");
            System.out.println("4 - Wyszukaj studenta");
            System.out.println("5 - Wyszukaj kurs");
            System.out.println("6 - Wyświetl wszystkich pracowników");
            System.out.println("7 - Wyświetl wszystkich studentów");
            System.out.println("8 - Wyświetl wszystkie kursy");
            System.out.println("9 - Sortuj listę osób");
            System.out.println("10 - Sortuj listę kursów");
            System.out.println("11 - Usuń pracowników");
            System.out.println("12 - Usuń studentów");
            System.out.println("13 - Usuń kursy");
            System.out.println("14 - Wyczyść listę studentów");
            System.out.println("15 - Dodaj ogłoszenie");
            System.out.println("0 - Zakończ i zapisz");
            wybor = Integer.parseInt(input.nextLine());
            switch(wybor){
                case 1:{
                    pwr.dodajOsobe();
                    break;
                }
                case 2:{
                    pwr.dodajKurs();
                    break;
                }
                case 3:{
                    pwr.wyszukajPracownika();
                    break;
                }
                case 4:{
                    pwr.wyszukajStudenta();
                    break;
                }
                case 5:{
                    pwr.wyszukajKurs();
                    break;
                }
                case 6:{
                    pwr.wyswietlPracownikow();
                    break;
                }
                case 7:{
                    pwr.wyswietlStudentow();
                    break;
                }
                case 8:{
                    pwr.wyswietlKursy();
                    break;
                }
                case 9:{
                    pwr.sortujOsoby();
                    break;
                }
                case 10:{
                    pwr.sortujKursy();
                    break;
                }
                case 11:{
                    pwr.usunPracownikow();
                    break;
                }
                case 12:{
                    pwr.usunStudentow();
                    break;
                }
                case 13:{
                    pwr.usunKursy();
                    break;
                }
                case 14:{
                    pwr.czyszczenie();
                    break;
                }
                case 15:{
                    pwr.ogloszenie();
                    break;
                }
                case 0:{
                    try{
                        ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream("osoby.txt"));
                        oos.writeObject(pwr.getOsoby());
                        oos = new ObjectOutputStream(new FileOutputStream("kursy.txt"));
                        oos.writeObject(pwr.getKursy());
                        oos.close();
                    }catch(IOException ioe){
                        ioe.printStackTrace();
                    }
                    break;
                }
                default:{
                    System.out.println("Nieprawidłowy wybór!");
                }
            }
        }
    }
}
