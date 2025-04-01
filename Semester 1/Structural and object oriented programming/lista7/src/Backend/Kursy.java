package Backend;

import java.io.Serializable;

public class Kursy implements Serializable{
    private String nazwaKursu;
    private String nazwiskoProwadzacego;
    private String imieProwadzacego;
    private int ECTS;

    public Kursy(String nazwaKursu, String nazwiskoProwadzacego, String imieProwadzacego, int ECTS) {
        this.nazwaKursu = nazwaKursu;
        this.nazwiskoProwadzacego = nazwiskoProwadzacego;
        this.imieProwadzacego = imieProwadzacego;
        this.ECTS = ECTS;
    }

    public String getNazwaKursu() {
        return nazwaKursu;
    }
    public void setNazwaKursu(String nazwaKursu) {
        this.nazwaKursu = nazwaKursu;
    }
    public String getNazwiskoProwadzacego() {
        return nazwiskoProwadzacego;
    }
    public void setNazwiskoProwadzacego(String nazwiskoProwadzacego) {
        this.nazwiskoProwadzacego = nazwiskoProwadzacego;
    }
    public String getImieProwadzacego() {
        return imieProwadzacego;
    }
    public void setImieProwadzacego(String imieProwadzacego) {
        this.imieProwadzacego = imieProwadzacego;
    }
    public int getECTS() {
        return ECTS;
    }
    public void setECTS(int ECTS) {
        this.ECTS = ECTS;
    }

    public String toString(){
        return "Nazwa kursu: "+nazwaKursu+" Prowadzący: "+nazwiskoProwadzacego+" Liczba punktów ECTS: "+ECTS;
    }
}
