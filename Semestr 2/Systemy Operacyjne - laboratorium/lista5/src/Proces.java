public class Proces {
    private int czasWykonania;
    private final int obciazenieProcesora;
    private final int docelowyProcesor;

    public Proces(int czasWykonania, int obciazenieProcesora, int docelowyProcesor){
        this.czasWykonania=czasWykonania;
        this.obciazenieProcesora = obciazenieProcesora;
        this.docelowyProcesor=docelowyProcesor;
    }

    public int getCzasWykonania() {
        return czasWykonania;
    }

    public int getObciazenieProcesora() {
        return obciazenieProcesora;
    }

    public int getDocelowyProcesor(){
        return docelowyProcesor;
    }

    public void zmniejszCzas(){
        this.czasWykonania--;
    }
}
