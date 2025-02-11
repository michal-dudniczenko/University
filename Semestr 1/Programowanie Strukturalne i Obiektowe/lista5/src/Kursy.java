public class Kursy {
    private String nazwaKursu;
    private String imieProwadzacego;
    private String nazwiskoProwadzacego;
    private int punktyECTS;

    public Kursy(String nazwaKursu, String imieProwadzacego, String nazwiskoProwadzacego, int punktyECTS) {
        this.nazwaKursu = nazwaKursu;
        this.imieProwadzacego = imieProwadzacego;
        this.nazwiskoProwadzacego = nazwiskoProwadzacego;
        this.punktyECTS = punktyECTS;
    }

    public String getNazwaKursu() {
        return nazwaKursu;
    }
    public String getImieProwadzacego() {
        return imieProwadzacego;
    }
    public String getNazwiskoProwadzacego() {
        return nazwiskoProwadzacego;
    }
    public int getPunktyECTS() {
        return punktyECTS;
    }

    public String toString() {
        return "nazwa kursu: "+nazwaKursu+" imie prowadzącego: "+imieProwadzacego+" nazwisko prowadzącego: "+nazwiskoProwadzacego+" punkty ECTS: "+punktyECTS;
    }
}
