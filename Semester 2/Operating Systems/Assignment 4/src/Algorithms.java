import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;

public class Algorithms {
    public static void przydzialRowny(ArrayList<Request> req, int liczbaRamek, int liczbaProcesow){
        int[] result = new int[liczbaProcesow];
        ArrayList<ArrayList<Integer>> przydzial = new ArrayList<>();
        for (int i=0; i<liczbaProcesow; i++){
            przydzial.add(new ArrayList<>());
        }
        int[] ramki = new int[liczbaRamek];
        Arrays.fill(ramki, -1);

        ArrayList<Request> requests = new ArrayList<>();
        for (Request r : req){
            requests.add(new Request(r.getProcess(), r.getPage()));
        }

        int liczbaPrzydzielonych = liczbaRamek/liczbaProcesow;

        int marker=0;
        for (int i=0; i<liczbaProcesow; i++){
            for (int j=marker; j<=Math.min(marker+liczbaPrzydzielonych-1,liczbaRamek-1); j++){
                przydzial.get(i).add(j);
            }
            marker+=liczbaPrzydzielonych;
        }
        while (marker<liczbaRamek){
            przydzial.get(liczbaProcesow-1).add(marker);
            marker++;
        }

        for (int requestNumber=0; requestNumber<requests.size(); requestNumber++){
            int process = requests.get(requestNumber).getProcess();
            if (LRU(requestNumber, requests, ramki, przydzial, process)){
                result[process]++;
            }
        }

        System.out.println("-----------------------------");
        System.out.println("\nPRZYDZIAŁ RÓWNY\n");

        int suma=0;
        System.out.println("Liczba błędów strony:\n");
        for (int i=0; i<liczbaProcesow; i++){
            System.out.println("Proces "+(i+1)+": "+result[i]);
            suma+=result[i];
        }
        System.out.println("\nSuma = "+suma);
    }

    public static void przydzialProporcjonalny(ArrayList<Request> req, int liczbaRamek, int liczbaProcesow){
        int[] result = new int[liczbaProcesow];
        ArrayList<ArrayList<Integer>> przydzial = new ArrayList<>();
        for (int i=0; i<liczbaProcesow; i++){
            przydzial.add(new ArrayList<>());
        }
        int[] ramki = new int[liczbaRamek];
        Arrays.fill(ramki, -1);

        ArrayList<Request> requests = new ArrayList<>();
        for (Request r : req){
            requests.add(new Request(r.getProcess(), r.getPage()));
        }

        HashSet<Integer> temp = new HashSet<>();
        for (Request r : requests){
            temp.add(r.getPage());
        }
        double wszystkieUzywaneStrony = temp.size();

        int marker=0;
        for (int i=0; i<liczbaProcesow; i++){
            temp = new HashSet<>();
            for (Request r : requests){
                if (r.getProcess()==i){
                    temp.add(r.getPage());
                }
            }
            double uzywaneStrony = temp.size();

            int liczbaPrzydzielonych = Math.max((int)Math.round(uzywaneStrony/wszystkieUzywaneStrony*liczbaRamek),1);
            for (int j=marker; j<=Math.min(marker+liczbaPrzydzielonych-1,liczbaRamek-1); j++){
                przydzial.get(i).add(j);
            }
            marker+=liczbaPrzydzielonych;
        }
        while (marker<liczbaRamek){
            przydzial.get(liczbaProcesow-1).add(marker);
            marker++;
        }

        for (int requestNumber=0; requestNumber<requests.size(); requestNumber++){
            int process = requests.get(requestNumber).getProcess();
            if (LRU(requestNumber, requests, ramki, przydzial, process)){
                result[process]++;
            }
        }

        System.out.println("-----------------------------");
        System.out.println("\nPRZYDZIAŁ PROPORCJONALNY\n");

        int suma=0;
        System.out.println("Liczba błędów strony:\n");
        for (int i=0; i<liczbaProcesow; i++){
            System.out.println("Proces "+(i+1)+": "+result[i]);
            suma+=result[i];
        }
        System.out.println("\nSuma = "+suma);
    }

