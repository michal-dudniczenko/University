class PustaListaException extends RuntimeException{
    public PustaListaException(){
        System.err.println("Próbowałeś pobrać element z pustej listy!");
    }
}

public class Zadanie3<E> {
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
        if (this.size()==0){
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
        if (this.size()==0){
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
        if (this.size()==0) throw new PustaListaException();
        head.getNext().setPrev(null);
        head=head.getNext();
    }
    public void removeLast(){
        if (this.size()==0) throw new PustaListaException();
        tail.getPrev().setNext(null);
        tail=tail.getPrev();
    }
    public void removeElement(E o){
        int temp=this.indexOf(o);
        if (temp!=-1){
            this.remove(temp);
        }
    }

    public void dodajListeNaKoncu(Zadanie3<E> dll){
        dll.head.setPrev(this.tail);
        this.tail.setNext(dll.head);
        this.tail=dll.tail;
    }
    public void dodajListePrzedElementem(Zadanie3<E> dll, int index){
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
    public void wypisz(){
        String s = "[";
        Element elem = head;
        while (elem!=tail){
            s+=elem.getValue()+", ";
            elem=elem.getNext();
        }
        if (elem!=null)s+=elem.getValue();
        s+="]";
        System.out.print(s);
    }
}
