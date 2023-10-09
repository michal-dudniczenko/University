import java.util.ArrayList;

public class Procesor {
    private ArrayList<Proces> procesy;
    private int obciazenie;
    private int[] historiaObciazen;

    public Procesor(){
        this.procesy=new ArrayList<>();
        this.obciazenie =0;
        this.historiaObciazen=new int[1000];
    }

    public ArrayList<Proces> getProcesy() {
        return procesy;
    }

    public int getObciazenie() {
        return obciazenie;
    }

    public void zwiekszZuzycie(int n){
        this.obciazenie +=n;
    }

    public void zmniejszZuzycie(int n){
        this.obciazenie -=n;
    }

    public void updateHistoria(int n){
        historiaObciazen[n]=this.obciazenie;
    }

    public void addProces(Proces p){
        this.procesy.add(p);
        zwiekszZuzycie(p.getObciazenieProcesora());
    }

    public void removeProces(Proces p){
        this.procesy.remove(p);
        zmniejszZuzycie(p.getObciazenieProcesora());
    }

    public double getSrednieObciazenie(){
        double wynik=0;
        for (int obciazenie : historiaObciazen){
            wynik+=obciazenie;
        }
        wynik/=1000;
        return wynik;
    }
}
