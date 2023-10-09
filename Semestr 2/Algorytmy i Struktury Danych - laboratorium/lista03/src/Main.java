public class Main {
    public static void main(String[] args){
        System.out.println("-----------------------");
        System.out.println("Zadanie 1:");
        ListaCykliczna<Integer> zadanie1 = new ListaCykliczna<>();

        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.add(5);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.add(3);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.add(1);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        System.out.println(zadanie1.indexOf(1));
        System.out.println(zadanie1.contains(1));
        zadanie1.add(10);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.add(15);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.add(20);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());
        zadanie1.insert(3, 70);
        zadanie1.display();System.out.println(" size: "+zadanie1.size());

        System.out.println(zadanie1.get(4));

        System.out.print(zadanie1.removeFirst()+" ");zadanie1.display();

        System.out.print("\n"+zadanie1.removePos(1)+" ");zadanie1.display();

        zadanie1.clear();
        System.out.println();
        zadanie1.display();System.out.println(" size: "+zadanie1.size());

        System.out.println("-----------------------");
        System.out.println("Zadanie 2:");
        ListaCykliczna<Integer> zadanie2 = new ListaCykliczna<>();
        int n=12;
        for (int i=1; i<=n; i++){
            zadanie2.add(i);
        }
        zadanie2.display();
        System.out.println();
        zadanie2.zadanie2(4);

        System.out.println("-----------------------");
        System.out.println("Zadanie 3:");
        Zadanie3<Integer> zadanie3 = new Zadanie3<>();
        zadanie3.add(1);
        zadanie3.add(2);
        zadanie3.add(3);
        zadanie3.add(4);
        zadanie3.add(5);
        zadanie3.add(6);
        zadanie3.add(7);
        zadanie3.display();

        zadanie3.wyswietlRek();
        System.out.println();

        zadanie3.wyswietlOdTyluRek();
        System.out.println();

        zadanie3.kopiaRek().display();
        System.out.println();

        System.out.println(zadanie3.sumaRek());
        System.out.println();

        System.out.println(zadanie3.liczbaElementowRek());

        System.out.println("-----------------------");
    }
}
