import java.util.ArrayList;
import java.util.Comparator;

public class Algorytmy {
    public static void FCFS(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();
        Proces obecny = null;

        int obecnyCel = -1;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while (gotowe.size() < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            if (glowica == obecnyCel) {
                obecnyCel = -1;
                gotowe.add(obecny);
                oczekujace.remove(obecny);
                obecny = null;
            }

            if (obecnyCel == -1 && oczekujace.size() > 0) {
                obecny = oczekujace.get(0);
                obecnyCel = obecny.getBlok();
            }
            if (obecnyCel != -1) {
                if (glowica < obecnyCel) {
                    glowica++;
                    licznik++;
                } else if (glowica > obecnyCel) {
                    glowica--;
                    licznik++;
                }
            }

            czas++;
        }
        wyniki[0] += licznik;
    }

    public static void SSTF(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();
        Proces obecny = null;

        int obecnyCel = -1;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while (gotowe.size() < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            int finalGlowica = glowica;
            oczekujace.sort(new Comparator<>() {
                public int compare(Proces p1, Proces p2) {
                    return Math.abs(p1.getBlok() - finalGlowica) - Math.abs(p2.getBlok() - finalGlowica);
                }
            });

            if (glowica == obecnyCel) {
                obecnyCel = -1;
                gotowe.add(obecny);
                oczekujace.remove(obecny);
                obecny = null;
            }

            if (obecnyCel == -1 && oczekujace.size() > 0) {
                obecny = oczekujace.get(0);
                obecnyCel = obecny.getBlok();
            }

            if (obecnyCel != -1) {
                if (glowica < obecnyCel) {
                    glowica++;
                    licznik++;
                } else if (glowica > obecnyCel) {
                    glowica--;
                    licznik++;
                }
            }

            czas++;
        }
        wyniki[1] += licznik;
    }

    public static void SCAN(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();

        int obecnyCel = 1024;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while (gotowe.size() < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            if (glowica == obecnyCel) {
                if (obecnyCel == 1024) {
                    obecnyCel = 1;
                } else {
                    obecnyCel = 1024;
                }
            }

            for (int i = oczekujace.size() - 1; i >= 0; i--) {
                if (oczekujace.get(i).getBlok() == glowica) {
                    gotowe.add(oczekujace.get(i));
                    oczekujace.remove(i);
                }
            }

            if (glowica < obecnyCel) {
                glowica++;
            } else {
                glowica--;
            }

            licznik++;
            czas++;
        }
        wyniki[2] += licznik;
    }

    public static void CSCAN(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();

        int obecnyCel = 1024;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while (gotowe.size() < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            for (int i = oczekujace.size() - 1; i >= 0; i--) {
                if (oczekujace.get(i).getBlok() == glowica) {
                    gotowe.add(oczekujace.get(i));
                    oczekujace.remove(i);
                }
            }

            if (glowica == obecnyCel) {
                glowica = 1;
            }
            else if (glowica < obecnyCel) {
                glowica++;
            }

            licznik++;
            czas++;
        }
        wyniki[3] += licznik;
    }

    public static void SSTF_EDF(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();
        Proces obecny = null;
        ArrayList<Proces> nieudane = new ArrayList<>();

        int obecnyCel = -1;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while ((gotowe.size()+nieudane.size()) < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            int finalGlowica = glowica;
            oczekujace.sort(new Comparator<>() {
                public int compare(Proces p1, Proces p2) {
                    if (p1.isRealtime()&& !p2.isRealtime()){
                        return -1;
                    }else if (!p1.isRealtime()&&p2.isRealtime()){
                        return 1;
                    }else if(p1.isRealtime()&&p2.isRealtime()){
                        return p1.getDeadline()-p2.getDeadline();
                    }else{
                        return Math.abs(p1.getBlok() - finalGlowica) - Math.abs(p2.getBlok() - finalGlowica);
                    }
                }
            });

            if (obecnyCel!=-1 && obecny.isRealtime() && czas>obecny.getDeadline()){
                obecnyCel = -1;
                nieudane.add(obecny);
                oczekujace.remove(obecny);
                obecny=null;
            }

            if (glowica == obecnyCel) {
                obecnyCel = -1;
                gotowe.add(obecny);
                oczekujace.remove(obecny);
                obecny = null;
            }

            if (obecnyCel == -1 && oczekujace.size() > 0) {
                obecny = oczekujace.get(0);
                obecnyCel = obecny.getBlok();
            }

            if (obecnyCel != -1) {
                if (glowica < obecnyCel) {
                    glowica++;
                    licznik++;
                } else if (glowica > obecnyCel) {
                    glowica--;
                    licznik++;
                }
            }

            czas++;
        }
        wyniki[4] += licznik;
        wyniki[5] += (10-(double)nieudane.size())/10;
    }


    public static void SSTF_FDSCAN(ArrayList<Proces> procesy, double[] wyniki) {
        ArrayList<Proces> wszystkie = new ArrayList<>();
        for (Proces p : procesy) {
            if (p.isRealtime())
                wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia(), p.isRealtime(), p.getDeadline()));
            else wszystkie.add(new Proces(p.getNumer(), p.getBlok(), p.getCzasZgloszenia()));
        }

        ArrayList<Proces> oczekujace = new ArrayList<>();
        ArrayList<Proces> gotowe = new ArrayList<>();
        Proces obecny = null;
        ArrayList<Proces> nieudane = new ArrayList<>();

        int obecnyCel = -1;
        int licznik = 0;
        int glowica = 512;

        int czas = 0;
        while ((gotowe.size()+nieudane.size()) < procesy.size()) {
            while (wszystkie.size() > 0 && wszystkie.get(0).getCzasZgloszenia() <= czas) {
                oczekujace.add(wszystkie.get(0));
                wszystkie.remove(0);
            }

            int finalGlowica = glowica;
            oczekujace.sort(new Comparator<>() {
                public int compare(Proces p1, Proces p2) {
                    if (p1.isRealtime()&& !p2.isRealtime()){
                        return -1;
                    }else if (!p1.isRealtime()&&p2.isRealtime()){
                        return 1;
                    }else if(p1.isRealtime()&&p2.isRealtime()){
                        return p1.getDeadline()-p2.getDeadline();
                    }else{
                        return Math.abs(p1.getBlok() - finalGlowica) - Math.abs(p2.getBlok() - finalGlowica);
                    }
                }
            });

            if (glowica == obecnyCel) {
                obecnyCel = -1;
                gotowe.add(obecny);
                oczekujace.remove(obecny);
                obecny = null;
            }

            if(obecny!=null && obecny.isRealtime()) {
                for (int i = oczekujace.size() - 1; i >= 0; i--) {
                    if (oczekujace.get(i).getBlok() == glowica) {
                        gotowe.add(oczekujace.get(i));
                        oczekujace.remove(i);
                    }
                }
            }

            while(obecnyCel == -1 && oczekujace.size() > 0){
                obecny = oczekujace.get(0);
                obecnyCel = obecny.getBlok();
                if(obecny.isRealtime()&&obecny.getDeadline()<(czas+Math.abs(glowica-obecny.getBlok()))){
                    nieudane.add(obecny);
                    oczekujace.remove(obecny);
                    obecny=null;
                    obecnyCel=-1;
                }
            }

            if (obecnyCel != -1) {
                if (glowica < obecnyCel) {
                    glowica++;
                    licznik++;
                } else if (glowica > obecnyCel) {
                    glowica--;
                    licznik++;
                }
            }

            czas++;
        }
        wyniki[6] += licznik;
        wyniki[7] += (10-(double)nieudane.size())/10;
    }
}
