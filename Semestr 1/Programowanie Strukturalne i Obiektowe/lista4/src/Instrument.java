public class Instrument {
    private int jakosc;
    private int zuzycie;
    private final int trudnosc;

    public Instrument(int trudnosc){
        this.jakosc=1;
        this.zuzycie=0;
        this.trudnosc=trudnosc;
    }

    public int getJakosc() {
        return jakosc;
    }

    public void setJakosc(int jakosc) {
        this.jakosc = jakosc;
    }

    public int getTrudnosc() {
        return trudnosc;
    }

    public int getZuzycie() {
        return zuzycie;
    }

    public void setZuzycie(int zuzycie) {
        this.zuzycie=zuzycie;
    }

}
