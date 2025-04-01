public class Proces {
    private int numer;
    private int blok;
    private int czasZgloszenia;
    private boolean realtime;
    private int deadline;

    public Proces(int numer, int blok, int czasZgloszenia) {
        this.numer = numer;
        this.blok = blok;
        this.czasZgloszenia = czasZgloszenia;
        this.realtime = false;
    }

    public Proces(int numer, int blok, int czasZgloszenia, boolean Realtime, int deadline) {
        this.numer = numer;
        this.blok = blok;
        this.czasZgloszenia = czasZgloszenia;
        this.realtime = Realtime;
        this.deadline = deadline;
    }

    public int getNumer() {
        return numer;
    }
    public void setNumer(int numer) {
        this.numer = numer;
    }
    public int getBlok() {
        return blok;
    }
    public void setBlok(int blok) {
        this.blok = blok;
    }
    public int getCzasZgloszenia() {
        return czasZgloszenia;
    }
    public void setCzasZgloszenia(int czasZgloszenia) {
        this.czasZgloszenia = czasZgloszenia;
    }
    public boolean isRealtime() {
        return realtime;
    }
    public void setRealtime(boolean realtime) {
        this.realtime = realtime;
    }
    public int getDeadline() {
        return deadline;
    }
    public void setDeadline(int deadline) {
        this.deadline = deadline;
    }

    public String toString(){
        String wynik= "nr."+numer+" czas: "+ czasZgloszenia+" blok: "+blok;
        if (realtime)wynik+=" deadline: "+deadline;
        return wynik;
    }
}
