import java.util.ArrayList;

public class PracownikBadawczoDydaktyczny extends PracownikUczelni{
    private int liczbaPublikacji;

    public PracownikBadawczoDydaktyczny(String imie, String nazwisko, String PESEL, int wiek, String plec, String stanowisko, int stazPracy, int pensja, int liczbaPublikacji) {
        super(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja);
        this.liczbaPublikacji = liczbaPublikacji;
    }

    public int getLiczbaPublikacji() {
        return liczbaPublikacji;
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

    public int getLiczbaNadgodzin(){
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
        return super.toString()+" liczba publikacji: "+liczbaPublikacji;
    }
}
