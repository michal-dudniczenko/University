public class Zadanie4 {
    private Integer[] tab;
    private int rozmiar;
    private int elementy;

    public Zadanie4(){
        this.tab=new Integer[5];
        this.rozmiar=5;
        this.elementy=0;
    }
    public Zadanie4(int rozmiar){
        this.rozmiar = rozmiar;
        this.tab=new Integer[rozmiar];
        this.elementy=0;
    }

    public void push(int n){
        if (elementy<rozmiar){
            if (elementy==0){ tab[0]=n;}
            else{tab[elementy]=n;}
            elementy++;
        }else{
            for (int i=0; i<elementy-1; i++){
                tab[i]=tab[i+1];
            }
            tab[elementy-1]=n;
        }
    }
    public int pop(){
        if (elementy==0)throw new PustyStosException();
        int temp=tab[elementy-1];
        tab[elementy-1]=null;
        elementy--;
        return temp;
    }

    public boolean isEmpty(){
        return elementy==0;
    }
    public boolean isFull(){
        return elementy==rozmiar;
    }

    public int size(){
        return elementy;
    }
    public void wypisz(){
        String s ="[";
        for (int i=0; i<elementy-1; i++){
            s+=tab[i]+", ";
        }
        if (elementy>0){
            s+=tab[elementy-1];
        }
        s+="]";
        System.out.print(s);
    }
}
