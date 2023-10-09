package Backend;

import Frontend.Menu;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.util.ArrayList;

public class Main{
    public static ArrayList<Osoba> osoby = new ArrayList<Osoba>();
    public static ArrayList<Kursy> kursy = new ArrayList<Kursy>();
    public static String wiadomoscUczelni = "Brak ogłoszeń";

    public static ArrayList<Osoba> getOsoby(){
        return osoby;
    }
    public static ArrayList<Kursy> getKursy(){
        return kursy;
    }
    public static void setOsoby(ArrayList<Osoba> osoby) {
        Main.osoby = osoby;
    }
    public static void setKursy(ArrayList<Kursy> kursy) {
        Main.kursy = kursy;
    }
    public static String getWiadomoscUczelni() {
        return wiadomoscUczelni;
    }
    public static void setWiadomoscUczelni(String wiadomoscUczelni) {
        Main.wiadomoscUczelni = wiadomoscUczelni;
    }

    public static void main(String[] args){
        //deserializacja
        File osobySer = new File("osoby.txt");
        File kursySer = new File("kursy.txt");
        try{
            if (osobySer.length()!=0) {
                ObjectInputStream ois = new ObjectInputStream(new FileInputStream(osobySer));
                Main.setOsoby((ArrayList) ois.readObject());
                ois.close();
            }
            if (kursySer.length()!=0) {
                ObjectInputStream ois = new ObjectInputStream(new FileInputStream(kursySer));
                Main.setKursy((ArrayList) ois.readObject());
                ois.close();
            }
        } catch (IOException | ClassNotFoundException exc) {
            exc.printStackTrace();
        }

        new Menu();
    }
}
