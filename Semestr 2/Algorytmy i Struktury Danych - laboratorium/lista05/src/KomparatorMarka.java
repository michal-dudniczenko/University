import java.util.Comparator;

public class KomparatorMarka implements Comparator<Samochod>{
    public int compare(Samochod s1, Samochod s2){
        return s1.getMarka().compareTo(s2.getMarka());
    }
}
