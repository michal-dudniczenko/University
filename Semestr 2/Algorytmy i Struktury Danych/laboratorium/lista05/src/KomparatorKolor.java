import java.util.Comparator;

public class KomparatorKolor implements Comparator<Samochod>{
    public int compare(Samochod s1, Samochod s2){
        return s1.getKolor().compareTo(s2.getKolor());
    }
}
