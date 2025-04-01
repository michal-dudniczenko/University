class FullArrayException extends RuntimeException{}

public class Zadanie1 {
    private Student[] array;

    public Zadanie1(int size){
        this.array = new Student[size];
    }

    public void add(Student s){
        if (array[array.length-1]!=null)throw new FullArrayException();
        for (int i=0; i<array.length; i++){
            if (array[i]==null){
                array[i]=s;
                break;
            }
        }
    }

    public void wyswietlWszystkich(){
        IteratorTablicowy iter = new IteratorTablicowy(array);
        System.out.println("\nPełna lista studentów:");
        while(iter.hasNext()){
            System.out.println(iter.next());
        }
    }

    public void wpiszOcene(int index, double ocena){
        Student temp;
        IteratorTablicowy iter = new IteratorTablicowy(array);
        while(iter.hasNext()){
            temp=iter.next();
            if (temp.getIndex()==index){
                temp.setOcena(ocena);
                break;
            }
        }
    }

    public void sredniaKlasaZwykla(){
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy(array),new ZaliczyliPredicate());
        double suma=0;
        int licznik=0;
        while(iter.hasNext()){
            suma+=iter.next().getOcena();
            licznik++;
        }
        System.out.println("\n(klasa zwykła)średnia ocen studentów z pozytywnymi ocenami: "+suma / licznik);
    }

    public void sredniaKlasaAnonimowa(){
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy(array),new Predicate<>(){
            @Override
            public boolean accept(Student s){
                return s.getOcena()>=3;
            }
        });
        double suma=0;
        int licznik=0;
        while(iter.hasNext()){
            suma+=iter.next().getOcena();
            licznik++;
        }
        System.out.println("\n(klasa anonimowa)średnia ocen studentów z pozytywnymi ocenami: "+suma / licznik);
    }

    public void sredniaWyrazenieLambda(){
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy(array), s -> s.getOcena()>=3);
        double suma=0;
        int licznik=0;
        while(iter.hasNext()){
            suma+=iter.next().getOcena();
            licznik++;
        }
        System.out.println("\n(wyrażenie lambda)średnia ocen studentów z pozytywnymi ocenami: "+suma / licznik);
    }

    public void wyswietlNiezaliczyli(){
        System.out.println("\nLista studentów, którzy nie zaliczyli:");
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy(array), new NiezaliczyliPredicate());
        while (iter.hasNext()){
            System.out.println(iter.next());
        }
    }


    public static void main(String[] args){
        System.out.println("-----------------");
        System.out.println("Zadanie 1:");
        Zadanie1 zadanie1 = new Zadanie1(5);
        zadanie1.add(new Student(1, "Bakłażan", "Bogdan", 3));
        zadanie1.add(new Student(2, "Truskawka", "Tosia", 5.5));
        zadanie1.add(new Student(3, "Banan", "Basia", 4.5));
        zadanie1.add(new Student(4, "Pomidor", "Piotr", 2));
        zadanie1.add(new Student(5, "Marchewka", "Maciej", 3.5));
        zadanie1.wyswietlWszystkich();

        zadanie1.wpiszOcene(3, 2);
        zadanie1.wyswietlWszystkich();

        zadanie1.sredniaKlasaZwykla();
        zadanie1.sredniaKlasaAnonimowa();
        zadanie1.sredniaWyrazenieLambda();

        zadanie1.wyswietlNiezaliczyli();

        zadanie1.wpiszOcene(4, 3.5);
        zadanie1.wyswietlNiezaliczyli();

        System.out.println("-----------------");
    }
}
