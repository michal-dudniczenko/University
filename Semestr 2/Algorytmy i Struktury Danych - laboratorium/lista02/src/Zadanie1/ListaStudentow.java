package Zadanie1;

public class ListaStudentow {
    private Student[] lista;
    private int liczbaStudentow;
    private int rozmiar;

    public ListaStudentow(int size){
        this.lista = new Student[size];
        this.rozmiar=size;
    }

    public Student[] getLista() {
        return lista;
    }
    public void setLista(Student[] lista) {
        this.lista = lista;
    }
    public int getLiczbaStudentow() {
        return liczbaStudentow;
    }
    public void setLiczbaStudentow(int liczbaStudentow) {
        this.liczbaStudentow = liczbaStudentow;
    }
    public int getRozmiar() {
        return rozmiar;
    }
    public void setRozmiar(int rozmiar) {
        this.rozmiar = rozmiar;
    }

    public void dodaj(Student s){
        if (liczbaStudentow<rozmiar){
            this.lista[liczbaStudentow]=s;
        }else{
            Student[] temp = new Student[rozmiar+1];
            IteratorTablicowy<Student> iter = new IteratorTablicowy<>(lista);
            int licznik=0;
            while(iter.hasNext()){
                temp[licznik]=iter.next();
                licznik++;
            }
            temp[temp.length-1] = s;
            setLista(temp);
            this.rozmiar=this.rozmiar+1;
        }
        setLiczbaStudentow(getLiczbaStudentow()+1);
    }
    public void wyswietlWszystkich(){
        IteratorTablicowy<Student> iter = new IteratorTablicowy<>(lista);
        while(iter.hasNext()){
            System.out.println(iter.next());
        }
        System.out.println("----------------------------------------------");
    }

    public void wyswietlWiekszaSrednia(double srednia){
        System.out.println("Studenci z oceną większą niż: "+srednia+" :");
        Student s;
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy<>(lista), new OcenaWiekszaPredicate(srednia));
        while(iter.hasNext()){
            s = iter.next();
            System.out.println(s);
        }
        System.out.println("----------------------------------------------");
    }

    public ListaStudentow zaliczyli(){
        ListaStudentow temp = new ListaStudentow(0);
        Student s;
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy<>(lista), new OcenaWiekszaRownaPredicate(3));
        while(iter.hasNext()){
            s=iter.next();
            temp.dodaj(s);
        }
        return temp;
    }

    public ListaStudentow niezaliczyli(){
        ListaStudentow temp = new ListaStudentow(0);
        Student s;
        IteratorFiltrujacy<Student> iter = new IteratorFiltrujacy<>(new IteratorTablicowy<>(lista), new OcenaMniejszaPredicate(3));
        while(iter.hasNext()){
            s=iter.next();
            temp.dodaj(s);
        }
        return temp;
    }

    public void zmienOcene(String indeks, double ocena){
        double temp = 0;
        Student s;
        IteratorTablicowy<Student> iter = new IteratorTablicowy<>(lista);
        while(iter.hasNext()){
            s=iter.next();
            if (s.getIndeks().equals(indeks)){
                temp = s.getSr_ocena();
                s.setSr_ocena(ocena);
            }
        }
        System.out.println("//// zmieniono ocene studentowi o indeksie "+indeks+" z "+temp+"  na "+ocena+" ////");
    }
}
