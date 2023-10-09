import java.io.*;
import java.util.Scanner;

public class Main {
    public static void main(String args[]){
        Scanner input = new Scanner(System.in);
        System.out.println("Podaj nazwę pliku tekstowego:");
        File f = new File(input.nextLine()+".txt");
        ListaStudentow lista = new ListaStudentow();
        try {
            BufferedReader br = new BufferedReader(new FileReader(f));
            String line;
            int licznik=0;
            while ((line = br.readLine()) != null){
                if (licznik>1){
                    String[] pom = line.split(" ");
                    double srednia = 0;
                    for (int i=3; i<pom.length; i++){
                        srednia+=Double.parseDouble(pom[i]);
                    }
                    srednia/=(pom.length-3);
                    lista.dodaj(new Student(pom[2].substring(1, pom[2].length()-1), pom[0], pom[1], srednia));
                }
                licznik++;
            }
            br.close();
        }catch(IOException ioe){
            ioe.printStackTrace();
        }
        System.out.println("Wszyscy studenci:");
        lista.wyswietlWszystkich();
        lista.wyswietlWiekszaSrednia(4);
        System.out.println("Wszyscy studenci, ktorzy zaliczyli:");
        lista.zaliczyli().wyswietlWszystkich();
        System.out.println("Wszyscy studenci, ktorzy nie zaliczyli:");
        lista.niezaliczyli().wyswietlWszystkich();
        lista.zmienOcene("123456", 5.5);
        System.out.println("Wszyscy studenci:");
        lista.wyswietlWszystkich();
    }
}
