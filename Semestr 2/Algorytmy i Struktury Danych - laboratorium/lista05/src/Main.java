public class Main {
    public static void main(String[] args){
        Samochod[] array = new Samochod[8];
        Samochod s0 = new Samochod("Ford", "zielony", 1900);
        Samochod s1 = new Samochod("Ford", "czerwony", 2020);
        Samochod s2 = new Samochod("Tesla", "czarny", 2023);
        Samochod s3 = new Samochod("BMW", "biały", 2003);
        Samochod s4 = new Samochod("Toyota", "czerwony", 2010);
        Samochod s5 = new Samochod("Ford", "czerwony", 1970);
        Samochod s6 = new Samochod("Ford", "czerwony", 1995);
        Samochod s7 = new Samochod("Ford", "fioletowy", 1995);
        //Samochod s8 = new Samochod("Ford", "fioletowy", 1995);
        array[0]=s0;
        array[1]=s1;
        array[2]=s2;
        array[3]=s3;
        array[4]=s4;
        array[5]=s5;
        array[6]=s6;
        array[7]=s7;
        //array[8]=s8;
        System.out.println("Tablica poczatkowa: ");
        for (Samochod s : array) System.out.println(s);

        System.out.println("\n///// sortowanie kolor //////");
        for (Samochod s : Sortowania.bubbleSort(array, new KomparatorKolor())) System.out.println(s);

        System.out.println("\n///// sortowanie marka //////");
        for (Samochod s : Sortowania.bubbleSort(array, new KomparatorMarka())) System.out.println(s);

        System.out.println("\n///// sortowanie rok produkcji //////");
        for (Samochod s : Sortowania.bubbleSort(array, new KomparatorRokProdukcji())) System.out.println(s);

        System.out.println("\n///// sortowanie marka-kolor-rok produkcji (komparator złożony)//////");
        for (Samochod s : Sortowania.insertionSort(array, new KomparatorB())) System.out.println(s);

        System.out.println("\n///// sortowanie marka-kolor-rok produkcji (złożone compare)//////");
        for (Samochod s : Sortowania.selectionSort(array, new KomparatorC())) System.out.println(s);
    }
}
