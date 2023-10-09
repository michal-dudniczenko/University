import java.util.ArrayList;
import java.util.Random;

public class Algorytmy {
    public static int FIFO(int ramki, ArrayList<Integer> requests){
        Integer[] pamiec = new Integer[ramki];

        int[] czasWPamieci = new int[ramki];

        int licznik=0;

        boolean hit;

        for (int r=0; r<requests.size(); r++){
            Integer request=requests.get(r);
            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null){
                    czasWPamieci[i]++;
                }
            }

            hit=false;

            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null && pamiec[i].equals(request)){
                    hit=true;
                    break;
                }
            }
            if (!hit){
                licznik++;
                if (r<ramki) {
                    for (int i = 0; i < ramki; i++) {
                        if (pamiec[i] == null) {
                            pamiec[i] = request;
                            break;
                        }
                    }
                }
                else{
                    int max=-1;
                    int doUsuniecia=0;
                    for (int i=0; i<ramki; i++){
                        if (czasWPamieci[i]>max){
                            doUsuniecia=i;
                            max=czasWPamieci[i];
                        }
                    }
                    pamiec[doUsuniecia]=request;
                    czasWPamieci[doUsuniecia]=0;
                }
            }
        }
        return licznik;
    }

    public static int OPT(int ramki, ArrayList<Integer> requests){
        Integer[] pamiec = new Integer[ramki];

        int licznik=0;

        boolean hit;

        for (int r=0; r<requests.size(); r++){
            Integer request=requests.get(r);

            hit=false;

            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null && pamiec[i].equals(request)){
                    hit=true;
                    break;
                }
            }
            if (!hit){
                licznik++;
                if (r<ramki) {
                    for (int i = 0; i < ramki; i++) {
                        if (pamiec[i] == null) {
                            pamiec[i] = request;
                            break;
                        }
                    }
                }
                else{
                    int max=-1;
                    int doUsuniecia=0;
                    Integer strona;
                    int odleglosc;
                    for (int i=0; i<ramki; i++){
                        strona=pamiec[i];
                        odleglosc=0;
                        for (int j=r+1; j<requests.size(); j++){
                            if (requests.get(j).equals(strona)){
                                odleglosc = j-r;
                                if (odleglosc>max){
                                    max=odleglosc;
                                    doUsuniecia=i;
                                }
                                break;
                            }
                        }
                        if (odleglosc==0){
                            doUsuniecia=i;
                            break;
                        }
                    }
                    pamiec[doUsuniecia]=request;
                }
            }
        }
        return licznik;
    }

    public static int LRU(int ramki, ArrayList<Integer> requests){
        Integer[] pamiec = new Integer[ramki];

        int licznik=0;

        boolean hit;

        for (int r=0; r<requests.size(); r++){
            Integer request=requests.get(r);

            hit=false;

            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null && pamiec[i].equals(request)){
                    hit=true;
                    break;
                }
            }
            if (!hit){
                licznik++;
                if (r<ramki) {
                    for (int i = 0; i < ramki; i++) {
                        if (pamiec[i] == null) {
                            pamiec[i] = request;
                            break;
                        }
                    }
                }
                else{
                    int max=-1;
                    int doUsuniecia=0;
                    Integer strona;
                    int odleglosc;
                    for (int i=0; i<ramki; i++){
                        strona=pamiec[i];
                        for (int j=r-1; j>=0; j--){
                            if (requests.get(j).equals(strona)){
                                odleglosc = r-j;
                                if (odleglosc>max){
                                    max=odleglosc;
                                    doUsuniecia=i;
                                }
                                break;
                            }
                        }
                    }
                    pamiec[doUsuniecia]=request;
                }
            }
        }
        return licznik;
    }

    public static int LRU_APR(int ramki, ArrayList<Integer> requests){
        Integer[] pamiec = new Integer[ramki];

        int licznik=0;

        boolean hit;

        ArrayList<int[]> FIFO = new ArrayList<>();

        for (int r=0; r<requests.size(); r++){
            Integer request=requests.get(r);

            hit=false;

            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null && pamiec[i].equals(request)){
                    hit=true;
                    for (int[] s : FIFO){
                        if (s[0]==request){
                            s[1]=1;
                            break;
                        }
                    }
                    break;
                }
            }
            if (!hit){
                licznik++;
                if (r<ramki) {
                    for (int i = 0; i < ramki; i++) {
                        if (pamiec[i] == null) {
                            pamiec[i] = request;
                            int[] temp = {request, 1};
                            FIFO.add(temp);
                            break;
                        }
                    }
                }
                else{
                    int doUsuniecia=0;

                    while (FIFO.get(0)[1]!=0){
                        int[] temp = {FIFO.get(0)[0], 0};
                        FIFO.add(temp);
                        FIFO.remove(0);
                    }

                    int strona = FIFO.get(0)[0];
                    FIFO.remove(0);

                    for (int i=0; i<ramki; i++){
                        if (pamiec[i].equals(strona)){
                            doUsuniecia=i;
                            break;
                        }
                    }

                    pamiec[doUsuniecia]=request;
                    int[] temp = {request, 1};
                    FIFO.add(temp);
                }
            }
        }
        return licznik;
    }


    public static int RAND(int ramki, ArrayList<Integer> requests){
        Integer[] pamiec = new Integer[ramki];

        int licznik=0;

        boolean hit;

        for (int r=0; r<requests.size(); r++){
            Integer request=requests.get(r);

            hit=false;

            for (int i=0; i<ramki; i++){
                if (pamiec[i]!=null && pamiec[i].equals(request)){
                    hit=true;
                    break;
                }
            }
            if (!hit){
                licznik++;
                if (r<ramki) {
                    for (int i = 0; i < ramki; i++) {
                        if (pamiec[i] == null) {
                            pamiec[i] = request;
                            break;
                        }
                    }
                }
                else{
                    Random rand = new Random();

                    int doUsuniecia=rand.nextInt(ramki);
                    pamiec[doUsuniecia]=request;
                }
            }
        }
        return licznik;
    }


}
