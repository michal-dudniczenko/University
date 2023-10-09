import java.util.Comparator;

public class KomparatorC implements Comparator<Samochod>{
    public int compare(Samochod s1, Samochod s2){
        int temp;
        if ((temp=s1.getMarka().compareTo(s2.getMarka()))!=0)return temp;
        else if ((temp=s1.getKolor().compareTo(s2.getKolor()))!=0)return temp;
        else return s1.getRokProdukcji()- s2.getRokProdukcji();
    }
}
