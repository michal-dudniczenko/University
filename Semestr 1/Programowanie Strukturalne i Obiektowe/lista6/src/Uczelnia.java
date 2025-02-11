import java.io.Serializable;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Scanner;

public class Uczelnia implements Observable, Serializable {
    private ArrayList<Osoba> osoby;
    private ArrayList<Kursy> kursy;
    private String najnowszeOgloszenie;

    public Uczelnia(){
        osoby = new ArrayList<Osoba>();
        kursy = new ArrayList<Kursy>();
        najnowszeOgloszenie="";

    }

    public ArrayList<Osoba> getOsoby() {
        return osoby;
    }
    public void setOsoby(ArrayList<Osoba> osoby) {
        this.osoby = osoby;
    }
    public ArrayList<Kursy> getKursy() {
        return kursy;
    }
    public void setKursy(ArrayList<Kursy> kursy) {
        this.kursy = kursy;
    }
    public String getNajnowszeOgloszenie(){
        return najnowszeOgloszenie;
    }
    public void setNajnowszeOgloszenie(String najnowszeOgloszenie){
        this.najnowszeOgloszenie=najnowszeOgloszenie;
    }

    public void dodajOsobe(){
        System.out.println("Podaj jaką osobę chcesz dodać:");
        System.out.println("1 - pracownik badawczo-dydaktyczny");
        System.out.println("2 - pracownik administracji");
        System.out.println("3 - student");
        Scanner input = new Scanner(System.in);
        switch(input.nextLine()){
            case "1":{
                System.out.println("Podaj imię:");
                String imie=input.nextLine();
                System.out.println("Podaj nazwisko:");
                String nazwisko=input.nextLine();
                System.out.println("Podaj PESEL:");
                String PESEL=input.nextLine();
                System.out.println("Podaj wiek:");
                int wiek=Integer.parseInt(input.nextLine());
                System.out.println("Podaj płeć:(m/k)");
                String plec=input.nextLine();
                System.out.println("Podaj stanowisko: (Asystent, Adiunkt, Profesor Nadzwyczajny, Profesor Zwyczajny, Wykładowca)");
                String stanowisko=input.nextLine();
                System.out.println("Podaj staż pracy:");
                int stazPracy=Integer.parseInt(input.nextLine());
                System.out.println("Podaj pensję:");
                int pensja=Integer.parseInt(input.nextLine());
                System.out.println("Podaj liczbę publikacji:");
                int liczbaPublikacji=Integer.parseInt(input.nextLine());
                osoby.add(new PracownikBadawczoDydaktyczny(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja, liczbaPublikacji));
                System.out.println("Pomyślnie dodano pracownika badawczo-dydaktycznego.");
                break;
            }
            case "2":{
                System.out.println("Podaj imię:");
                String imie=input.nextLine();
                System.out.println("Podaj nazwisko:");
                String nazwisko=input.nextLine();
                System.out.println("Podaj PESEL:");
                String PESEL=input.nextLine();
                System.out.println("Podaj wiek:");
                int wiek=Integer.parseInt(input.nextLine());
                System.out.println("Podaj płeć:(m/k)");
                String plec=input.nextLine();
                System.out.println("Podaj stanowisko: (Referent, Specjalista, Starszy Specjalista)");
                String stanowisko=input.nextLine();
                System.out.println("Podaj staż pracy:");
                int stazPracy=Integer.parseInt(input.nextLine());
                System.out.println("Podaj pensję:");
                int pensja=Integer.parseInt(input.nextLine());
                System.out.println("Podaj liczbę nadgodzin:");
                int liczbaNadgodzin=Integer.parseInt(input.nextLine());
                osoby.add(new PracownikAdministracji(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja, liczbaNadgodzin));
                System.out.println("Pomyślnie dodano pracownika administracji.");
                break;
            }
            case "3":{
                System.out.println("Podaj imię:");
                String imie=input.nextLine();
                System.out.println("Podaj nazwisko:");
                String nazwisko=input.nextLine();
                System.out.println("Podaj PESEL:");
                String PESEL=input.nextLine();
                System.out.println("Podaj wiek:");
                int wiek=Integer.parseInt(input.nextLine());
                System.out.println("Podaj płeć:(m/k)");
                String plec=input.nextLine();
                System.out.println("Podaj nr indeksu:");
                String nrIndeksu=input.nextLine();
                System.out.println("Podaj rok studiów:");
                int rokStudiow=Integer.parseInt(input.nextLine());
                ArrayList<Kursy> listaKursow=new ArrayList<Kursy>();
                System.out.println("Wybierz kursy z listy i zakończ wprowadzając 0.");
                for (int i=0; i<kursy.size(); i++){
                    System.out.println((i+1)+". "+kursy.get(i));
                }
                System.out.println("0 - koniec");
                System.out.println("-----------------------------------");
                int numerKursu=Integer.parseInt(input.nextLine());
                while (numerKursu!=0){
                    listaKursow.add(kursy.get(numerKursu-1));
                    numerKursu=Integer.parseInt(input.nextLine());
                }
                System.out.println("Czy student bierze udział w programie ERASMUS?(tak/nie)");
                boolean ERASMUS;
                if (input.nextLine().equals("tak")) ERASMUS=true;
                else ERASMUS=false;
                System.out.println("Czy student jest studentem 1 stopnia?(tak/nie))");
                boolean studia1stopnia;
                if (input.nextLine().equals("tak")) studia1stopnia=true;
                else studia1stopnia=false;
                System.out.println("Czy student studiuje stacjonarnie?(tak/nie))");
                boolean stacjonarne;
                if (input.nextLine().equals("tak")) stacjonarne=true;
                else stacjonarne=false;
                osoby.add(new Student(imie, nazwisko, PESEL, wiek, plec, nrIndeksu, rokStudiow, listaKursow, ERASMUS, studia1stopnia, stacjonarne, this));
                System.out.println("Pomyślnie dodano studenta.");
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
                break;
            }
        }
    }
    public void dodajKurs(){
        Scanner input = new Scanner(System.in);
        System.out.println("Podaj nazwę kursu:");
        String nazwaKursu=input.nextLine();
        System.out.println("Podaj nazwisko prowadzącego:");
        String nazwiskoProwadzacego=input.nextLine();
        System.out.println("Podaj imię prowadzącego:");
        String imieProwadzacego=input.nextLine();
        System.out.println("Podaj liczbę punktów ECTS:");
        int ECTS=Integer.parseInt(input.nextLine());
        kursy.add(new Kursy(nazwaKursu, nazwiskoProwadzacego, imieProwadzacego, ECTS));
        System.out.println("Pomyślnie dodano kurs.");
    }
    public void wyszukajPracownika(){
        Scanner input=new Scanner(System.in);
        System.out.println("Podaj według czego chcesz wyszukać pracownika:");
        System.out.println("1-nazwisko\n2-imię\n3-stanowisko (Asystent, Adiunkt, Profesor Nadzwyczajny, Profesor Zwyczajny, Wykładowca, Referent, Specjalista, Starszy Specjalista)\n4-staż pracy\n5-liczba nadgodzin\n6-pensja");
        String filtr=input.nextLine();
        System.out.println("Podaj frazę do wyszukania.");
        String fraza=input.nextLine();
        System.out.println("Wyszukani pracownicy:");
        System.out.println("-------------------------------------------");
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){

                    if (o instanceof PracownikUczelni && o.getNazwisko().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getImie().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStanowisko().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStazPracy()==Integer.parseInt(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "5":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikAdministracji && o.getLiczbaNadgodzin()==Integer.parseInt(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "6":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getPensja()==Integer.parseInt(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
        System.out.println("-------------------------------------------");
    }
    public void wyszukajStudenta(){
        Scanner input=new Scanner(System.in);
        System.out.println("Podaj według czego chcesz wyszukać studenta:");
        System.out.println("1-nazwisko\n2-imię\n3-nr indeksu\n4-rok studiów\n5-nazwa kursu");
        String filtr=input.nextLine();
        System.out.println("Podaj frazę do wyszukania.");
        String fraza=input.nextLine();
        System.out.println("Wyszukani studenci:");
        System.out.println("-------------------------------------------");
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNazwisko().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getImie().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNrIndeksu().equals(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getRokStudiow()==Integer.parseInt(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            case "5":{
                for (Osoba o : osoby){
                    if (o instanceof Student){
                        for (Kursy k : o.getListaKursow()){
                            if (k.getNazwaKursu().equals(fraza)){
                                System.out.println(o);
                                break;
                            }
                        }

                    }
                }
                break;
            }
            case "6":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getPensja()==Integer.parseInt(fraza)){
                        System.out.println(o);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
        System.out.println("-------------------------------------------");
    }
    public void wyszukajKurs(){
        Scanner input=new Scanner(System.in);
        System.out.println("Podaj według czego chcesz wyszukać kurs:");
        System.out.println("1-nazwa kursu\n2-nazwisko prowadzącego\n3-liczba punktów ECTS");
        String filtr=input.nextLine();
        System.out.println("Podaj frazę do wyszukania.");
        String fraza=input.nextLine();
        System.out.println("Wyszukane kursy:");
        System.out.println("-------------------------------------------");
        switch(filtr){
            case "1":{
                for (Kursy k : kursy){
                    if (k.getNazwaKursu().equals(fraza)){
                        System.out.println(k);
                    }
                }
                break;
            }
            case "2":{
                for (Kursy k : kursy){
                    if (k.getNazwiskoProwadzacego().equals(fraza)){
                        System.out.println(k);
                    }
                }
                break;
            }
            case "3":{
                for (Kursy k : kursy){
                    if (k.getECTS()==Integer.parseInt(fraza)){
                        System.out.println(k);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
        System.out.println("-------------------------------------------");
    }
    public void wyswietlPracownikow(){
        System.out.println("Wszyscy pracownicy:");
        System.out.println("-------------------------------------------");
        for (Osoba o : osoby){
            if (o instanceof PracownikUczelni){
                System.out.println(o);
            }
        }
        System.out.println("-------------------------------------------");
    }
    public void wyswietlStudentow(){
        System.out.println("Wszyscy studenci:");
        System.out.println("-------------------------------------------");
        for (Osoba o : osoby){
            if (o instanceof Student){
                System.out.println(o);
            }
        }
        System.out.println("-------------------------------------------");
    }
    public void wyswietlKursy(){
        System.out.println("Wszystkie kursy:");
        System.out.println("-------------------------------------------");
        for (Kursy k : kursy){
            System.out.println(k);
        }
        System.out.println("-------------------------------------------");
    }
    public void sortujOsoby(){
        System.out.println("Wybierz w jaki sposób chcesz posortować:");
        System.out.println("1 - po nazwisku");
        System.out.println("2 - po nazwisku i imieniu");
        System.out.println("3 - po nazwisku i wieku");
        Scanner pom = new Scanner(System.in);
        String wybor = pom.nextLine();
        switch(wybor){
            case "1":{
                Collections.sort(osoby, new SortByNazwisko());
                break;
            }
            case "2":{
                Collections.sort(osoby, new SortByNazwiskoImie());
                break;
            }
            case "3":{
                Collections.sort(osoby, new SortByNazwiskoWiek());
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór.");
            }
        }
        System.out.println("Pomyślnie posortowano listę osób we wskazany sposób.");

    }
    public void sortujKursy(){
        Collections.sort(kursy, new SortByECTSNazwisko());
        System.out.println("Pomyślnie posortowano listę kursów według ECTS i nazwisko prowadzącego.");
    }
    public void usunPracownikow(){
        Scanner input = new Scanner(System.in);
        String filtr;
        String fraza;
        System.out.println("Wybierz według czego chcesz usunąć pracowników:");
        System.out.println("(1-nazwisko / 2-imię / 3-staż pracy / 4-stanowisko)");
        filtr = input.nextLine();
        System.out.println("Podaj frazę do usunięcia pracowników:");
        fraza = input.nextLine();
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getNazwisko().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getImie().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStazPracy()==Integer.parseInt(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStanowisko().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
    }
    public void usunStudentow(){
        Scanner input = new Scanner(System.in);
        String filtr;
        String fraza;
        System.out.println("Wybierz według czego chcesz usunąć studentów:");
        System.out.println("(1-nazwisko / 2-imię / 3-rok studiów / 4-nr indeksu)");
        filtr = input.nextLine();
        System.out.println("Podaj frazę do usunięcia studentów:");
        fraza = input.nextLine();
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNazwisko().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getImie().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getRokStudiow()==Integer.parseInt(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNrIndeksu().equals(fraza)){
                        osoby.remove(o);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
    }
    public void usunKursy(){
        Scanner input = new Scanner(System.in);
        String filtr;
        String fraza;
        System.out.println("Wybierz według czego chcesz usunąć kursy:");
        System.out.println("(1-nazwisko prowadzącego/ 2-liczba punktów ECTS)");
        filtr = input.nextLine();
        System.out.println("Podaj frazę do usunięcia kursów:");
        fraza = input.nextLine();
        switch(filtr){
            case "1":{
                for (Kursy k : kursy){
                    if (k.getNazwiskoProwadzacego().equals(fraza)){
                        kursy.remove(k);
                    }
                }
                break;
            }
            case "2":{
                for (Kursy k : kursy){
                    if (k.getECTS()==Integer.parseInt(fraza)){
                        kursy.remove(k);
                    }
                }
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
            }
        }
    }
    public void czyszczenie(){
        Scanner input = new Scanner(System.in);
        System.out.println("Wybierz w jaki sposób chcesz wyczyścić listę studentów:");
        System.out.println("1 - usuń studentów z niewłaściwymi danymi");
        System.out.println("2 - usuń studentów niezapisanych na żadne kursy");
        String sposob = input.nextLine();
        switch(sposob){
            case "1":{
                this.osoby=Czyszczenie1.SposobNaCzyszczenie(osoby);
                System.out.println("Pomyślnie wyczyszczono listę studentów.");
                break;
            }
            case "2":{
                this.osoby=Czyszczenie2.SposobNaCzyszczenie(osoby);
                System.out.println("Pomyślnie wyczyszczono listę studentów.");
                break;
            }
            default:{
                System.out.println("Nieprawidłowy wybór!");
                break;
            }


        }
    }
    public void ogloszenie(){
        Scanner input = new Scanner(System.in);
        System.out.println("Podaj jakie ogłoszenie chcesz zamieścić:");
        setNajnowszeOgloszenie(input.nextLine());
        notifyObservers();
    }

    public void notifyObservers(){
        for (Osoba o : osoby){
            if (o instanceof Student){
                ((Student)o).update();
            }
        }
    }

}
