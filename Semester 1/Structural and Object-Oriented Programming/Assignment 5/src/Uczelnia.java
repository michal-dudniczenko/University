import java.util.ArrayList;

public class Uczelnia {
    private ArrayList<Osoba> osoby;
    private ArrayList<Kursy> kursy;

    public Uczelnia(){
        this.osoby = new ArrayList<Osoba>();
        this.kursy = new ArrayList<Kursy>();
    }

    public void dodajOsobe(Osoba o){
        osoby.add(o);
    }
    public void dodajKurs(Kursy k){
        kursy.add(k);
    }

    public void wyszukajPracownika(String filtr, String fraza){
        System.out.println("Wszyscy pracownicy tacy, że: "+filtr+" = "+fraza+"\n");
        for (int i=0; i<osoby.size(); i++){
            if(osoby.get(i) instanceof PracownikUczelni){
                switch(filtr){
                    case "nazwisko":{
                        if (osoby.get(i).getNazwisko().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "imie":{
                        if (osoby.get(i).getImie().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "stanowisko":{
                        if (osoby.get(i).getStanowisko().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "stazPracy":{
                        if (osoby.get(i).getStazPracy()==Integer.parseInt(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "liczbaNadgodzin":{
                        if (osoby.get(i).getLiczbaNadgodzin()==Integer.valueOf(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "pensja":{
                        if (osoby.get(i).getPensja()==Integer.valueOf(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    default:{
                        System.out.println("Nieprawidłowy filtr wyszukiwania!");
                        break;
                    }

                }
            }
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");

    }
    public void wyszukajStudenta(String filtr, String fraza){
        System.out.println("Wszyscy studenci tacy, że: "+filtr+" = "+fraza+"\n");
        for (int i=0; i<osoby.size(); i++){
            if(osoby.get(i) instanceof Student){
                switch(filtr){
                    case "nazwisko":{
                        if (osoby.get(i).getNazwisko().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "imie":{
                        if (osoby.get(i).getImie().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "nrIndeksu":{
                        if ((osoby.get(i)).getNrIndeksu().equals(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "nazwaKursu":{
                        for (int j=0; j<osoby.get(i).getKursy().size(); j++){
                            if (osoby.get(i).getKursy().get(j).getNazwaKursu().equals(fraza)){
                                System.out.println(osoby.get(i).toString());
                                System.out.println();
                            }
                        }
                        break;
                    }
                    case "rokStudiow":{
                        if ((osoby.get(i)).getRokStudiow()==Integer.valueOf(fraza)){
                            System.out.println(osoby.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    default:{
                        System.out.println("Nieprawidłowy filtr wyszukiwania!");
                        break;
                    }

                }
            }
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");
    }
    public void wyszukajKurs(String filtr, String fraza){
        System.out.println("Wszystkie kursy takie, że: "+filtr+" = "+fraza+"\n");
        for (int i=0; i<kursy.size(); i++){
                switch(filtr) {
                    case "nazwa": {
                        if (kursy.get(i).getNazwaKursu().equals(fraza)) {
                            System.out.println(kursy.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "nazwiskoProwadzacego": {
                        if (kursy.get(i).getNazwiskoProwadzacego().equals(fraza)) {
                            System.out.println(kursy.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    case "ECTS":{
                        if (kursy.get(i).getPunktyECTS()==Integer.valueOf(fraza)){
                            System.out.println(kursy.get(i).toString());
                            System.out.println();
                        }
                        break;
                    }
                    default: {
                        System.out.println("Nieprawidłowy filtr wyszukiwania!");
                        break;
                    }

                }
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");
    }
    public void wyswietlWszystkichPracownikow() {
        System.out.println("Wszyscy pracownicy uczelni: ");
        System.out.println("-----------------------------------------------------------------------------------------------------------");
        for (int i = 0; i < osoby.size(); i++) {
            if (osoby.get(i) instanceof PracownikUczelni) {
                System.out.println(osoby.get(i).toString());
                System.out.println();
            }
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");

    }
    public void wyswietlWszystkichStudentow() {
        System.out.println("Wszyscy studenci: ");
        System.out.println("-----------------------------------------------------------------------------------------------------------");

        for (int i = 0; i < osoby.size(); i++) {
            if (osoby.get(i) instanceof Student) {
                System.out.println(osoby.get(i).toString());
                System.out.println();
            }
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");

    }
    public void wyswietlWszystkieKursy() {
        System.out.println("Wszystkie kursy: ");
        System.out.println("-----------------------------------------------------------------------------------------------------------");
        for (int i = 0; i < kursy.size(); i++) {
            System.out.println(kursy.get(i).toString());
            System.out.println();
        }
        System.out.println("-----------------------------------------------------------------------------------------------------------");
    }
}