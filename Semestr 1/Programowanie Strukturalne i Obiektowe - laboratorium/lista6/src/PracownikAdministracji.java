import java.io.Serializable;

public class PracownikAdministracji extends PracownikUczelni implements Serializable{
    private int liczbaNadgodzin;

    public PracownikAdministracji(String imie, String nazwisko, String PESEL, int wiek, String plec, String stanowisko, int stazPracy, int pensja, int liczbaNadgodzin) {
        super(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja);
        this.liczbaNadgodzin = liczbaNadgodzin;
    }

    public int getLiczbaNadgodzin() {
        return liczbaNadgodzin;
    }
    public void setLiczbaNadgodzin(int liczbaNadgodzin) {
        this.liczbaNadgodzin = liczbaNadgodzin;
    }

    public String toString(){
        return super.toString()+" Liczba nadgodzin: "+liczbaNadgodzin;
    }

}
