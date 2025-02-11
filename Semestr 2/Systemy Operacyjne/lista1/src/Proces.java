public class Proces{
    private int numer;
    private int dlugoscFazy;
    private int czasZgloszenia;
    private int czasOczekiwania;
    private int pozostalyCzas;
    private int czasRozpoczecia;
    private int czasZakonczenia;
    private boolean czyRozpoczety;

    public Proces(int numer, int dlugoscFazy, int czasZgloszenia){
        this.numer=numer;
        this.dlugoscFazy=dlugoscFazy;
        this.czasZgloszenia=czasZgloszenia;
        this.czasOczekiwania=0;
        this.pozostalyCzas = dlugoscFazy;
        this.czyRozpoczety=false;
    }

    public int getNumer() {
        return numer;
    }
    public void setNumer(int numer) {
        this.numer = numer;
    }
    public int getDlugoscFazy() {
        return dlugoscFazy;
    }
    public void setDlugoscFazy(int dlugoscFazy) {
        this.dlugoscFazy = dlugoscFazy;
    }
    public int getCzasZgloszenia() {
        return czasZgloszenia;
    }
    public void setCzasZgloszenia(int czasZgloszenia) {
        this.czasZgloszenia = czasZgloszenia;
    }
    public int getCzasOczekiwania() {
        return czasOczekiwania;
    }
    public int getPozostalyCzas() {
        return pozostalyCzas;
    }
    public int getCzasRozpoczecia() {
        return czasRozpoczecia;
    }
    public void setCzasRozpoczecia(int czasRozpoczecia) {
        this.czasRozpoczecia = czasRozpoczecia;
    }
    public int getCzasZakonczenia() {
        return czasZakonczenia;
    }
    public void setCzasZakonczenia(int czasZakonczenia) {
        this.czasZakonczenia = czasZakonczenia;
    }
    public boolean isCzyRozpoczety() {
        return czyRozpoczety;
    }
    public void setCzyRozpoczety(boolean czyRozpoczety) {
        this.czyRozpoczety = czyRozpoczety;
    }

    public void zmniejszPozostalyCzas() {
        this.pozostalyCzas=getPozostalyCzas()-1;
    }
    public void zwiekszCzasOczekiwania(){this.czasOczekiwania=getCzasOczekiwania()+1;}

    public String toString(){
        return "nr: "+numer+" dl. fazy: "+dlugoscFazy+" czas zgłoszenia: "+czasZgloszenia;
    }
}
