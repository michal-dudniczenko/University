package Backend;

import java.util.Comparator;

public class SortByNazwiskoWiek implements Comparator<Osoba> {
    public int compare(Osoba o1, Osoba o2){
        if (o1.getNazwisko().compareTo(o2.getNazwisko())!=0) {
            return o1.getNazwisko().compareTo(o2.getNazwisko());
        }
        else{
            return o2.getWiek()-o1.getWiek();
        }
    }
}
