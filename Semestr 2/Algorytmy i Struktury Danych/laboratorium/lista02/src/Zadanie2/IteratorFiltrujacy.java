package Zadanie2;

class PierwszaPredicate implements Predicate<Integer>{
    public boolean accept(Integer n){
        if (n==2)return true;
        if (n<2 || n%2==0)return false;
        for (int i=3; i<n/2; i+=2){
            if (n % i == 0)return false;
        }
        return true;
    }
}

public class IteratorFiltrujacy<T> {
    private final Iterator<T> iterator;
    private final Predicate<T> predicate;
    private boolean bHasNext;

    public IteratorFiltrujacy(Iterator<T> iterator, Predicate<T> predicate){
        super();
        this.iterator=iterator;
        this.predicate=predicate;
        this.bHasNext=true;
        iterator.first();
        iterator.previous();
        findNextValid();
    }

    public void findNextValid(){
        while (!iterator.isDone()){
            iterator.next();
            if (predicate.accept(iterator.current())&&!iterator.isDone()){
                return;
            }
        }
        bHasNext=false;
    }
    public boolean hasNext(){
        return bHasNext;
    }

    public T next(){
        T nextValue = iterator.current();
        findNextValid();
        return nextValue;
    }
}
