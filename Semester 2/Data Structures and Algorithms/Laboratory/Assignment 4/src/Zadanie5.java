public class Zadanie5 {
    private class Element{
        private int value;
        private Element next;

        Element(int o){
            this.value=o;
        }

        public int getValue(){
            return this.value;
        }
        public void setValue(int o){
            this.value = o;
        }
        public Element getNext(){
            return this.next;
        }
        public void setNext(Element o){
            this.next=o;
        }
    }

    private Element head = null;

    public void push(int e){
        Element newElem = new Element(e);
        if (head!=null) newElem.setNext(head);
        head=newElem;
    }
    public int pop() throws PustyStosException{
        if (head==null) throw new PustyStosException();
        int temp = head.getValue();
        head=head.getNext();
        return temp;



    }

    public boolean isFull(){
        return false;
    }
    public boolean isEmpty(){
        return head==null;
    }

    public void oblicz(String wyrazenie){
        String[]tokeny=wyrazenie.split(" ");
        for (String s : tokeny){
            if (s.equals("+")){
                int liczba1 = this.pop();
                int liczba2 = this.pop();
                this.push(liczba1+liczba2);
            }else if (s.equals("*")){
                int liczba1 = this.pop();
                int liczba2 = this.pop();
                this.push(liczba1*liczba2);
            }else{
                this.push(Integer.parseInt(s));
            }
        }
        System.out.println(wyrazenie);
        System.out.println("wynik = "+this.pop());
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
