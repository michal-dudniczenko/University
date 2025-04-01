public class Wiolonczelistka extends Muzycy{
    private final Instrument wiolonczela;
    private int popularnosc;
    private int portfel;

    public Wiolonczelistka(String imie, int wiek){
        super(imie, wiek);
        this.wiolonczela = new Instrument(10);
        this.popularnosc=0;
        this.portfel=1000;
    }

    public void getInstrument() {
        System.out.println("Wiolonczela:   "+"Jakość: "+wiolonczela.getJakosc()+"      Zużycie: "+wiolonczela.getZuzycie()+"%");
    }

    public int getPopularnosc() {
        return popularnosc;
    }

    public void setPopularnosc(int popularnosc) {
        this.popularnosc = popularnosc;
    }

    public int getPortfel() {
        return portfel;
    }

    public void setPortfel(int portfel) {
        this.portfel = portfel;
    }

    public void get_stan(){
        System.out.println("Imię: "+getImie()+"     Wiek: "+getWiek()+"     Poziom gry: "+ getPoziom_gry()+"     Popularność: "+popularnosc+"     Stan konta: "+portfel);
    }

    public void getStanShort(){
        System.out.print("Wiolonczelistka "+getImie()+" lat "+getWiek());
    }
    public void daj_koncert(){
        System.out.println(this.getImie() + " gra koncert!\nJak zwykle super jej poszło!");
        int zarobek = (popularnosc*10+this.getPoziom_gry()*100+wiolonczela.getJakosc()*50+wiolonczela.getTrudnosc()*20)*(100- wiolonczela.getZuzycie())/100;
        setPortfel(getPortfel()+zarobek);
        setPopularnosc(getPopularnosc()+10);
        wiolonczela.setZuzycie(wiolonczela.getZuzycie()+10);
        System.out.println("Zarobiono: "+zarobek+"zł!");
    }

    public void idz_na_lekcje(){
        if (portfel>=50) {
            System.out.println(this.getImie() + " idzie na lekcje...\nOstro ćwiczyła i sporo się nauczyła!");
            Nauczyciel.nauczaj(this);
            portfel -= 50;
        }
        else System.out.println("Nie masz wystarczająco pieniedzy!");

    }
    public void ulepsz_instrument(){
        if (portfel>=1000) {
            wiolonczela.setJakosc(wiolonczela.getJakosc() + 1);
            System.out.println("Ulepszono instrument.");
            portfel-=1000;
        }
        else System.out.println("Nie masz wystarczajaco pieniedzy!");

    }
    public void napraw_instrument(){
        if (portfel>= wiolonczela.getZuzycie()) {
            if(wiolonczela.getZuzycie()>0) {
                portfel -= wiolonczela.getZuzycie();
                wiolonczela.setZuzycie(0);
                System.out.println("Naprawiono instrument.");
            }
            else System.out.println("Nie ma już czego naprawiać!");
        }
        else System.out.println("Nie masz wystarczajaco pieniedzy!");

    }
    public void cwicz(){
        System.out.println(this.getImie() + " idzie ćwiczyć w orkiestrze!");
        wiolonczela.setZuzycie(wiolonczela.getZuzycie()+5);
        this.setPoziom_gry(this.getPoziom_gry()+1);
    }
}
