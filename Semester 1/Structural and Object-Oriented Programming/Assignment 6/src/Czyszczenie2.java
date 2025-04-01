import java.util.ArrayList;

public class Czyszczenie2 implements CzyszczenieListyStudentow{
    public static ArrayList<Osoba> SposobNaCzyszczenie(ArrayList<Osoba> osoby){
        for (Osoba o : osoby){
            if (o instanceof Student && o.getListaKursow().size()==0){
                osoby.remove(o);
            }
        }
        return osoby;
    }
}
