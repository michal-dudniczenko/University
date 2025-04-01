package Backend;

import java.util.Comparator;

public class SortByNazwiskoImie implements Comparator<Osoba> {
    public int compare(Osoba o1, Osoba o2){
        if (o1.getNazwisko().compareTo(o2.getNazwisko())!=0) {
            return o1.getNazwisko().compareTo(o2.getNazwisko());
        }
        else{
            return o1.getImie().compareTo(o2.getImie());
        }
    }
}
