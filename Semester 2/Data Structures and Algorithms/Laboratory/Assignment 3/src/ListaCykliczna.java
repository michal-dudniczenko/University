class PustaListaException extends RuntimeException{}

public class ListaCykliczna<E> {
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
        Element tail=null;

        public boolean add(E e){
            Element elem = new Element(e);
            if (head==null){
                head=elem;
                tail=elem;
                tail.setNext(head);
            }else{
                tail.setNext(elem);
                elem.setNext(head);
                tail=elem;
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
            while(licznik<this.size()){
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

        public E removePos(int index){
            if (index>=this.size())throw new IndexOutOfBoundsException();
            if (index==0){
                return this.removeFirst();
            }
            Element elem=head;
            int licznik=0;
            while(licznik<index-1){
                elem = elem.getNext();
                licznik++;
            }
            if (index==this.size()-1){
                tail=elem;
            }
            Element temp=elem.getNext();
            elem.setNext(temp.getNext());

            return temp.getValue();
        }

        public E removeFirst(){
            if (head==null)throw new PustaListaException();
            Element temp=head;
            if (this.size()==1){
                this.clear();
            }else{
                head=head.getNext();
                tail.setNext(head);
            }
            return temp.getValue();
        }

        public int size(){
            int licznik=1;
            Element elem = head;
            if(elem==null)return 0;
            while(elem!=tail){
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
            tail=null;
        }

        public void display(){
        String s = "[";
        Element elem=head;
        if (this.size()==1){
            s+=head.getValue();
        }else if (this.size()>1){
            while (elem!=tail){
                s+=elem.getValue()+", ";
                elem=elem.getNext();
            }
            s+=elem.getValue();
        }
        s+="]";
        System.out.print(s);
    }
        public void zadanie2(int k){
            Element elem = head;
            while(this.size()>1){
                for (int i=1; i<k; i++){
                    elem = elem.getNext();
                }
                System.out.println(this.removePos(this.indexOf(elem.getValue())));
                elem = elem.getNext();
            }
        }
}

