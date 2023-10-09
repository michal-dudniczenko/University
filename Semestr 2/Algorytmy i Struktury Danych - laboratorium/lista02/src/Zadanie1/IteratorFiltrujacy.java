package Zadanie1;

import java.util.Iterator;

class OcenaWiekszaPredicate implements Predicate<Student>{
    private final double ocena;

    public OcenaWiekszaPredicate(double ocena){
        this.ocena=ocena;
    }

    public boolean accept(Student s){
        return s.getSr_ocena()>this.ocena;
    }
}
class OcenaWiekszaRownaPredicate implements Predicate<Student>{
    private final double ocena;

    public OcenaWiekszaRownaPredicate(double ocena){
        this.ocena=ocena;
    }

    public boolean accept(Student s){
        return s.getSr_ocena()>=this.ocena;
    }
}
class OcenaMniejszaPredicate implements Predicate<Student>{
    private final double ocena;

    public OcenaMniejszaPredicate(double ocena){
        this.ocena=ocena;
    }

    public boolean accept(Student s){
        return s.getSr_ocena()<this.ocena;
    }
}

public class IteratorFiltrujacy<T> implements Iterator<T>{
    private final Iterator<T> iterator;
    private final Predicate<T> predicate;
    private T elemNext;
    private boolean bHasNext;

    public IteratorFiltrujacy(Iterator<T> iterator, Predicate<T> predicate){
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

    public T next(){
        T nextValue = elemNext;
        findNextValid();
        return nextValue;
    }
}

