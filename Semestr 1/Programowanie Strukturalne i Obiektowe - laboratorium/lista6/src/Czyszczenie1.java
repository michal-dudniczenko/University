import java.util.ArrayList;

public class Czyszczenie1 implements CzyszczenieListyStudentow{
    public static ArrayList<Osoba> SposobNaCzyszczenie(ArrayList<Osoba> osoby){
        for (Osoba o : osoby){
            if (o instanceof Student){
                if (o.getNazwisko().length()==0||o.getImie().length()==0||o.getPESEL().length()==0||o.getWiek()<18||o.getPlec().length()==0||o.getNrIndeksu().length()!=6||o.getRokStudiow()<1||o.getRokStudiow()>5){
                    osoby.remove(o);
                }
            }
        }
        return osoby;
    }
}
