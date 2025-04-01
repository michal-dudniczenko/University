public class Student {
    private int index;
    private String nazwisko;
    private String imie;
    private double ocena;

    public Student(int index, String nazwisko, String imie, double ocena) {
        this.index = index;
        this.nazwisko = nazwisko;
        this.imie = imie;
        this.ocena = ocena;
    }

    public int getIndex() {
        return index;
    }
    public void setIndex(int index) {
        this.index = index;
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
    public double getOcena() {
        return ocena;
    }
    public void setOcena(double ocena) {
        this.ocena = ocena;
    }

    public String toString(){
        return nazwisko+" "+imie+" index: "+index+" ocena: "+ocena;
    }
}
