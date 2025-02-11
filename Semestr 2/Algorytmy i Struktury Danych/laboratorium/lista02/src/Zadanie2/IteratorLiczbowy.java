package Zadanie2;

public class IteratorLiczbowy implements Iterator{
    private Integer liczba;
    private final Integer zakres;

    public IteratorLiczbowy(Integer zakres){
        this.zakres=zakres;
    }

    public void first(){
        liczba = 1;
    }
    public void last() {
        liczba=zakres;
    }
    public void next(){
        liczba++;
    }
    public void previous(){
        liczba--;
    }
    public boolean isDone(){
        return liczba>zakres;
    }
    public Integer current(){
        return liczba;
    }
}
