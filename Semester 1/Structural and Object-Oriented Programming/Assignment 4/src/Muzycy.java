public abstract class Muzycy {
    private String imie;
    private int wiek;
    private int poziom_gry;

    public Muzycy(String imie, int wiek) {
        this.imie = imie;
        this.wiek = wiek;
        this.poziom_gry = 0;
    }

    public String getImie() {
        return imie;
    }

    public void setImie(String imie) {
        this.imie = imie;
    }

    public int getWiek() {
        return wiek;
    }

    public void setWiek(int wiek) {
        this.wiek = wiek;
    }

    public int getPoziom_gry() {
        return poziom_gry;
    }

    public void setPoziom_gry(int poziom_gry) {
        this.poziom_gry = poziom_gry;
    }

    public abstract void get_stan();
    public abstract void getStanShort();
    public abstract void getInstrument();
    public abstract void daj_koncert();
    public abstract void idz_na_lekcje();
    public abstract void ulepsz_instrument();
    public abstract void napraw_instrument();
    public abstract void cwicz();
    public abstract int getPortfel();
    public abstract int getPopularnosc();




}
