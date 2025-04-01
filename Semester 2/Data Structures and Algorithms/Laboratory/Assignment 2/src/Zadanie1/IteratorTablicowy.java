package Zadanie1;

import java.util.Iterator;
import java.util.NoSuchElementException;

public class IteratorTablicowy<T> implements Iterator<T>{
    private final T[] tablica;
    private int pozycja=0;

    public IteratorTablicowy(T[] tablica){
        this.tablica=tablica;
    }

    public boolean hasNext(){
        return (pozycja<tablica.length && tablica[pozycja]!=null);
    }

    public T next() throws NoSuchElementException{
        if (hasNext()){
            return tablica[pozycja++];
        }
        else{
            throw new NoSuchElementException();
        }
    }
}
