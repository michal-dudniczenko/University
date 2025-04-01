package Backend;

import java.util.ArrayList;

public class Czyszczenie1 implements CzyszczenieListyStudentow{
    public static ArrayList<Osoba> SposobNaCzyszczenie(ArrayList<Osoba> osoby){
        for (int i=0; i<osoby.size(); i++){
            if (osoby.get(i) instanceof Student){
                if (osoby.get(i).getNazwisko().length()==0||osoby.get(i).getImie().length()==0||osoby.get(i).getPESEL().length()==0||osoby.get(i).getWiek()<18||osoby.get(i).getPlec().length()==0||osoby.get(i).getNrIndeksu().length()!=6||osoby.get(i).getRokStudiow()<1||osoby.get(i).getRokStudiow()>5){
                    osoby.remove(i);
                }
            }
        }
        return osoby;
    }
}
