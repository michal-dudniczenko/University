import java.util.ArrayList;

public class DisjointSetLW {
    private ArrayList<ListaWiazana> listy;

    public DisjointSetLW(){
        this.listy=new ArrayList<>();
    }

    public void makeSet(int x){
        ListaWiazana temp = new ListaWiazana();
        temp.add(x);
        listy.add(temp);
    }

    public ElementLW findSet(int x){
        ElementLW elem=null;
        for (ListaWiazana lw : listy){
            elem = lw.find(x);
            if (elem!=null){
                return elem.repr;
            }
        }
        return elem;
    }

    private void removeSet(ElementLW repr){
        int i=0;
        for (ListaWiazana lw : listy){
            if (lw.head==repr){
                listy.remove(i);
                return;
            }
            i++;
        }
    }

    public void union(int x, int y){
        ElementLW repr1 = findSet(x);
        ElementLW repr2 = findSet(y);

        repr1.last.next=repr2;
        repr1.last=repr2.last;

        ElementLW temp=repr2;
        while(temp!=null){
            temp.repr=repr1;
            temp=temp.next;
        }
        repr1.size+=repr2.size;
        removeSet(repr2);
    }

    public void unionWywazanie(int x, int y){
        ElementLW repr1 = findSet(x);
        ElementLW repr2 = findSet(y);

        if (repr2.size<=repr1.size) {

            repr1.last.next = repr2;
            repr1.last = repr2.last;

            ElementLW temp = repr2;
            while (temp != null) {
                temp.repr = repr1;
                temp = temp.next;
            }
            repr1.size+=repr2.size;
            removeSet(repr2);
        }else{
            repr2.last.next = repr1;
            repr2.last = repr1.last;

            ElementLW temp = repr1;
            while (temp != null) {
                temp.repr = repr2;
                temp = temp.next;
            }
            repr2.size+=repr1.size;
            removeSet(repr1);
        }
    }

    public void display(){
        System.out.print("\n[");
        for (ListaWiazana lw : listy){
            System.out.print(lw);
            System.out.print(", ");
        }
        System.out.print("]\n");
    }
}
