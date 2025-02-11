public class Gitarzysta extends Muzycy{
    private final Instrument gitara;
    private int popularnosc;
    private int portfel;

    public Gitarzysta(String imie, int wiek){
        super(imie, wiek);
        this.gitara = new Instrument(5);
        this.popularnosc=0;
        this.portfel=1000;
    }

    public void getInstrument() {
        System.out.println("Gitara:   "+"Jakość: "+gitara.getJakosc()+"      Zużycie: "+gitara.getZuzycie()+"%");
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
        System.out.print("Gitarzysta "+getImie()+" lat "+getWiek());
    }
    public void daj_koncert(){
        System.out.println(this.getImie() + " gra koncert!\nJak zwykle super mu poszło!");
        int zarobek = (popularnosc*10+this.getPoziom_gry()*100+gitara.getJakosc()*50+gitara.getTrudnosc()*20)*(100- gitara.getZuzycie())/100;
        setPortfel(getPortfel()+zarobek);
        setPopularnosc(getPopularnosc()+10);
        gitara.setZuzycie(gitara.getZuzycie()+10);
        System.out.println("Zarobiono: "+zarobek+"zł!");
    }

    public void idz_na_lekcje(){
        if (portfel>=50) {
            System.out.println(this.getImie() + " idzie na lekcje...\nOstro ćwiczył i sporo się nauczył!");
            Nauczyciel.nauczaj(this);
            portfel -= 50;
        }
        else System.out.println("Nie masz wystarczająco pieniedzy!");

    }
    public void ulepsz_instrument(){
        if (portfel>=1000) {
            gitara.setJakosc(gitara.getJakosc() + 1);
            System.out.println("Ulepszono instrument.");
            portfel-=1000;
        }
        else System.out.println("Nie masz wystarczajaco pieniedzy!");

    }
    public void napraw_instrument(){
        if (portfel>= gitara.getZuzycie()) {
            if(gitara.getZuzycie()>0) {
                portfel -= gitara.getZuzycie();
                gitara.setZuzycie(0);
                System.out.println("Naprawiono instrument.");
            }
            else System.out.println("Nie ma już czego naprawiać!");
        }
        else System.out.println("Nie masz wystarczajaco pieniedzy!");

    }
    public void cwicz(){
        System.out.println(this.getImie() + " idzie zagrać w barze!");
        gitara.setZuzycie(gitara.getZuzycie()+5);
        this.setPoziom_gry(this.getPoziom_gry()+1);
    }
}
