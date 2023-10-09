import java.io.*;

public class Main {
    public static void main(String[] args){
        System.out.println("---------------------");
        System.out.println("Zadanie 1:");
        Zadanie1<Integer> zadanie1 = new Zadanie1<>();
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        zadanie1.push(7);
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        zadanie1.push(8);
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        zadanie1.push(9);
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        System.out.print("pop: "+zadanie1.pop()+" ");
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        System.out.print("pop: "+zadanie1.pop()+" ");
        zadanie1.wypisz();System.out.print(" size: "+zadanie1.size()+"\n");

        System.out.println("---------------------");

        System.out.println("Zadanie 2:");
        Zadanie2<Integer> zadanie2 = new Zadanie2<>();

        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        zadanie2.push(7);
        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        zadanie2.push(8);
        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        zadanie2.push(9);
        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        System.out.print("pop: "+zadanie2.pop()+" ");
        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        System.out.print("pop: "+zadanie2.pop()+" ");
        zadanie2.wypisz();System.out.print(" size: "+zadanie2.size()+"\n");

        System.out.println("---------------------");

        System.out.println("Zadanie 3:");
        Zadanie3<Integer> dll = new Zadanie3<>();
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.addFirst(7);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.addFirst(8);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.add(71);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.addFirst(9);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.add(10);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.add(2, 79);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.add(2, 15);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        System.out.println(dll.get(0));
        System.out.println(dll.getFirst());
        System.out.println(dll.getLast());
        System.out.println(dll.indexOf(7));

        dll.set(4, 20);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");
        System.out.println(dll.get(4));

        Zadanie3<Integer> dll2 = new Zadanie3<>();
        dll2.add(1);
        dll2.add(2);
        dll2.add(3);

        dll.dodajListeNaKoncu(dll2);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");
        System.out.println(dll.get(8));
        System.out.println(dll.get(2));


        Zadanie3<Integer> dll3 = new Zadanie3<>();
        dll3.add(100);
        dll3.add(101);
        dll3.add(102);

        dll.dodajListePrzedElementem(dll3, 3);

        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");
        System.out.println(dll.get(8));
        System.out.println(dll.get(2));

        dll.remove(2);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.removeFirst();
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.removeLast();
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.removeElement(71);
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        dll.clear();
        dll.wypisz();System.out.print(" size: "+dll.size()+"\n");

        System.out.println("---------------------");

        System.out.println("Zadanie 4:");
        Zadanie4 zadanie4 = new Zadanie4(5);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(1);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(2);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(3);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(4);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(5);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(6);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(7);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        System.out.print("pop: "+zadanie4.pop()+" ");
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        System.out.print("pop: "+zadanie4.pop()+" ");
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        System.out.print("pop: "+zadanie4.pop()+" ");
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(9);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(10);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(11);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        zadanie4.push(12);
        zadanie4.wypisz();System.out.print(" size: "+ zadanie4.size()+"\n");

        System.out.println("---------------------");

        System.out.println("Zadanie 5:");
        Zadanie5 zadanie5 = new Zadanie5();
        String wyrazenie="";
        try{
            File f = new File("zadanie5.txt");
            BufferedReader br = new BufferedReader(new FileReader(f));
            while((wyrazenie=br.readLine())!=null) {
                zadanie5.oblicz(wyrazenie);
            }
            br.close();
        }catch(IOException ioe){
            ioe.printStackTrace();
        }

        System.out.println("---------------------");
    }
}
