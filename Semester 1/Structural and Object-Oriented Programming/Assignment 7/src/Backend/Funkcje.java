package Backend;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashSet;

public class Funkcje implements Observable{
    public static ArrayList<Osoba> dodajOsobe(ArrayList<Osoba> osoby, Osoba o){
        osoby.add(o);
        return osoby;
    }
    public static ArrayList<Kursy> dodajKurs(ArrayList<Kursy> kursy, Kursy k){
        kursy.add(k);
        return kursy;
    }

    public static ArrayList<Osoba> wyszukajPracownika(ArrayList<Osoba> osoby, String filtr, String fraza){
        ArrayList<Osoba> wyniki = new ArrayList<Osoba>();
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getNazwisko().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getImie().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStanowisko().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getStazPracy()==Integer.parseInt(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "5":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikAdministracji && o.getLiczbaNadgodzin()==Integer.parseInt(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "6":{
                for (Osoba o : osoby){
                    if (o instanceof PracownikUczelni && o.getPensja()==Integer.parseInt(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
        }
        return wyniki;
    }
    public static ArrayList<Osoba> wyszukajStudenta(ArrayList<Osoba> osoby, String filtr, String fraza){
        ArrayList<Osoba> wyniki = new ArrayList<Osoba>();
        switch(filtr){
            case "1":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNazwisko().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "2":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getImie().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "3":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getNrIndeksu().equals(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "4":{
                for (Osoba o : osoby){
                    if (o instanceof Student && o.getRokStudiow()==Integer.parseInt(fraza)){
                        wyniki.add(o);
                    }
                }
                break;
            }
            case "5":{
                for (Osoba o : osoby){
                    if (o instanceof Student){
                        for (Kursy k : o.getListaKursow()){
                            if (k.getNazwaKursu().equals(fraza)){
                                wyniki.add(o);
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }
        return wyniki;
    }
    public static ArrayList<Kursy> wyszukajKurs(ArrayList<Kursy> kursy, String filtr, String fraza){
        ArrayList<Kursy> wyniki = new ArrayList<Kursy>();
        switch(filtr){
            case "1":{
                for (Kursy k : kursy){
                    if (k.getNazwaKursu().equals(fraza)){
                        wyniki.add(k);
                    }
                }
                break;
            }
            case "2":{
                for (Kursy k : kursy){
                    if (k.getNazwiskoProwadzacego().equals(fraza)){
                        wyniki.add(k);
                    }
                }
                break;
            }
            case "3":{
                for (Kursy k : kursy){
                    if (k.getECTS()==Integer.parseInt(fraza)){
                        wyniki.add(k);
                    }
                }
                break;
            }
        }
        return wyniki;
    }

    public static ArrayList<Osoba> sortujOsoby(ArrayList<Osoba> osoby, String rodzaj){
        switch(rodzaj){
            case "1":{
                Collections.sort(osoby, new SortByNazwisko());
                break;
            }
            case "2":{
                Collections.sort(osoby, new SortByNazwiskoImie());
                break;
            }
            case "3":{
                Collections.sort(osoby, new SortByNazwiskoWiek());
                break;
            }
        }
        return osoby;
    }
    public static ArrayList<Kursy> sortujKursy(ArrayList<Kursy> kursy){
        Collections.sort(kursy, new SortByECTSNazwisko());
        return kursy;
    }

    public static ArrayList<Osoba> usunPracownikow(ArrayList<Osoba> osoby, String filtr, String fraza){
        switch(filtr){
            case "1":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof PracownikUczelni && osoby.get(i).getNazwisko().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "2":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof PracownikUczelni && osoby.get(i).getImie().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "3":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof PracownikUczelni && osoby.get(i).getStazPracy()==Integer.parseInt(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "4":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof PracownikUczelni && osoby.get(i).getStanowisko().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
        }
        return osoby;
    }
    public static ArrayList<Osoba> usunStudentow(ArrayList<Osoba> osoby, String filtr, String fraza){
        switch(filtr){
            case "1":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof Student && osoby.get(i).getNazwisko().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "2":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof Student && osoby.get(i).getImie().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "3":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof Student && osoby.get(i).getRokStudiow()==Integer.parseInt(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
            case "4":{
                for (int i=0; i<osoby.size(); i++){
                    if (osoby.get(i) instanceof Student && osoby.get(i).getNrIndeksu().equals(fraza)){
                        osoby.remove(i);
                    }
                }
                break;
            }
        }
        return osoby;
    }
    public static ArrayList<Kursy> usunKursy(ArrayList<Kursy> kursy, String filtr, String fraza){
        switch(filtr){
            case "1":{
                for (int i=0; i<kursy.size(); i++){
                    if (kursy.get(i).getNazwiskoProwadzacego().equals(fraza)){
                        kursy.remove(i);
                    }
                }
                break;
            }
            case "2":{
                for (int i=0; i<kursy.size(); i++){
                    if (kursy.get(i).getECTS()==Integer.parseInt(fraza)){
                        kursy.remove(i);
                    }
                }
                break;
            }
        }
        return kursy;
    }

    public static ArrayList<Osoba> czyszczenie(ArrayList<Osoba> osoby, String sposob){
        switch(sposob){
            case "1":{
                osoby=Czyszczenie1.SposobNaCzyszczenie(osoby);
                break;
            }
            case "2":{
                osoby=Czyszczenie2.SposobNaCzyszczenie(osoby);
                break;
            }
        }
        return osoby;
    }

    public void notifyObservers(String wiadomosc){
        Main.setWiadomoscUczelni(wiadomosc);
        for (int i=0; i<Main.getOsoby().size();i++){
            if (Main.getOsoby().get(i) instanceof Student){
                Main.getOsoby().get(i).update();
            }
        }
    }

    public static ArrayList<Osoba> usunDuplikaty(ArrayList<Osoba> osoby){
        return new ArrayList<Osoba>(new HashSet<Osoba>(osoby));
    }
}
