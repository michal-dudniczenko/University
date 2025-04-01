public class Zadanie4<E> {
    private Object[] array;
    private int size;
    private int liczbaElementow;

    public Zadanie4(int size) {
        this.array = new Object[size];
        this.size = size;
        this.liczbaElementow=0;
    }

    public void push(E e){
        if (liczbaElementow<size){
            array[liczbaElementow]=e;
            liczbaElementow++;
        }
        else{
            for (int i=0; i<size-1; i++){
                array[i]=array[i+1];
            }
            array[size-1]=e;
        }
    }

    public E pop(){
        if (liczbaElementow==0)throw new PustyStosException();
        @SuppressWarnings("unchecked")
        E temp = (E)array[liczbaElementow-1];
        array[liczbaElementow-1]=null;
        liczbaElementow--;
        return temp;
    }

    public int size(){
        return liczbaElementow;
    }

    public void display(){
        String s="[";
        for (int i=0; i<liczbaElementow-1; i++){
            s+=array[i]+", ";
        }
        if(liczbaElementow!=0){
            s+=array[liczbaElementow-1];
        }
        s+="]";
        System.out.println(s+" size: "+size());
    }

    public static void main(String[] args){
        System.out.println("------------------");
        System.out.println("Zadanie 4:");
        Zadanie4<Integer> zadanie4 = new Zadanie4<>(5);
        zadanie4.display();

        zadanie4.push(1);
        zadanie4.display();

        zadanie4.push(2);
        zadanie4.display();

        zadanie4.push(3);
        zadanie4.display();

        zadanie4.push(4);
        zadanie4.display();

        zadanie4.push(5);
        zadanie4.display();

        zadanie4.push(6);
        zadanie4.display();

        zadanie4.push(7);
        zadanie4.display();

        System.out.println("pop: "+zadanie4.pop());
        zadanie4.display();

        System.out.println("pop: "+zadanie4.pop());
        zadanie4.display();

        System.out.println("pop: "+zadanie4.pop());
        zadanie4.display();

        System.out.println("------------------");
    }
}
