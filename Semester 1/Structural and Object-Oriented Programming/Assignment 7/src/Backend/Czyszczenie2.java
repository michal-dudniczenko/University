package Backend;

import java.util.ArrayList;

public class Czyszczenie2 implements CzyszczenieListyStudentow{
    public static ArrayList<Osoba> SposobNaCzyszczenie(ArrayList<Osoba> osoby){
        for (int i=0; i<osoby.size(); i++){
            if (osoby.get(i) instanceof Student && osoby.get(i).getListaKursow().size()==0){
                osoby.remove(i);
            }
        }
        return osoby;
    }
}
