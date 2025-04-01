package Backend;

import java.io.Serializable;
import java.util.ArrayList;

public class Student extends Osoba implements Serializable, Observer{
    private String nrIndeksu;
    private int rokStudiow;
    private ArrayList<Kursy> listaKursow;
    private boolean ERASMUS;
    private boolean studia1stopnia;
    private boolean stacjonarne;
    private String wiadomoscUczelni;


    public Student(String imie, String nazwisko, String PESEL, int wiek, String plec, String nrIndeksu, int rokStudiow, ArrayList<Kursy> listaKursow, boolean ERASMUS, boolean studia1stopnia, boolean stacjonarne) {
        super(imie, nazwisko, PESEL, wiek, plec);
        this.nrIndeksu = nrIndeksu;
        this.rokStudiow = rokStudiow;
        this.listaKursow = listaKursow;
        this.ERASMUS = ERASMUS;
        this.studia1stopnia = studia1stopnia;
        this.stacjonarne = stacjonarne;
        this.wiadomoscUczelni = Main.getWiadomoscUczelni();
    }

    public String getNrIndeksu() {
        return nrIndeksu;
    }
    public void setNrIndeksu(String nrIndeksu) {
        this.nrIndeksu = nrIndeksu;
    }
    public int getRokStudiow() {
        return rokStudiow;
    }
    public void setRokStudiow(int rokStudiow) {
        this.rokStudiow = rokStudiow;
    }
    public ArrayList<Kursy> getListaKursow() {
        return listaKursow;
    }
    public void setListaKursow(ArrayList<Kursy> listaKursow) {
        this.listaKursow = listaKursow;
    }
    public boolean isERASMUS() {
        return ERASMUS;
    }
    public void setERASMUS(boolean ERASMUS) {
        this.ERASMUS = ERASMUS;
    }
    public boolean isStudia1stopnia() {
        return studia1stopnia;
    }
    public void setStudia1stopnia(boolean studia1stopnia) {
        this.studia1stopnia = studia1stopnia;
    }
    public boolean isStacjonarne() {
        return stacjonarne;
    }
    public void setStacjonarne(boolean stacjonarne) {
        this.stacjonarne = stacjonarne;
    }

    public String toString(){
        String kursyStudenta="";
        for (Kursy k : listaKursow){
            kursyStudenta+=k.getNazwaKursu()+" ";
        }

        return super.toString()+" Nr indeksu: "+nrIndeksu+" Rok studiów: "+rokStudiow;
    }

    public void update(){
        wiadomoscUczelni = Main.getWiadomoscUczelni();
        System.out.println("Studencie o indeksie "+nrIndeksu+", najnowsze ogłoszenie uczelni:");
        System.out.println(wiadomoscUczelni);
    }
    public boolean equals(Object o){
        if (o instanceof Student){
            return this.hashCode()==o.hashCode();
        }
        return false;
    }
    public int hashCode() {
        return this.getNrIndeksu().hashCode();
    }
}
