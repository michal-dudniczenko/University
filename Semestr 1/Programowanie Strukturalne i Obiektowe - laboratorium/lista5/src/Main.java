import java.util.ArrayList;
import java.util.Arrays;

public class Main {
    public static void main(String[] args){
        //przykładowe obiekty
        PracownikBadawczoDydaktyczny pbd1 = new PracownikBadawczoDydaktyczny("imie1", "nazwisko1", "pesel1", 20, "mężczyzna", "asystent", 5, 3000, 5);
        PracownikBadawczoDydaktyczny pbd2 = new PracownikBadawczoDydaktyczny("imie2", "nazwisko2", "pesel2", 35, "mężczyzna", "wykładowca", 12, 4000, 7);
        PracownikBadawczoDydaktyczny pbd3 = new PracownikBadawczoDydaktyczny("imie3", "nazwisko3", "pesel3", 42, "kobieta", "profesor nadzwyczajny", 9, 5500, 15);

        PracownikAdministracji pa1 = new PracownikAdministracji("imie4", "nazwisko4", "pesel4", 32, "mężczyzna", "referent", 2, 3250, 20);
        PracownikAdministracji pa2 = new PracownikAdministracji("imie5", "nazwisko5", "pesel5", 37, "kobieta", "specjalista", 2, 3500, 0);
        PracownikAdministracji pa3 = new PracownikAdministracji("imie6", "nazwisko6", "pesel6", 28, "kobieta", "starszy specjalista", 2, 4100, 10);

        Kursy k1 = new Kursy("analiza", "imie7", "nazwisko7", 4);
        Kursy k2 = new Kursy("algebra", "imie8", "nazwisko8", 6);
        Kursy k3 = new Kursy("psio", "imie9", "nazwisko9", 8);
        Kursy k4 = new Kursy("fizyka", "imie10", "nazwisko10", 4);
        Kursy k5 = new Kursy("osk", "imie11", "nazwisko11", 2);

        Student s1 = new Student("imie12", "nazwisko12", "pesel12", 20, "mężczyzna", "nrIndeksu1", 1, new ArrayList<>(Arrays.asList(k1, k2, k3)));
        Student s2 = new Student("imie13", "nazwisko13", "pesel13", 19, "mężczyzna", "nrIndeksu2", 2, new ArrayList<>(Arrays.asList(k1, k3, k5)));
        Student s3 = new Student("imie14", "nazwisko14", "pesel14", 21, "kobieta", "nrIndeksu3", 3, new ArrayList<>(Arrays.asList(k3, k4, k5, k2)));

        Uczelnia pwr = new Uczelnia();

        pwr.dodajOsobe(pbd1);
        pwr.dodajOsobe(pbd2);
        pwr.dodajOsobe(pbd3);
        pwr.dodajOsobe(pa1);
        pwr.dodajOsobe(pa2);
        pwr.dodajOsobe(pa3);
        pwr.dodajOsobe(s1);
        pwr.dodajOsobe(s2);
        pwr.dodajOsobe(s3);
        pwr.dodajKurs(k1);
        pwr.dodajKurs(k2);
        pwr.dodajKurs(k3);
        pwr.dodajKurs(k4);
        pwr.dodajKurs(k5);

        pwr.wyswietlWszystkichPracownikow();
        pwr.wyswietlWszystkichStudentow();
        pwr.wyswietlWszystkieKursy();

        pwr.wyszukajPracownika("nazwisko", "nazwisko3");
        pwr.wyszukajPracownika("imie", "imie2");
        pwr.wyszukajPracownika("stanowisko", "wykładowca");
        pwr.wyszukajPracownika("stazPracy", "2");
        pwr.wyszukajPracownika("liczbaNadgodzin", "10");
        pwr.wyszukajPracownika("pensja", "4000");

        pwr.wyszukajStudenta("nazwisko", "nazwisko13");
        pwr.wyszukajStudenta("imie", "imie12");
        pwr.wyszukajStudenta("nrIndeksu", "nrIndeksu2");
        pwr.wyszukajStudenta("rokStudiow", "3");
        pwr.wyszukajStudenta("nazwaKursu", "analiza");

        pwr.wyszukajKurs("nazwa", "analiza");
        pwr.wyszukajKurs("ECTS", "8");
        pwr.wyszukajKurs("nazwiskoProwadzacego", "nazwisko11");
    }

}
