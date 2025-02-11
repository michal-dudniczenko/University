import java.util.ArrayList;
import java.util.Random;

public class Algorytmy {
    public static void Strategia1(ArrayList<Proces> procesyOrg, int N, int p, int z, int[] pom){
        Random rnd = new Random();

        Procesor[] procesory = new Procesor[N];
        for (int i=0; i<N; i++){
            procesory[i]=new Procesor();
        }

        ArrayList<Proces> procesy = new ArrayList<>();
        for (Proces pr : procesyOrg){
            procesy.add(new Proces(pr.getCzasWykonania(), pr.getObciazenieProcesora(), pr.getDocelowyProcesor()));
        }

        int licznikZapytan=0;
        int licznikPrzemieszczen=0;

        int czas=0;
        while(czas<1000){
            int liczbaProcesow = pom[czas];
            for (int i=0; i<liczbaProcesow; i++){
                Proces proces = procesy.get(0);
                procesy.remove(0);

                boolean flaga=false;
                int licznikLosowan=0;
                int numerProcesora;
                while (licznikLosowan<z){
                    numerProcesora=rnd.nextInt(N);
                    if (numerProcesora==proces.getDocelowyProcesor()){
                        continue;
                    }
                    licznikZapytan++;
                    if (procesory[numerProcesora].getObciazenie()<p){
                        licznikPrzemieszczen++;
                        procesory[numerProcesora].addProces(proces);
                        flaga=true;
                        break;
                    }
                    licznikLosowan++;
                }
                if (!flaga){
                    procesory[proces.getDocelowyProcesor()].addProces(proces);
                }
            }

            for (Procesor p1 : procesory){
                int i=0;
                int limit=p1.getProcesy().size();
                while (i<limit){
                    Proces proces = p1.getProcesy().get(i);
                    if (proces.getCzasWykonania()==0){
                        p1.removeProces(proces);
                        i--;
                        limit--;
                    }
                    else{
                        proces.zmniejszCzas();
                    }
                    i++;
                }
                p1.updateHistoria(czas);
            }

            czas++;
        }

        double srednieObciazenie=0;
        for (Procesor pr : procesory){
            srednieObciazenie+=pr.getSrednieObciazenie();
        }
        srednieObciazenie/=N;

        double srednieOdchylenie=0;
        for (Procesor pr : procesory){
            srednieOdchylenie+=Math.abs(pr.getSrednieObciazenie()-srednieObciazenie);
        }
        srednieOdchylenie/=N;

        System.out.println("--------------------------------------------------");
        System.out.println("Strategia 1:");
        System.out.print("Średnie obciążenie procesorów = ");
        System.out.printf("%.2f", srednieObciazenie);
        System.out.print("\nŚrednie odchylenie = ");
        System.out.printf("%.2f", srednieOdchylenie);
        System.out.println("\nLiczba zapytań o obciążenie = "+licznikZapytan);
        System.out.println("Liczba przemieszczeń = "+licznikPrzemieszczen);
    }

    public static void Strategia2(ArrayList<Proces> procesyOrg, int N, int p, int[] pom){
        Random rnd = new Random();

        Procesor[] procesory = new Procesor[N];
        for (int i=0; i<N; i++){
            procesory[i]=new Procesor();
        }

        ArrayList<Proces> procesy = new ArrayList<>();
        for (Proces pr : procesyOrg){
            procesy.add(new Proces(pr.getCzasWykonania(), pr.getObciazenieProcesora(), pr.getDocelowyProcesor()));
        }

        int licznikZapytan=0;
        int licznikPrzemieszczen=0;

        int czas=0;
        while(czas<1000){
            int liczbaProcesow = pom[czas];
            for (int i=0; i<liczbaProcesow; i++){
                Proces proces = procesy.get(0);
                procesy.remove(0);
                licznikZapytan++;
                if (procesory[proces.getDocelowyProcesor()].getObciazenie()>p){
                    boolean flaga=false;
                    int numerProcesora;
                    int safeLimit=1000;
                    while (safeLimit>0){
                        numerProcesora=rnd.nextInt(N);
                        licznikZapytan++;
                        if (procesory[numerProcesora].getObciazenie()<p){
                            procesory[numerProcesora].addProces(proces);
                            licznikPrzemieszczen++;
                            flaga=true;
                            break;
                        }
                        safeLimit--;
                    }
                    if (!flaga){
                        procesory[proces.getDocelowyProcesor()].addProces(proces);
                    }
                }else{
                    procesory[proces.getDocelowyProcesor()].addProces(proces);
                }

            }

            for (Procesor p1 : procesory){
                int i=0;
                int limit=p1.getProcesy().size();
                while (i<limit){
                    Proces proces = p1.getProcesy().get(i);
                    if (proces.getCzasWykonania()==0){
                        p1.removeProces(proces);
                        i--;
                        limit--;
                    }
                    else{
                        proces.zmniejszCzas();
                    }
                    i++;
                }
                p1.updateHistoria(czas);
            }

            czas++;
        }

        double srednieObciazenie=0;
        for (Procesor pr : procesory){
            srednieObciazenie+=pr.getSrednieObciazenie();
        }
        srednieObciazenie/=N;

        double srednieOdchylenie=0;
        for (Procesor pr : procesory){
            srednieOdchylenie+=Math.abs(pr.getSrednieObciazenie()-srednieObciazenie);
        }
        srednieOdchylenie/=N;

        System.out.println("--------------------------------------------------");
        System.out.println("Strategia 2:");
        System.out.print("Średnie obciążenie procesorów = ");
        System.out.printf("%.2f", srednieObciazenie);
        System.out.print("\nŚrednie odchylenie = ");
        System.out.printf("%.2f", srednieOdchylenie);
        System.out.println("\nLiczba zapytań o obciążenie = "+licznikZapytan);
        System.out.println("Liczba przemieszczeń = "+licznikPrzemieszczen);
    }

