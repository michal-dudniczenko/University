package Backend;

import java.io.Serializable;

public abstract class PracownikUczelni extends Osoba implements Serializable{
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
    public void setStanowisko(String stanowisko) {
        this.stanowisko = stanowisko;
    }
    public int getStazPracy() {
        return stazPracy;
    }
    public void setStazPracy(int stazPracy) {
        this.stazPracy = stazPracy;
    }
    public int getPensja() {
        return pensja;
    }
    public void setPensja(int pensja) {
        this.pensja = pensja;
    }

    public String toString(){
        return super.toString()+" Stanowisko: "+stanowisko+" Staż pracy: "+stazPracy+" Pensja: "+pensja;
    }

    public boolean equals(Object o){
        if (o instanceof PracownikUczelni){
            return this.hashCode()== o.hashCode();
        }
        return false;
    }
    public int hashCode() {
        return this.getPESEL().hashCode();
    }
}
