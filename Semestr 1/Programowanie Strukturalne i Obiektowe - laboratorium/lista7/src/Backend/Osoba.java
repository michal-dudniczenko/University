package Backend;

import java.io.Serializable;
import java.util.ArrayList;

public abstract class Osoba implements Serializable, Observer{
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
    public void setImie(String imie) {
        this.imie = imie;
    }
    public String getNazwisko() {
        return nazwisko;
    }
    public void setNazwisko(String nazwisko) {
        this.nazwisko = nazwisko;
    }
    public String getPESEL() {
        return PESEL;
    }
    public void setPESEL(String PESEL) {
        this.PESEL = PESEL;
    }
    public int getWiek() {
        return wiek;
    }
    public void setWiek(int wiek) {
        this.wiek = wiek;
    }
    public String getPlec() {
        return plec;
    }
    public void setPlec(String plec) {
        this.plec = plec;
    }

    public String getStanowisko(){
        return "";
    }
    public int getStazPracy(){
        return -1;
    }
    public int getPensja(){
        return -1;
    }
    public int getLiczbaPublikacji(){
        return -1;
    }
    public int getLiczbaNadgodzin(){
        return -1;
    }
    public String getNrIndeksu() {
        return "";
    }
    public int getRokStudiow() {
        return -1;
    }
    public ArrayList<Kursy> getListaKursow() {
        return new ArrayList<Kursy>();
    }
    public boolean isERASMUS() {
        return true;
    }
    public boolean isStudia1stopnia() {
        return true;
    }
    public boolean isStacjonarne() {
        return true;
    }

    public String toString(){
        return "Imię: "+imie+" Nazwisko: "+nazwisko+" PESEL: "+PESEL+" Wiek: "+wiek+" Płeć: "+plec;
    }
    public void update(){}
}
