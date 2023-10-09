class PustaListaException extends RuntimeException{
    public PustaListaException(){
        System.err.println("Próbowałeś pobrać element z pustej listy!");
    }
}

public class Zadanie5<E> {
    private class Element{
        private E value;
        private Element next;
        private Element prev;

        public Element(E o){
            this.value=o;
        }

        public E getValue() {
            return value;
        }
        public void setValue(E value) {
            this.value = value;
        }
        public Element getNext() {
            return next;
        }
        public void setNext(Element next) {
            this.next = next;
        }
        public Element getPrev() {
            return prev;
        }
        public void setPrev(Element prev) {
            this.prev = prev;
        }

        public String toString(){
            return this.value.toString();
        }
    }

    Element head=null;
    Element tail=null;

    public void addFirst(E o){
        Element elem = new Element(o);
        if (head==null){
            head=elem;
            tail=elem;
        }
        else{
            head.setPrev(elem);
            elem.setNext(head);
            head=elem;
        }
    }
    public void add(E o){
        Element elem = new Element(o);
        if (head==null){
            head=elem;
            tail=elem;
        }
        else{
            elem.setPrev(tail);
            tail.setNext(elem);
            tail=elem;
        }
    }
    public void add(int index, E o){
        Element elem = new Element(o);
        if (index>=this.size())throw new IndexOutOfBoundsException();
        else if (index==0)this.addFirst(o);
        else if (index==this.size()-1)this.add(o);
        else{
            Element temp=head;
            int licznik=0;
            while(licznik<index){
                temp=temp.getNext();
                licznik++;
            }
            elem.setPrev(temp.getPrev());
            elem.setNext(temp);
            temp.getPrev().setNext(elem);
            temp.setPrev(elem);
        }
    }

    public E get(int index){
        Element temp = head;
        if (index>=this.size())throw new IndexOutOfBoundsException();
        else{
            int licznik=0;
            while (licznik<index){
                temp = temp.getNext();
                licznik++;
            }
        }
        return temp.getValue();
    }
    public E getFirst(){
        if (head==null)throw new PustaListaException();
        return head.getValue();
    }
    public E getLast(){
        if (tail==null)throw new PustaListaException();
        return tail.getValue();
    }

    public int indexOf(E o){
        Element elem=head;
        if(head==null)throw new PustaKolejkaException();
        int licznik=0;
        while(!elem.getValue().equals(o) && licznik<this.size()-1){
            elem=elem.getNext();
            licznik++;
        }
        if (this.get(licznik).equals(o))return licznik;
        return -1;
    }

    public void set(int index, E o){
        if (index>=this.size())throw new IndexOutOfBoundsException();
        Element elem = new Element(o);
        Element temp = head;
        int licznik=0;
        while (licznik<index){
            temp=temp.getNext();
            licznik++;
        }
        elem.setNext(temp.getNext());
        elem.setPrev(temp.getPrev());
        temp.getNext().setPrev(elem);
        temp.getPrev().setNext(elem);
    }

    public void remove(int index){
        if(index>=this.size())throw new IndexOutOfBoundsException();
        int licznik=0;
        Element temp = head;
        while (licznik<index){
            temp=temp.getNext();
            licznik++;
        }
        temp.getNext().setPrev(temp.getPrev());
        temp.getPrev().setNext(temp.getNext());
    }
    public void removeFirst(){
        if (head==null) throw new PustaListaException();
        head.getNext().setPrev(null);
        head=head.getNext();
    }
    public void removeLast(){
        if (head==null) throw new PustaListaException();
        tail.getPrev().setNext(null);
        tail=tail.getPrev();
    }
    public void removeElement(E o){
        int temp=this.indexOf(o);
        if (temp!=-1){
            this.remove(temp);
        }
    }

    public void dodajListeNaKoncu(Zadanie5<E> dll){
        dll.head.setPrev(this.tail);
        this.tail.setNext(dll.head);
        this.tail=dll.tail;
    }
    public void dodajListePrzedElementem(Zadanie5<E> dll, int index){
        if (index>=this.size()) throw new IndexOutOfBoundsException();
        Element elem = head;
        int licznik=0;
        while (licznik<index){
            elem=elem.getNext();
            licznik++;
        }
        elem.getPrev().setNext(dll.head);
        dll.head.setPrev(elem.getPrev());
        dll.tail.setNext(elem);
        elem.setPrev(dll.tail);

    }

    public void clear(){
        head=null;
        tail=null;
    }

    public int size(){
        int licznik=0;
        Element elem = head;
        while (elem != null){
            licznik++;
            elem=elem.getNext();
        }
        return licznik;
    }
    public void display(){
        String s = "[";
        Element elem = head;
        while (elem!=tail){
            s+=elem.getValue()+", ";
            elem=elem.getNext();
        }
        if (elem!=null)s+=elem.getValue();
        s+="]";
        System.out.println(s+" size: "+size());
    }

    public static void main(String[] args){
        System.out.println("------------------");
        System.out.println("Zadanie 5:");
        Zadanie5<Integer> zadanie5 = new Zadanie5<>();
        zadanie5.display();

        zadanie5.addFirst(7);
        zadanie5.display();

        zadanie5.addFirst(8);
        zadanie5.display();

        zadanie5.add(71);
        zadanie5.display();

        zadanie5.addFirst(9);
        zadanie5.display();

        zadanie5.add(10);
        zadanie5.display();

        zadanie5.add(2, 79);
        zadanie5.display();

        zadanie5.add(2, 15);
        zadanie5.display();

        System.out.println(zadanie5.get(0));
        System.out.println(zadanie5.getFirst());
        System.out.println(zadanie5.getLast());
        System.out.println(zadanie5.indexOf(7));

        zadanie5.set(4, 20);
        zadanie5.display();
        System.out.println(zadanie5.get(4));

        Zadanie5<Integer> lista1 = new Zadanie5<>();
        lista1.add(1);
        lista1.add(2);
        lista1.add(3);

        zadanie5.dodajListeNaKoncu(lista1);
        zadanie5.display();
        System.out.println(zadanie5.get(8));
        System.out.println(zadanie5.get(2));


        Zadanie5<Integer> lista2 = new Zadanie5<>();
        lista2.add(100);
        lista2.add(101);
        lista2.add(102);

        zadanie5.dodajListePrzedElementem(lista2, 3);

        zadanie5.display();
        System.out.println(zadanie5.get(8));
        System.out.println(zadanie5.get(2));

        zadanie5.remove(2);
        zadanie5.display();

        zadanie5.removeFirst();
        zadanie5.display();

        zadanie5.removeLast();
        zadanie5.display();

        zadanie5.removeElement(71);
        zadanie5.display();

        zadanie5.clear();
        zadanie5.display();

        System.out.println("---------------------");
    }
}
