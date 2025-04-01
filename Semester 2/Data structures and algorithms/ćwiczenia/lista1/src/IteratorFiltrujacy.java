import java.util.Iterator;

class ZaliczyliPredicate implements Predicate<Student>{
    public boolean accept(Student s){
        return s.getOcena()>=3;
    }
}

class NiezaliczyliPredicate implements Predicate<Student>{
    public boolean accept(Student s){
        return s.getOcena()<3;
    }
}

public class IteratorFiltrujacy<E> implements Iterator<E> {
    private final Iterator<E> iterator;
    private final Predicate<E> predicate;
    private E elemNext;
    private boolean bHasNext;

    public IteratorFiltrujacy(Iterator<E> iterator, Predicate<E> predicate){
        this.iterator=iterator;
        this.predicate=predicate;
        this.elemNext=null;
        this.bHasNext=true;
        findNextValid();
    }

    public void findNextValid(){
        while (iterator.hasNext()){
            elemNext=iterator.next();
            if (predicate.accept(elemNext)){
                return;
            }
        }
        bHasNext=false;
        elemNext=null;
    }
    public boolean hasNext(){
        return bHasNext;
    }

    public E next(){
        E nextValue = elemNext;
        findNextValid();
        return nextValue;
    }
}
