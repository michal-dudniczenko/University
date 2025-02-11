import java.util.ArrayList;

public class PracownikAdministracji extends PracownikUczelni{
    private int liczbaNadgodzin;

    public PracownikAdministracji(String imie, String nazwisko, String PESEL, int wiek, String plec, String stanowisko, int stazPracy, int pensja, int liczbaNadgodzin) {
        super(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja);
        this.liczbaNadgodzin = liczbaNadgodzin;
    }

    public int getLiczbaNadgodzin() {
        return liczbaNadgodzin;
    }

    public String getStanowisko(){
        return super.getStanowisko();
    }
    public int getStazPracy(){
        return super.getStazPracy();
    }
    public int getPensja(){
        return super.getPensja();
    }

    public int getLiczbaPublikacji(){
        return -1;
    }

    public String getNrIndeksu(){
        return "";
    }
    public int getRokStudiow(){
        return -1;
    }
    public ArrayList<Kursy> getKursy() {
        return new ArrayList<>();
    }

    public String toString(){
        return super.toString()+" liczba nadgodzin: "+liczbaNadgodzin;
    }
}
