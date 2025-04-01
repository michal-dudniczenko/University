import java.util.ArrayList;
import java.util.Comparator;

public class Algorytmy {

    public static void FCFS(ArrayList<Proces> listaStartowa, double[] wyniki, int czasSymulacji){
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : listaStartowa)wszystkie.add(new Proces(p.getNumer(), p.getDlugoscFazy(), p.getCzasZgloszenia()));
        wszystkie.sort(new Comparator<Proces>(){
            @Override
            public int compare(Proces p1, Proces p2){
                return p1.getCzasZgloszenia()-p2.getCzasZgloszenia();
            }
        });

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> zakonczone = new ArrayList<>();
        Proces wykonywany=null;

        int liczbaPrzelaczen=0;

        int czas = 0;
        while(czas<czasSymulacji){
            while ((wszystkie.size() > 0) && (wszystkie.get(0).getCzasZgloszenia() <= czas)){
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }
            if (wykonywany == null && oczekujace.size() > 0) {
                wykonywany = oczekujace.get(0);
                if (!wykonywany.isCzyRozpoczety()){
                    wykonywany.setCzasRozpoczecia(czas);
                }
                wykonywany.setCzyRozpoczety(true);
                oczekujace.remove(0);
            }
            if (wykonywany != null) {
                if (wykonywany.getPozostalyCzas() > 0) {
                    wykonywany.zmniejszPozostalyCzas();
                }
                if (wykonywany.getPozostalyCzas() == 0) {
                    wykonywany.setCzasZakonczenia(czas);
                    zakonczone.add(wykonywany);
                    wykonywany = null;
                }
            }
            for (Proces p : oczekujace) {
                p.zwiekszCzasOczekiwania();
            }
            czas++;
        }

        double sredniCzas=0;
        for (Proces p: zakonczone){
            sredniCzas+=p.getCzasOczekiwania();
        }
        for (Proces p:oczekujace){
            sredniCzas+=p.getCzasOczekiwania();
        }

        sredniCzas/= listaStartowa.size();

        double procentUkonczone = (double)zakonczone.size()/(double)listaStartowa.size()*100;

        int najdluzszyCzas=-1;
        for (Proces p : zakonczone){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }

        double sredniCzasOdRozpoczeciaDoZakonczenia=0;

        for (Proces p: zakonczone)sredniCzasOdRozpoczeciaDoZakonczenia+=(p.getCzasZakonczenia()-p.getCzasRozpoczecia()+1);

        sredniCzasOdRozpoczeciaDoZakonczenia/= zakonczone.size();

