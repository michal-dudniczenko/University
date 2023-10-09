import java.util.ArrayList;

public class Student extends Osoba{
    private String nrIndeksu;
    private int rokStudiow;
    private ArrayList<Kursy> kursy;

    public Student(String imie, String nazwisko, String PESEL, int wiek, String plec, String nrIndeksu, int rokStudiow, ArrayList<Kursy> kursy) {
        super(imie, nazwisko, PESEL, wiek, plec);
        this.nrIndeksu = nrIndeksu;
        this.rokStudiow = rokStudiow;
        this.kursy = kursy;
    }

    public int getRokStudiow() {
        return rokStudiow;
    }
    public ArrayList<Kursy> getKursy() {
        return kursy;
    }
    public String getNrIndeksu() {
        return nrIndeksu;
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

    public int getLiczbaNadgodzin(){
        return -1;
    }
    public int getLiczbaPublikacji(){
        return -1;
    }


    public String toString(){
        String kursyDoWypisania="";
        for (Kursy i : kursy){kursyDoWypisania+=i.getNazwaKursu()+", ";}
        return super.toString()+" nr indeksu: "+nrIndeksu+" rok studiow: "+rokStudiow+" Jego kursy: "+kursyDoWypisania;
    }
}
