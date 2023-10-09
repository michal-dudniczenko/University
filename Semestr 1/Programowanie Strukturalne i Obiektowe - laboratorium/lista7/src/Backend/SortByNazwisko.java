package Backend;

import java.util.Comparator;

public class SortByNazwisko implements Comparator<Osoba> {
    public int compare(Osoba o1, Osoba o2){
        return o1.getNazwisko().compareTo(o2.getNazwisko());
    }
}
