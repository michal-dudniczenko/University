import java.util.Iterator;

public class IteratorLiczbowy implements Iterator<Integer> {
    private final int end;
    private int current;

    public IteratorLiczbowy(int start, int end){
        this.current=start;
        this.end=end;
    }

    public boolean hasNext(){
        return current<end;
    }

    public Integer next(){
        return current++;
    }
}
