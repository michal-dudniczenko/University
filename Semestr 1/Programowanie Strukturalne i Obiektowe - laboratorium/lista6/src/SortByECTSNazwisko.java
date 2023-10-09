import java.util.Comparator;

public class SortByECTSNazwisko implements Comparator<Kursy> {
    public int compare(Kursy k1, Kursy k2){
        if (k1.getECTS()-k2.getECTS()!=0) {
            return k1.getECTS()-k2.getECTS();
        }
        else{
            return k1.getNazwiskoProwadzacego().compareTo(k2.getNazwiskoProwadzacego());
        }
    }
}
