package Zadanie2;

public interface Iterator<T>{
    void first(); // przejście na początek struktury
    void last(); // przejście na koniec struktury
    void next(); // przejście do przodu do kolejnego elementu
    void previous(); // przejście do tyłu do poprzedniego elementu
    boolean isDone(); // sprawdzenie, czy iterator wyszedł poza strukturę
    T current(); // odczytanie bieżącego elementu struktury
}