    public static void Strategia3(ArrayList<Proces> procesyOrg, int N, int r, int p, int[] pom){
        Random rnd = new Random();

        Procesor[] procesory = new Procesor[N];
        for (int i=0; i<N; i++){
            procesory[i]=new Procesor();
        }

        ArrayList<Proces> procesy = new ArrayList<>();
        for (Proces pr : procesyOrg){
            procesy.add(new Proces(pr.getCzasWykonania(), pr.getObciazenieProcesora(), pr.getDocelowyProcesor()));
        }

        int licznikZapytan=0;
        int licznikPrzemieszczen=0;

        int czas=0;
        while(czas<1000){
            for (Procesor pr : procesory){
                if (pr.getObciazenie()<r){
                    int numerProcesora = rnd.nextInt(N);
                    while (procesory[numerProcesora]==pr){
                        numerProcesora=rnd.nextInt(N);
                    }
                    Procesor pr2 = procesory[numerProcesora];
                    licznikZapytan++;
                    if (pr2.getObciazenie()>p){
                        int liczbaPrzejetych = pr2.getProcesy().size()*(pr2.getObciazenie()-p)/100;
                        for (int j=0; j<liczbaPrzejetych; j++){
                            pr.addProces(pr2.getProcesy().get(0));
                            pr2.getProcesy().remove(0);
                            licznikPrzemieszczen++;
                        }
                    }
                }
            }

            int liczbaProcesow = pom[czas];
            for (int i=0; i<liczbaProcesow; i++){
                Proces proces = procesy.get(0);
                procesy.remove(0);
                licznikZapytan++;
                if (procesory[proces.getDocelowyProcesor()].getObciazenie()>p){
                    boolean flaga=false;
                    int numerProcesora;
                    int safeLimit=1000;
                    while (safeLimit>0){
                        numerProcesora=rnd.nextInt(N);
                        licznikZapytan++;
                        if (procesory[numerProcesora].getObciazenie()<p){
                            procesory[numerProcesora].addProces(proces);
                            licznikPrzemieszczen++;
                            flaga=true;
                            break;
                        }
                        safeLimit--;
                    }
                    if (!flaga){
                        procesory[proces.getDocelowyProcesor()].addProces(proces);
                    }
                }else{
                    procesory[proces.getDocelowyProcesor()].addProces(proces);
                }

            }

            for (Procesor p1 : procesory){
                int i=0;
                int limit=p1.getProcesy().size();
                while (i<limit){
                    Proces proces = p1.getProcesy().get(i);
                    if (proces.getCzasWykonania()==0){
                        p1.removeProces(proces);
                        i--;
                        limit--;
                    }
                    else{
                        proces.zmniejszCzas();
                    }
                    i++;
                }
                p1.updateHistoria(czas);
            }

            czas++;
        }

        double srednieObciazenie=0;
        for (Procesor pr : procesory){
            srednieObciazenie+=pr.getSrednieObciazenie();
        }
        srednieObciazenie/=N;

        double srednieOdchylenie=0;
        for (Procesor pr : procesory){
            srednieOdchylenie+=Math.abs(pr.getSrednieObciazenie()-srednieObciazenie);
        }
        srednieOdchylenie/=N;

        System.out.println("--------------------------------------------------");
        System.out.println("Strategia 3:");
        System.out.print("Średnie obciążenie procesorów = ");
        System.out.printf("%.2f", srednieObciazenie);
        System.out.print("\nŚrednie odchylenie = ");
        System.out.printf("%.2f", srednieOdchylenie);
        System.out.println("\nLiczba zapytań o obciążenie = "+licznikZapytan);
        System.out.println("Liczba przemieszczeń = "+licznikPrzemieszczen);
    }
}