    public static void sterowanieCzestoscia(ArrayList<Request> req, int liczbaRamek, int liczbaProcesow, int l, int h, int t){
        int[] result = new int[liczbaProcesow];
        ArrayList<ArrayList<Integer>> przydzial = new ArrayList<>();
        for (int i=0; i<liczbaProcesow; i++){
            przydzial.add(new ArrayList<>());
        }
        int[] ramki = new int[liczbaRamek];
        Arrays.fill(ramki, -1);

        ArrayList<Request> requests = new ArrayList<>();
        for (Request r : req){
            requests.add(new Request(r.getProcess(), r.getPage()));
        }

        HashSet<Integer> temp = new HashSet<>();
        for (Request r : requests){
            temp.add(r.getPage());
        }
        double wszystkieUzywaneStrony = temp.size();

        int marker=0;
        for (int i=0; i<liczbaProcesow; i++){
            temp = new HashSet<>();
            for (Request r : requests){
                if (r.getProcess()==i){
                    temp.add(r.getPage());
                }
            }
            double uzywaneStrony = temp.size();

            int liczbaPrzydzielonych = Math.max((int)Math.round(uzywaneStrony/wszystkieUzywaneStrony*liczbaRamek),1);
            for (int j=marker; j<=Math.min(marker+liczbaPrzydzielonych-1,liczbaRamek-1); j++){
                przydzial.get(i).add(j);
            }
            marker+=liczbaPrzydzielonych;
        }
        while (marker<liczbaRamek){
            przydzial.get(liczbaProcesow-1).add(marker);
            marker++;
        }

        ArrayList<Integer> historia = new ArrayList<>();
        ArrayList<Integer> wolneStrony = new ArrayList<>();
        boolean[] wstrzymane = new boolean[liczbaProcesow];

        for (int requestNumber=0; requestNumber<requests.size(); requestNumber++){
            for (int i=0; i<liczbaProcesow; i++){
                int e=0;
                for (int j=historia.size()-1; j>=Math.max(0, historia.size()-1-t); j--){
                    if (historia.get(j)==i){
                        e++;
                    }
                }
                if (e<l && przydzial.get(i).size()>1){
                    wolneStrony.add(przydzial.get(i).get(0));
                    przydzial.get(i).remove(0);
                }
            }
            for (int i=0; i<liczbaProcesow; i++){
                int e=0;
                for (int j=historia.size()-1; j>=Math.max(0, historia.size()-1-t); j--){
                    if (historia.get(j)==i){
                        e++;
                    }
                }
                if (e>h){
                    if (wolneStrony.size()>0){
                        przydzial.get(i).add(wolneStrony.get(0));
                        wolneStrony.remove(0);
                        wstrzymane[i]=false;
                    }else{
                        wstrzymane[i]=true;
                    }
                }else{
                    wstrzymane[i]=false;
                }
            }

            int pom=requestNumber;
            while (pom<requests.size() && wstrzymane[requests.get(pom).getProcess()]){
                pom++;
            }
            if (pom==requests.size()){
                historia.add(-1);
                continue;
            }
            if (pom!=requestNumber){
                Request swap = requests.get(requestNumber);
                requests.set(requestNumber, requests.get(pom));
                requests.set(pom, swap);
            }

            int process = requests.get(requestNumber).getProcess();
            if (LRU(requestNumber, requests, ramki, przydzial, process)){
                result[process]++;
                historia.add(process);
            }
            else{
                historia.add(-1);
            }
        }

        System.out.println("-----------------------------");
        System.out.println("\nSTEROWANIE CZESTOSCIA\n");

        int suma=0;
        System.out.println("Liczba błędów strony:\n");
        for (int i=0; i<liczbaProcesow; i++){
            System.out.println("Proces "+(i+1)+": "+result[i]);
            suma+=result[i];
        }
        System.out.println("\nSuma = "+suma);
    }

