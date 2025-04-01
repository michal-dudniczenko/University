import java.io.Serializable;

public class PracownikBadawczoDydaktyczny extends PracownikUczelni implements Serializable{
    private int liczbaPublikacji;

    public PracownikBadawczoDydaktyczny(String imie, String nazwisko, String PESEL, int wiek, String plec, String stanowisko, int stazPracy, int pensja, int liczbaPublikacji) {
        super(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja);
        this.liczbaPublikacji = liczbaPublikacji;
    }

    public int getLiczbaPublikacji() {
        return liczbaPublikacji;
    }
    public void setLiczbaPublikacji(int liczbaPublikacji) {
        this.liczbaPublikacji = liczbaPublikacji;
    }

    public String toString(){
        return super.toString()+" Liczba publikacji: "+liczbaPublikacji;
    }

}
