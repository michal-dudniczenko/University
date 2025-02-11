import java.util.Comparator;

public class KomparatorRokProdukcji implements Comparator<Samochod>{
    public int compare(Samochod s1, Samochod s2){
        return s1.getRokProdukcji()-s2.getRokProdukcji();
    }
}
