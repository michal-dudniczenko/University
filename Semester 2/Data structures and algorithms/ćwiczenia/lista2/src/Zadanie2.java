class PustaKolejkaException extends RuntimeException{}

public class Zadanie2<E> {
    private class Element{
        private E value;
        private Element next;

        public Element(E value) {
            this.value = value;
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

    private Element head=null;

    public void push(E e){
        Element nowy = new Element(e);
        Element elem = head;

        if (head==null){
            head=nowy;
        }
        else{
            while(elem.getNext()!=null){
                elem=elem.getNext();
            }
            elem.setNext(nowy);
        }
    }

    public E pop(){
        if (head==null) throw new PustaKolejkaException();
        E temp = head.getValue();
        head=head.getNext();
        return temp;
    }

    public int size(){
        int licznik=0;
        Element elem=head;
        while(elem!=null){
            elem=elem.getNext();
            licznik++;
        }
        return licznik;
    }

    public void display(){
        String s="[";
        Element elem=head;
        while(elem!=null){
            s+=elem.getValue()+", ";
            elem=elem.getNext();
        }
        if(head!=null)s=s.substring(0,s.length()-2);
        s+="]";
        System.out.println(s+" size: "+size());
    }

    public static void main(String[] args){
        System.out.println("------------------");
        System.out.println("Zadanie 2:");
        Zadanie2<Integer> zadanie2 = new Zadanie2<>();
        zadanie2.display();

        zadanie2.push(6);
        zadanie2.display();

        zadanie2.push(5);
        zadanie2.display();

        zadanie2.push(4);
        zadanie2.display();

        zadanie2.push(3);
        zadanie2.display();

        System.out.println("pop: "+zadanie2.pop());
        zadanie2.display();

        System.out.println("pop: "+zadanie2.pop());
        zadanie2.display();

        System.out.println("------------------");
    }
}