    public static void modelStrefowy(ArrayList<Request> req, int liczbaRamek, int liczbaProcesow, int t){
        //tablica wynikow
        int[] result = new int[liczbaProcesow];
        //lista 2d przydzielonych ramek dla kazdego procesu
        ArrayList<ArrayList<Integer>> przydzial = new ArrayList<>();
        //inicjalizacja
        for (int i=0; i<liczbaProcesow; i++){
            przydzial.add(new ArrayList<>());
        }
        //tablica ramek
        int[] ramki = new int[liczbaRamek];
        Arrays.fill(ramki, -1);

        //przepisanie requestow
        ArrayList<Request> requests = new ArrayList<>();
        for (Request r : req){
            requests.add(new Request(r.getProcess(), r.getPage()));
        }

        //lista wolnych stron
        ArrayList<Integer> wolneStrony = new ArrayList<>();
        for (int i=0; i<liczbaRamek; i++){
            wolneStrony.add(i);
        }

        //znaczniki czy wstrzymane
        boolean[] wstrzymane = new boolean[liczbaProcesow];

        //tablica liczonych wss
        int[] wss = new int[liczbaProcesow];

        //przydzial po jednej ramce
        for (int i=0; i<liczbaProcesow; i++){
            przydzial.get(i).add(wolneStrony.get(0));
            wolneStrony.remove(0);
        }

        for (int requestNumber=0; requestNumber<requests.size(); requestNumber++){
            //czynnosci organizacyjne co czas c = 1/2 t
            if (requestNumber % (t/2) == 0) {
                //obliczenie wss w oknie t
                for (int i = 0; i < liczbaProcesow; i++) {
                    HashSet<Integer> pom = new HashSet<>();
                    for (int j = requestNumber; j >= Math.max(0, requestNumber - t); j--) {
                        if (requests.get(j).getProcess() == i) {
                            pom.add(requests.get(j).getPage());
                        }
                    }
                    wss[i] = pom.size();
                }

                //obliczenie D oraz znalezienie procesu o max wss (do wstrzymania)

                int suma = 0;
                int suma_global = 0;
                int wss_max = wss[0];
                int wss_max_proces = 0;

                for (int i = 0; i < liczbaProcesow; i++) {
                    if (wss[i] > wss_max) {
                        wss_max = wss[i];
                        wss_max_proces = i;
                    }

                    if (!wstrzymane[i]) {
                        suma += wss[i];
                    }
                    suma_global += wss[i];
                }

                //wstrzymanie procesu o wss max
                if (suma > liczbaRamek) {
                    wstrzymane[wss_max_proces] = true;
                    wolneStrony.addAll(przydzial.get(wss_max_proces));
                    przydzial.set(wss_max_proces, new ArrayList<>());
                }

                //ponowne uruchomienie wstrzymanych
                if (suma_global <= liczbaRamek) {
                    Arrays.fill(wstrzymane, false);
                }

                //zwrot niepotrzebnych ramek do puli
                for (int i = 0; i < liczbaProcesow; i++) {
                    if (!wstrzymane[i]) {
                        while (przydzial.get(i).size() > wss[i] && przydzial.get(i).size()>1) {
                            wolneStrony.add(przydzial.get(i).get(0));
                            przydzial.get(i).remove(0);
                        }
                    }
                }

                //przydzial ramek
                for (int i = 0; i < liczbaProcesow; i++) {
                    if (!wstrzymane[i]) {
                        while (przydzial.get(i).size() < wss[i] && wolneStrony.size() > 0) {
                            przydzial.get(i).add(wolneStrony.get(0));
                            wolneStrony.remove(0);
                        }
                    }
                }
            }

            //pominiecie wstrzymanych i takich z zerowym przydzialem
            int pom=requestNumber;
            while (pom<requests.size() && (wstrzymane[requests.get(pom).getProcess()] || przydzial.get(requests.get(pom).getProcess()).size()==0)){
                pom++;
            }
            if (pom==requests.size()){
                continue;
            }
            if (pom!=requestNumber){
                Request swap = requests.get(requestNumber);
                requests.set(requestNumber, requests.get(pom));
                requests.set(pom, swap);
            }

            int process = requests.get(requestNumber).getProcess();
            if (LRU(requestNumber, requests, ramki, przydzial, process)){
                result[process]++;
            }
        }

        System.out.println("-----------------------------");
        System.out.println("\nMODEL STREFOWY\n");

        int suma=0;
        System.out.println("Liczba błędów strony:\n");
        for (int i=0; i<liczbaProcesow; i++){
            System.out.println("Proces "+(i+1)+": "+result[i]);
            suma+=result[i];
        }
        System.out.println("\nSuma = "+suma);
    }



    private static boolean LRU(int requestNumber, ArrayList<Request> requests, int[] ramki, ArrayList<ArrayList<Integer>> przydzial, int process){
        int strona=requests.get(requestNumber).getPage();

        for (int i: przydzial.get(process)){
            if (ramki[i]==strona){
                return false;
            }
        }

        for (int i: przydzial.get(process)){
            if (ramki[i]==-1){
                ramki[i]=strona;
                return true;
            }
        }
        int ramkaMin=przydzial.get(process).get(0);
        int timeMin=requestNumber+1;
        for (int i: przydzial.get(process)){
            int stronaTemp = ramki[i];
            int j = requestNumber-1;
            while (requests.get(j).getPage()!=stronaTemp){
                j--;
            }
            if (j<timeMin){
                timeMin=j;
                ramkaMin=i;
            }
        }
        ramki[ramkaMin]=strona;
        return true;
    }
}
