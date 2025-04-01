import java.util.Iterator;

public class IteratorTablicowy implements Iterator<Student> {
    private final Student[] array;
    private int index;

    public IteratorTablicowy(Student[] array){
        this.array=array;
        this.index=0;
    }

    @Override
    public boolean hasNext() {
        return index<array.length;
    }

    public Student next(){
        return array[index++];
    }
}
