import java.util.ArrayList;
import java.util.Iterator;

public class ListaStudentow {
    private ArrayList<Student> lista;

    public ListaStudentow(){
        this.lista = new ArrayList<>();
    }

    public ArrayList<Student> getLista() {
        return lista;
    }
    public void setLista(ArrayList<Student> lista) {
        this.lista = lista;
    }

    public void dodaj(Student s){
        this.lista.add(s);
    }
    public void wyswietlWszystkich(){
        Iterator<Student> iter = this.lista.iterator();
        while(iter.hasNext()){
            System.out.println(iter.next());
        }
        System.out.println("----------------------------------------------");
    }
    public void wyswietlWiekszaSrednia(double srednia){
        System.out.println("Studenci z oceną większą niż: "+srednia+" :");
        Student s;
        Iterator<Student> iter = this.lista.iterator();
        while(iter.hasNext()){
            s = iter.next();
            if (s.getSr_ocena()>srednia){
                System.out.println(s);
            }
        }
        System.out.println("----------------------------------------------");
    }
    public ListaStudentow zaliczyli(){
        ListaStudentow temp = new ListaStudentow();
        Student s;
        Iterator<Student> iter = this.lista.iterator();
        while(iter.hasNext()){
            s=iter.next();
            if (s.getSr_ocena()>=3){
                temp.dodaj(s);
            }
        }
        return temp;
    }
    public ListaStudentow niezaliczyli(){
        ListaStudentow temp = new ListaStudentow();
        Student s;
        Iterator<Student> iter = this.lista.iterator();
        while(iter.hasNext()){
            s=iter.next();
            if (s.getSr_ocena()<3){
                temp.dodaj(s);
            }
        }
        return temp;
    }
    public void zmienOcene(String indeks, double ocena){
        double temp = 0;
        Student s;
        Iterator<Student> iter = this.lista.iterator();
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
