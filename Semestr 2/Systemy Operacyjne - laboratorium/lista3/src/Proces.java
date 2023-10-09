import java.util.Random;

public class Proces {
    public int[] odwolania;

    public Proces(int liczbaStron, int promien){
        Random rand = new Random();
        int liczbaOdwolan = rand.nextInt(10)+1;
        this.odwolania = new int[liczbaOdwolan];

        int odwolanieGlowne = rand.nextInt(liczbaStron);
        int odwolanie;
        for (int i=0; i<liczbaOdwolan; i++){
            odwolanie = rand.nextInt(odwolanieGlowne-promien, odwolanieGlowne+promien);
            if (odwolanie<0)odwolanie=0;
            else if (odwolanie>=liczbaStron)odwolanie=liczbaStron-1;
            odwolania[i]=odwolanie;
        }
    }
}
