public abstract class PracownikUczelni extends Osoba{
    private String stanowisko;
    private int stazPracy;
    private int pensja;

    public PracownikUczelni(String imie, String nazwisko, String PESEL, int wiek, String plec, String stanowisko, int stazPracy, int pensja) {
        super(imie, nazwisko, PESEL, wiek, plec);
        this.stanowisko = stanowisko;
        this.stazPracy = stazPracy;
        this.pensja = pensja;
    }

    public String getStanowisko() {
        return stanowisko;
    }
    public int getStazPracy() {
        return stazPracy;
    }
    public int getPensja() {
        return pensja;
    }

    public String toString() {
        return super.toString() + " stanowisko: " + stanowisko + " staż pracy: " + stazPracy + " pensja: " + pensja;
    }
}
