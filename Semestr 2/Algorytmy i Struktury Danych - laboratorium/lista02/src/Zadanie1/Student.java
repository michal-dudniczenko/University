package Zadanie1;

public class Student{
    private String indeks;
    private String nazwisko;
    private String imie;
    private double sr_ocena;

    public Student(String indeks, String nazwisko, String imie, double sr_ocena) {
        this.indeks = indeks;
        this.nazwisko = nazwisko;
        this.imie = imie;
        this.sr_ocena = sr_ocena;
    }

    public String getIndeks() {
        return indeks;
    }
    public void setIndeks(String indeks) {
        this.indeks = indeks;
    }
    public String getNazwisko() {
        return nazwisko;
    }
    public void setNazwisko(String nazwisko) {
        this.nazwisko = nazwisko;
    }
    public String getImie() {
        return imie;
    }
    public void setImie(String imie) {
        this.imie = imie;
    }
    public double getSr_ocena() {
        return sr_ocena;
    }
    public void setSr_ocena(double sr_ocena) {
        this.sr_ocena = sr_ocena;
    }

    public String toString(){
        return imie+" "+nazwisko+" "+indeks+" "+sr_ocena;
    }
}
