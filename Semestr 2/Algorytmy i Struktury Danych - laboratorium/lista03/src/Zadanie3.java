public class Zadanie3<E> {
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
    }

    Element head=null;

    public boolean add(E e){
        Element nowy = new Element(e);
        if (head==null) {
            head = nowy;
        }else{
            Element elem = head;
            while(elem.getNext()!=null){
                elem = elem.getNext();
            }
            elem.setNext(nowy);
        }
        return true;
    }
    public boolean insert(int index, E e){
        if (index>=this.size())throw new IndexOutOfBoundsException();
        Element nowy=new Element(e);
        Element elem=head;
        int licznik=0;
        while (licznik<index-1){
            elem=elem.getNext();
            licznik++;
        }
        nowy.setNext(elem.getNext());
        elem.setNext(nowy);

        return true;
    }

    public int indexOf(E e){
        int licznik=0;
        Element elem=head;
        while(elem.getNext()!=null){
            if (elem.getValue().equals(e))return licznik;
            elem=elem.getNext();
            licznik++;
        }
        return -1;
    }
    public boolean contains(E e){
        return this.indexOf(e)!=-1;
    }

    public E get(int index){
        if (index>=this.size())throw new IndexOutOfBoundsException();
        int licznik=0;
        Element elem=head;
        while(licznik<index){
            elem=elem.getNext();
            licznik++;
        }
        return elem.getValue();
    }

    public E remove(int index){
        if (index>=this.size())throw new IndexOutOfBoundsException();
        int licznik=0;
        Element elem=head;
        while(licznik<index-1){
            elem=elem.getNext();
            licznik++;
        }
        Element temp=elem.getNext();
        elem.setNext(temp.getNext());
        return temp.getValue();
    }

    public int size(){
        int licznik=1;
        Element elem = head;
        if(elem==null)return 0;
        while(elem.getNext()!=null){
            elem=elem.getNext();
            licznik++;
        }
        return licznik;
    }
    public boolean isEmpty(){
        return head==null;
    }
    public void clear(){
        head=null;
    }

    public void display(){
        String s = "[";
        Element elem=head;
        if (this.size()==1){
            s+=head.getValue();
        }else if (this.size()>1){
            while (elem.getNext()!=null){
                s+=elem.getValue()+", ";
                elem=elem.getNext();
            }
            s+=elem.getValue();
        }
        s+="]";
        System.out.println(s);
    }
    public void wyswietlRek(){
        wyswietlRek(head);
    }
    private void wyswietlRek(Element e){
        if (e==null) return;
        System.out.println(e.getValue());
        wyswietlRek(e.getNext());
    }

    public void wyswietlOdTyluRek(){
        wyswietlOdTyluRek(head);
    }
    private void wyswietlOdTyluRek(Element e){
        if (e==null) return;
        wyswietlOdTyluRek(e.getNext());
        System.out.println(e.getValue());
    }

    public Zadanie3<E> kopiaRek(){
        return kopiaRek(head, new Zadanie3<E>());
    }
    private Zadanie3<E> kopiaRek(Element e, Zadanie3<E> accumulator){
        if (e==null) return accumulator;
        accumulator.add(e.getValue());
        return kopiaRek(e.getNext(), accumulator);

    }

    public int sumaRek(){
        return sumaRek(head);
    }
    private int sumaRek(Element e){
        if (e==null)return 0;
        return (int)e.getValue()+ sumaRek(e.getNext());
    }

    public int liczbaElementowRek(){
        return liczbaElementowRek(head);
    }
    private int liczbaElementowRek(Element e){
        if (e==null)return 0;
        return 1 + liczbaElementowRek(e.getNext());
    }
}

