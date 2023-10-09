public class Samochod {
    private String marka;
    private String kolor;
    private int rokProdukcji;

    public Samochod(String marka, String kolor, int rokProdukcji) {
        this.marka = marka;
        this.kolor = kolor;
        this.rokProdukcji = rokProdukcji;
    }

    public String getMarka() {
        return marka;
    }
    public void setMarka(String marka) {
        this.marka = marka;
    }
    public String getKolor() {
        return kolor;
    }
    public void setKolor(String kolor) {
        this.kolor = kolor;
    }
    public int getRokProdukcji() {
        return rokProdukcji;
    }
    public void setRokProdukcji(int rokProdukcji) {
        this.rokProdukcji = rokProdukcji;
    }

    public String toString(){
        return kolor+" "+marka+" "+rokProdukcji;
    }
}
