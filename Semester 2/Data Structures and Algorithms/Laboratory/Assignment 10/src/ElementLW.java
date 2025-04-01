public class ElementLW {
    public int key;
    public ElementLW next;
    public ElementLW last;
    public ElementLW repr;
    public int size;

    public ElementLW(int key){
        this.key=key;
        this.next=null;
        this.last=null;
        this.repr=null;
    }
}