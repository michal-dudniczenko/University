import java.util.ArrayList;

public abstract class Osoba {
    private String imie;
    private String nazwisko;
    private String PESEL;
    private int wiek;
    private String plec;

    public Osoba(String imie, String nazwisko, String PESEL, int wiek, String plec) {
        this.imie = imie;
        this.nazwisko = nazwisko;
        this.PESEL = PESEL;
        this.wiek = wiek;
        this.plec = plec;
    }

    public String getImie() {
        return imie;
    }
    public String getNazwisko() {
        return nazwisko;
    }
    public String getPESEL() {
        return PESEL;
    }
    public int getWiek() {
        return wiek;
    }
    public String getPlec() {
        return plec;
    }

    public String toString() {
        return "imie: "+imie+" nazwisko: "+nazwisko+" PESEL: "+PESEL+" wiek: "+wiek+" płeć: "+plec;
    }

    public abstract String getStanowisko();
    public abstract int getStazPracy();
    public abstract int getPensja();

    public abstract int getLiczbaNadgodzin();
    public abstract int getLiczbaPublikacji();

    public abstract String getNrIndeksu();
    public abstract int getRokStudiow();
    public abstract ArrayList<Kursy> getKursy();

}
