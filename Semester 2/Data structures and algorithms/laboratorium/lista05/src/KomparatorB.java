import java.util.Comparator;

public class KomparatorB implements Comparator<Samochod>{
    public int compare(Samochod s1, Samochod s2){
        int temp;
        if ((temp=new KomparatorMarka().compare(s1, s2))!=0)return temp;
        else if ((temp=new KomparatorKolor().compare(s1, s2))!=0)return temp;
        else return new KomparatorRokProdukcji().compare(s1,s2);
    }
}
