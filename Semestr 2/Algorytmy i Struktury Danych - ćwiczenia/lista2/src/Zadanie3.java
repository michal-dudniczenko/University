class PustyStosException extends RuntimeException{}

public class Zadanie3<E> {
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
        nowy.setNext(head);
        head=nowy;
    }

    public E pop(){
        if (head==null) throw new PustyStosException();
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
        System.out.println("Zadanie 3:");
        Zadanie3<Integer> zadanie3 = new Zadanie3<>();
        zadanie3.display();

        zadanie3.push(6);
        zadanie3.display();

        zadanie3.push(5);
        zadanie3.display();

        zadanie3.push(4);
        zadanie3.display();

        zadanie3.push(3);
        zadanie3.display();

        System.out.println("pop: "+zadanie3.pop());
        zadanie3.display();

        System.out.println("pop: "+zadanie3.pop());
        zadanie3.display();

        System.out.println("------------------");
    }
}
