public class Zadanie1<E> {
    private class Element {
        private E value;
        private Element next;

        public Element(E data) {
            this.value = data;
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

        public String toString(){
            return this.value.toString();
        }
    }

    private final Element sentinel;
    private final Element head;

    public Zadanie1() {
        sentinel = new Element(null);
        head=sentinel;
    }

    public void insert(int index, E e) {
        if (index > this.size())
            throw new IndexOutOfBoundsException();
        Element nowy = new Element(e);
        Element elem = head;
        int licznik = 0;
        while (licznik < index) {
            elem = elem.getNext();
            licznik++;
        }
        nowy.setNext(elem.getNext());
        elem.setNext(nowy);
    }

    public E get(int index) {
        if (index >= this.size())
            throw new IndexOutOfBoundsException();
        int licznik = 0;
        Element elem = head;
        while (licznik < index) {
            elem = elem.getNext();
            licznik++;
        }
        return elem.getNext().getValue();
    }

    public int size() {
        int licznik = 0;
        Element elem = head;
        while (elem.getNext() != null) {
            elem = elem.getNext();
            licznik++;
        }
        return licznik;
    }

    public void clear() {
        head.setNext(null);
    }

    public E delete(int index) {
        if (index>=this.size())
            throw new IndexOutOfBoundsException();
        Element elem = head;
        int licznik=0;
        while(licznik<index){
            elem=elem.getNext();
            licznik++;
        }
        Element temp = elem.getNext();
        elem.setNext(temp.getNext());
        return temp.getValue();
    }

    public boolean deleteValue(E e){
        Element elem=head;
        while (elem.getNext()!=null){
            if (elem.getNext().getValue().equals(e)) {
                elem.setNext(elem.getNext().getNext());
                return true;
            }
            elem=elem.getNext();
        }
        return false;
    }

    public E set(int index, E e){
        if (index>=this.size())
            throw new IndexOutOfBoundsException();
        Element elem = head;
        int licznik=0;
        while(licznik<index){
            elem=elem.getNext();
            licznik++;
        }
        E temp = elem.getNext().getValue();
        elem.getNext().setValue(e);
        return temp;
    }

    public int indexOf(E e){
        Element elem=head;
        int licznik=0;
        while (elem.getNext()!=null){
            if (elem.getNext().getValue().equals(e)) {
                return licznik;
            }
            elem=elem.getNext();
            licznik++;
        }
        return -1;
    }

    public void display(){
        String s="[";
        Element elem=head;
        while(elem.getNext()!=null){
            s+=elem.getNext().getValue();
            elem=elem.getNext();
            if(elem.getNext()!=null){
                s+=", ";
            }
        }
        s+="]";
        System.out.println(s+" size: "+size());

    }
    public static void main(String[] args){
        System.out.println("------------------");
        System.out.println("Zadanie 1:");
        Zadanie1<Integer> zadanie1 = new Zadanie1<>();
        zadanie1.display();

        zadanie1.insert(0, 5);
        zadanie1.display();

        zadanie1.insert(0, 4);
        zadanie1.display();

        zadanie1.insert(0, 3);
        zadanie1.display();

        zadanie1.insert(1, 10);
        zadanie1.display();

        System.out.println(zadanie1.get(2));

        System.out.println(zadanie1.indexOf(5));

        System.out.println(zadanie1.delete(2));
        zadanie1.display();

        System.out.println(zadanie1.deleteValue(5));
        zadanie1.display();

        System.out.println(zadanie1.set(0, 10));
        zadanie1.display();

        zadanie1.clear();
        zadanie1.display();

        System.out.println("------------------");
    }
}
