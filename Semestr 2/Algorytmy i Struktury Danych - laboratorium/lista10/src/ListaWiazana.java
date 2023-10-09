public class ListaWiazana{
    public ElementLW head;

    public void add(int key){
        ElementLW elem = new ElementLW(key);
        if (this.head==null){
            elem.repr=elem;
            elem.last=elem;
            elem.size=1;
            this.head=elem;
        }else{
            this.head.last=elem;
            this.head.size++;
            elem.repr=this.head;
            ElementLW temp=this.head;
            while (temp.next!=null){
                temp=temp.next;
            }
            temp.next=elem;
        }
    }

    public ElementLW find(int key){
        ElementLW temp = head;
        while (temp!=null){
            if (temp.key==key) return temp;
            temp=temp.next;
        }
        return temp;
    }

    public String toString(){
        String s="{";
        ElementLW elem=this.head;
        while(elem!=null){
            s+=elem.key+", ";
            elem=elem.next;
        }
        s=s.substring(0, s.length()-2);
        s+="}";
        return s;
    }
}