        int zaglodzone=0;
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>8000)zaglodzone++;
        }

        int liczbaRozpoczete=0;
        for (Proces p :oczekujace){
            if (p.isCzyRozpoczety()){
                liczbaRozpoczete++;
            }
        }

        wyniki[0]=wyniki[0]+sredniCzas;
        wyniki[1]=wyniki[1]+najdluzszyCzas;
        wyniki[2]=wyniki[2]+sredniCzasOdRozpoczeciaDoZakonczenia;
        wyniki[3]=wyniki[3]+liczbaPrzelaczen;
        wyniki[4]=wyniki[4]+zaglodzone;
        wyniki[5]=wyniki[5]+liczbaRozpoczete;
        wyniki[6]=wyniki[6]+procentUkonczone;
    }

    public static void SJFbez(ArrayList<Proces> listaStartowa, double[] wyniki, int czasSymulacji){
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : listaStartowa)wszystkie.add(new Proces(p.getNumer(), p.getDlugoscFazy(), p.getCzasZgloszenia()));
        wszystkie.sort(new Comparator<Proces>(){
            @Override
            public int compare(Proces p1, Proces p2){
                return p1.getCzasZgloszenia()-p2.getCzasZgloszenia();
            }
        });

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> zakonczone = new ArrayList<>();
        Proces wykonywany=null;

        int liczbaPrzelaczen=0;

        int czas = 0;
        while(czas<czasSymulacji){
            while ((wszystkie.size() > 0) && (wszystkie.get(0).getCzasZgloszenia() <= czas)){
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }
            oczekujace.sort(new Comparator<Proces>() {
                @Override
                public int compare(Proces p1, Proces p2) {
                    return p1.getDlugoscFazy() - p2.getDlugoscFazy();
                }
            });
            if (wykonywany == null && oczekujace.size() > 0) {
                wykonywany = oczekujace.get(0);
                if (!wykonywany.isCzyRozpoczety()){
                    wykonywany.setCzasRozpoczecia(czas);
                }
                wykonywany.setCzyRozpoczety(true);
                oczekujace.remove(0);
            }
            if (wykonywany != null) {
                if (wykonywany.getPozostalyCzas() > 0) {
                    wykonywany.zmniejszPozostalyCzas();
                }
                if (wykonywany.getPozostalyCzas() == 0) {
                    wykonywany.setCzasZakonczenia(czas);
                    zakonczone.add(wykonywany);
                    wykonywany = null;
                }
            }
            for (Proces p : oczekujace) {
                p.zwiekszCzasOczekiwania();
            }
            czas++;
        }

        double sredniCzas=0;
        for (Proces p: zakonczone){
            sredniCzas+=p.getCzasOczekiwania();
        }
        for (Proces p:oczekujace){
            sredniCzas+=p.getCzasOczekiwania();
        }

        sredniCzas/= listaStartowa.size();

        double procentUkonczone = (double)zakonczone.size()/(double)listaStartowa.size()*100;

        int najdluzszyCzas=-1;
        for (Proces p : zakonczone){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }

        double sredniCzasOdRozpoczeciaDoZakonczenia=0;

        for (Proces p: zakonczone)sredniCzasOdRozpoczeciaDoZakonczenia+=(p.getCzasZakonczenia()-p.getCzasRozpoczecia()+1);

        sredniCzasOdRozpoczeciaDoZakonczenia/= zakonczone.size();

        int zaglodzone=0;
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>8000)zaglodzone++;
        }

        int liczbaRozpoczete=0;
        for (Proces p :oczekujace){
            if (p.isCzyRozpoczety()){
                liczbaRozpoczete++;
            }
        }

        wyniki[0]=wyniki[0]+sredniCzas;
        wyniki[1]=wyniki[1]+najdluzszyCzas;
        wyniki[2]=wyniki[2]+sredniCzasOdRozpoczeciaDoZakonczenia;
        wyniki[3]=wyniki[3]+liczbaPrzelaczen;
        wyniki[4]=wyniki[4]+zaglodzone;
        wyniki[5]=wyniki[5]+liczbaRozpoczete;
        wyniki[6]=wyniki[6]+procentUkonczone;
    }

    public static void SJFz(ArrayList<Proces> listaStartowa, double[] wyniki, int czasSymulacji){
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : listaStartowa)wszystkie.add(new Proces(p.getNumer(), p.getDlugoscFazy(), p.getCzasZgloszenia()));
        wszystkie.sort(new Comparator<Proces>(){
            @Override
            public int compare(Proces p1, Proces p2){
                return p1.getCzasZgloszenia()-p2.getCzasZgloszenia();
            }
        });

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> zakonczone = new ArrayList<>();
        Proces wykonywany=null;

        int liczbaPrzelaczen=0;

        int czas = 0;
        while(czas<czasSymulacji){
            while ((wszystkie.size() > 0) && (wszystkie.get(0).getCzasZgloszenia() <= czas)){
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }
            oczekujace.sort(new Comparator<Proces>() {
                @Override
                public int compare(Proces p1, Proces p2) {
                    return p1.getDlugoscFazy() - p2.getDlugoscFazy();
                }
            });
            if ((wykonywany!=null)&&(oczekujace.size()>0)&&(oczekujace.get(0).getPozostalyCzas() < wykonywany.getPozostalyCzas())){
                Proces temp=wykonywany;
                wykonywany=oczekujace.get(0);
                if (!wykonywany.isCzyRozpoczety()){
                    wykonywany.setCzasRozpoczecia(czas);
                }
                wykonywany.setCzyRozpoczety(true);
                oczekujace.set(0, temp);
                liczbaPrzelaczen++;
                for (Proces p : oczekujace) {
                    p.zwiekszCzasOczekiwania();
                }
                czas++;
                continue;
            }

            if (wykonywany == null && oczekujace.size() > 0) {
                wykonywany = oczekujace.get(0);
                if (!wykonywany.isCzyRozpoczety()){
                    wykonywany.setCzasRozpoczecia(czas);
                }
                wykonywany.setCzyRozpoczety(true);
                oczekujace.remove(0);
            }
            if (wykonywany != null) {
                if (wykonywany.getPozostalyCzas() > 0) {
                    wykonywany.zmniejszPozostalyCzas();
                }
                if (wykonywany.getPozostalyCzas() == 0) {
                    wykonywany.setCzasZakonczenia(czas);
                    zakonczone.add(wykonywany);
                    wykonywany = null;
                }
            }
            for (Proces p : oczekujace) {
                p.zwiekszCzasOczekiwania();
            }
            czas++;
        }

        double sredniCzas=0;
        for (Proces p: zakonczone){
            sredniCzas+=p.getCzasOczekiwania();
        }
        for (Proces p:oczekujace){
            sredniCzas+=p.getCzasOczekiwania();
        }

        sredniCzas/= listaStartowa.size();

        double procentUkonczone = (double)zakonczone.size()/(double)listaStartowa.size()*100;

        int najdluzszyCzas=-1;
        for (Proces p : zakonczone){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }

        double sredniCzasOdRozpoczeciaDoZakonczenia=0;

        for (Proces p: zakonczone)sredniCzasOdRozpoczeciaDoZakonczenia+=(p.getCzasZakonczenia()-p.getCzasRozpoczecia()+1);

        sredniCzasOdRozpoczeciaDoZakonczenia/= zakonczone.size();

        int zaglodzone=0;
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>8000)zaglodzone++;
        }

        int liczbaRozpoczete=0;
        for (Proces p :oczekujace){
            if (p.isCzyRozpoczety()){
                liczbaRozpoczete++;
            }
        }

        wyniki[0]=wyniki[0]+sredniCzas;
        wyniki[1]=wyniki[1]+najdluzszyCzas;
        wyniki[2]=wyniki[2]+sredniCzasOdRozpoczeciaDoZakonczenia;
        wyniki[3]=wyniki[3]+liczbaPrzelaczen;
        wyniki[4]=wyniki[4]+zaglodzone;
        wyniki[5]=wyniki[5]+liczbaRozpoczete;
        wyniki[6]=wyniki[6]+procentUkonczone;
    }

    public static void RR(ArrayList<Proces> listaStartowa, double[] wyniki, int czasSymulacji, int kwant){
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : listaStartowa)wszystkie.add(new Proces(p.getNumer(), p.getDlugoscFazy(), p.getCzasZgloszenia()));
        wszystkie.sort(new Comparator<Proces>(){
            @Override
            public int compare(Proces p1, Proces p2){
                return p1.getCzasZgloszenia()-p2.getCzasZgloszenia();
            }
        });

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> zakonczone = new ArrayList<>();
        Proces wykonywany=null;

        int liczbaPrzelaczen=0;

        int licznikKwantu=0;

        int czas = 0;
        while(czas<czasSymulacji){
            if (licznikKwantu<kwant) {
                while ((wszystkie.size() > 0) && (wszystkie.get(0).getCzasZgloszenia() <= czas)) {
                    oczekujace.add(wszystkie.get(0));
                    wszystkie.remove(0);
                }
                if (wykonywany == null && oczekujace.size() > 0) {
                    wykonywany = oczekujace.get(0);
                    if (!wykonywany.isCzyRozpoczety()) {
                        wykonywany.setCzasRozpoczecia(czas);
                    }
                    wykonywany.setCzyRozpoczety(true);
                    oczekujace.remove(0);
                }
                if (wykonywany != null) {
                    if (wykonywany.getPozostalyCzas() > 0) {
                        wykonywany.zmniejszPozostalyCzas();
                    }
                    if (wykonywany.getPozostalyCzas() == 0) {
                        wykonywany.setCzasZakonczenia(czas);
                        zakonczone.add(wykonywany);
                        wykonywany = null;
                    }
                }
                for (Proces p : oczekujace) {
                    p.zwiekszCzasOczekiwania();
                }
                czas++;
                licznikKwantu++;
            }else{
                licznikKwantu=0;
                if((wykonywany==null)&&(oczekujace.size()>0)){
                    wykonywany = oczekujace.get(0);
                    if (!wykonywany.isCzyRozpoczety()) {
                        wykonywany.setCzasRozpoczecia(czas);
                    }
                    wykonywany.setCzyRozpoczety(true);
                    oczekujace.remove(0);
                    if (wykonywany.getPozostalyCzas() > 0) {
                        wykonywany.zmniejszPozostalyCzas();
                    }
                    if (wykonywany.getPozostalyCzas() == 0) {
                        wykonywany.setCzasZakonczenia(czas);
                        zakonczone.add(wykonywany);
                        wykonywany = null;
                    }
                } else if ((wykonywany != null)&&(oczekujace.size()>0)) {
                    Proces temp = wykonywany;
                    wykonywany=oczekujace.get(0);
                    if (!wykonywany.isCzyRozpoczety()) {
                        wykonywany.setCzasRozpoczecia(czas);
                    }
                    wykonywany.setCzyRozpoczety(true);
                    oczekujace.remove(0);
                    oczekujace.add(temp);
                    liczbaPrzelaczen++;
                }
                for (Proces p : oczekujace) {
                    p.zwiekszCzasOczekiwania();
                }
                czas++;
                licznikKwantu++;
            }
        }

        double sredniCzas=0;
        for (Proces p: zakonczone){
            sredniCzas+=p.getCzasOczekiwania();
        }
        for (Proces p:oczekujace){
            sredniCzas+=p.getCzasOczekiwania();
        }

        sredniCzas/= listaStartowa.size();

        double procentUkonczone = (double)zakonczone.size()/(double)listaStartowa.size()*100;

        int najdluzszyCzas=-1;
        for (Proces p : zakonczone){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>najdluzszyCzas)najdluzszyCzas=p.getCzasOczekiwania();
        }

        double sredniCzasOdRozpoczeciaDoZakonczenia=0;

        for (Proces p: zakonczone)sredniCzasOdRozpoczeciaDoZakonczenia+=(p.getCzasZakonczenia()-p.getCzasRozpoczecia()+1);

        sredniCzasOdRozpoczeciaDoZakonczenia/= zakonczone.size();

        int zaglodzone=0;
        for (Proces p : oczekujace){
            if (p.getCzasOczekiwania()>8000)zaglodzone++;
        }

        int liczbaRozpoczete=0;
        for (Proces p :oczekujace){
            if (p.isCzyRozpoczety()){
                liczbaRozpoczete++;
            }
        }

        wyniki[0]=wyniki[0]+sredniCzas;
        wyniki[1]=wyniki[1]+najdluzszyCzas;
        wyniki[2]=wyniki[2]+sredniCzasOdRozpoczeciaDoZakonczenia;
        wyniki[3]=wyniki[3]+liczbaPrzelaczen;
        wyniki[4]=wyniki[4]+zaglodzone;
        wyniki[5]=wyniki[5]+liczbaRozpoczete;
        wyniki[6]=wyniki[6]+procentUkonczone;
    }
}
