class PustyStosException extends RuntimeException{
    public PustyStosException(){
        System.err.println("Próbowałeś pobrać element z pustego stosu!");
    }
}

public class Zadanie2<E>{
    private class Element{
        private E value;
        private Element next;

        Element(E o){
            this.value=o;
        }

        public E getValue(){
            return this.value;
        }
        public void setValue(E o){
            this.value = o;
        }
        public Element getNext(){
            return this.next;
        }
        public void setNext(Element o){
            this.next=o;
        }

        public String toString(){
            return this.value.toString();
        }
    }

    private Element head = null;

    public void push(E e){
        Element newElem = new Element(e);
        if (head!=null) newElem.setNext(head);
        head=newElem;
    }
    public E pop() throws PustyStosException{
        if (head==null) throw new PustyStosException();
        E temp = head.getValue();
        head=head.getNext();
        return temp;



    }

    public boolean isFull(){
        return false;
    }
    public boolean isEmpty(){
        return head==null;
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
        if (elem!=null) {
            while (elem.getNext() != null) {
                s += elem + ", ";
                elem = elem.getNext();
            }
            s += elem;
        }
        s+="]";
        System.out.print(s);
    }
}
